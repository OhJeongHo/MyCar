using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class StartScene : MonoBehaviour
{
    public GameObject[] intro;
    float introTime;
    bool introSkip;
    public GameObject AllUI;
    public Button loadBtn;
    public GameObject popUp; // 데이터슬롯 전체창 팝업
    public Text title;
    public GameObject[] inputPlayerName;
    public Button[] slotBtn; // 슬롯 단일 버튼
    public GameObject setting;
    public GameObject settingPanel;
    public InputField[] inputName;
    public Text[] inSlotText;
    // 새게임인지 로드게임인지
    bool newgame;
    public GameObject[] reset;
    // 불러오기
    public Text nameText;
    // 슬롯에 저장값이 존재하는지 여부
    bool[] dataSlot = new bool[3];
    // 플레이어 생성 및 불러오기 버튼
    public GameObject[] createPlayer = new GameObject[3];
    public GameObject[] loadPlayer = new GameObject[3];
    public Text[] playerNameText = new Text[3];
    // public Text[] timeText = new Text[3];
    public Image back;

    AudioSource audio;


    private void Start()
    {
        SoundManager.instance.effect = true;
        #region 이어서하기 여부 판단
        // 슬롯에 데이터 존재유무 확인
        for (int i = 0; i < 3; i++)
        {
            if(File.Exists(GameManager.instance.filePath + i.ToString()))
            {
                dataSlot[i] = true;
            }
        }
        if (dataSlot[0] || dataSlot[1] || dataSlot[2])
        {
            loadBtn.gameObject.SetActive(true);
        }
        else
        {
            loadBtn.gameObject.SetActive(false);
        }
        #endregion
        for (int i = 0; i < 3; i++)
        {
            print($"{i+1}번째 슬롯에 데이터가 존재? : {dataSlot[i]} ");
        }
        audio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        // 인트로 스킵
        if (intro[0].activeSelf)
        {
            introTime += Time.deltaTime;
            
            if (Input.anyKeyDown && introSkip == false || introTime >= 14f && introSkip == false)
            {
                introSkip = true;
                introTime = 0;
                print("스킵발동!");
                AllUI.SetActive(true);
            }
            if (introSkip == true)// && introTime >= 1
            {
                intro[0].SetActive(false);
                intro[1].SetActive(false);
                GameManager.instance.bgm.clip = GameManager.instance.startbgm;
                GameManager.instance.bgm.volume = 0.3f;
                GameManager.instance.bgm.Play();
            }
            return;
        }
        // esc키로 세팅창 팝업
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingPopUp();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == true)
            {
                if (SoundManager.instance.effect == true)
                {
                    audio.Play();
                }
            }
        }
    }
    public void EffectSound()
    {
        if (SoundManager.instance.effect == true)
        {
            SoundManager.instance.effect = false;
        }
        else
        {
            SoundManager.instance.effect = true;
        }
    }
    // 세팅창 팝업
    public void SettingPopUp()
    {
        if (setting.activeSelf)
        {
            setting.SetActive(false);
            settingPanel.SetActive(false);
        }
        else
        {
            setting.SetActive(true);
            settingPanel.SetActive(true);
        }
    }
    // 데이터 슬롯 창 팝업 // 데이터 유무에 따라서 다르게 슬롯 활성화
    public void DataWindowPopUp(bool select)
    {
        newgame = select;
        popUp.SetActive(true);
        title.text = newgame ? "저장할 슬롯을 선택하세요" : "불러올 데이터를 선택하세요";
        for (int i = 0; i < dataSlot.Length; i++)
        {
            if (dataSlot[i])
            {
                createPlayer[i].SetActive(false);
                loadPlayer[i].SetActive(true);
                // 플레이어 이름 가져오기
                string s = File.ReadAllText(GameManager.instance.filePath + i.ToString());
                PlayerData temp = JsonUtility.FromJson<PlayerData>(s);
                playerNameText[i].text = $"플레이어명 : {temp.name}\n마지막 시간 : {temp.time}";
                //timeText[i].text = temp.time;
            }
            else
            {
                loadPlayer[i].SetActive(false);
                createPlayer[i].SetActive(true);
            }
        }
    }

    // 비어있는 슬롯을 클릭했을 때 비어있다면 입력창 활성화하고,
    // 저장된 데이터가 존재한다면, 삭제하냐고 묻는다.
    public void DataSelect(int num)
    {
        if (newgame) // 새로운 시작을 누른 경우
        {
            if (createPlayer[num].activeSelf) // 빈공간이면 데이터 생성
            {
                // 플레이어 닉네임 입력 활성화
                inputPlayerName[num].SetActive(true);
                GameManager.slotNum = num;
                // 그 이외의 슬롯버튼 전부 비활성화
                slotBtn[0].interactable = false;
                slotBtn[1].interactable = false;
                slotBtn[2].interactable = false;
            }
            else // 데이터가 있는 곳을 터치했으면 데이터 제거하냐고 물어봄
            {
                loadPlayer[num].SetActive(false);
                createPlayer[num].SetActive(true);
                inSlotText[num].gameObject.SetActive(false);
                reset[num].SetActive(true);
                for (int i = 0; i < slotBtn.Length; i++)
                {
                    slotBtn[i].interactable = false;
                }
            }
        }
        else // 이어하기를 누른 경우
        {
            if (createPlayer[num].activeSelf) // 빈 공간을 터치한 경우
            {
                inSlotText[num].text = "데이터가 없습니다!!\n데이터가 저장된 슬롯을 선택해주세요!!";
            }
            else // 데이터가 있는 곳을 터치
            {
                CreateAndLoadData(num);
                LobbyGo();
            }
        }
    }
    public void DataResetYes(int num)
    {
        inSlotText[num].gameObject.SetActive(true);
        reset[num].SetActive(false);
        for (int i = 0; i < slotBtn.Length; i++)
        {
            slotBtn[i].interactable = true;
        }
        GameManager.instance.Delete(num.ToString());
    }
    public void DataResetNo(int num)
    {
        inSlotText[num].gameObject.SetActive(true);
        reset[num].SetActive(false);
        for (int i = 0; i < slotBtn.Length; i++)
        {
            slotBtn[i].interactable = true;
        }
        loadPlayer[num].SetActive(true);
        createPlayer[num].SetActive(false);
    }
    public void NewPlayerCreate(int num)
    {
        // 플레이어 닉네임 입력 활성화
        inputPlayerName[num].SetActive(true);
        GameManager.slotNum = num;
        // 그 이외의 슬롯버튼 전부 비활성화
        slotBtn[0].interactable = false;
        slotBtn[1].interactable = false;
        slotBtn[2].interactable = false;
    }

    // 최종적인 생성 버튼 혹은 데이터 로드 버튼에 연결
    public void CreateAndLoadData(int num)
    {
        if (newgame)
        {
            // 플레이어 새로 생성
            PlayerData player = new PlayerData();
            // 현재 플레이어에 대입
            GameManager.nowPlayer = player;
            GameManager.nowPlayer.name = inputName[num].text;
            GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
            LobbyGo();
        }
        else
        {
            string s = File.ReadAllText(GameManager.instance.filePath + num.ToString());
            PlayerData temp = JsonUtility.FromJson<PlayerData>(s);
            GameManager.nowPlayer = temp;
            GameManager.slotNum = num;
        }
    }
    // 저장 과정을 전부 넘겨야 할 듯.
    public void LobbyGo()
    {
        StartCoroutine(FadeOut(back, 1f));
        //SceneManager.LoadScene("Main_02");
    }

    public void PopUpExit()
    {
        popUp.SetActive(false);
    }
    IEnumerator FadeOut(Image target, float time)
    {
        var t = 0f;
        float a = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            target.color = new Color(0, 0, 0, a);
            a += Time.deltaTime * 5f;
            GameManager.instance.bgm.volume -= 0.008f;
            yield return null;
        }
        SceneManager.LoadScene("Main_02");
    }
}
