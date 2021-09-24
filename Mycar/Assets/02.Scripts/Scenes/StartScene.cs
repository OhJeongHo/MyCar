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
    public GameObject popUp; // �����ͽ��� ��üâ �˾�
    public Text title;
    public GameObject[] inputPlayerName;
    public Button[] slotBtn; // ���� ���� ��ư
    public GameObject setting;
    public GameObject settingPanel;
    public InputField[] inputName;
    public Text[] inSlotText;
    // ���������� �ε��������
    bool newgame;
    public GameObject[] reset;
    // �ҷ�����
    public Text nameText;
    // ���Կ� ���尪�� �����ϴ��� ����
    bool[] dataSlot = new bool[3];
    // �÷��̾� ���� �� �ҷ����� ��ư
    public GameObject[] createPlayer = new GameObject[3];
    public GameObject[] loadPlayer = new GameObject[3];
    public Text[] playerNameText = new Text[3];
    // public Text[] timeText = new Text[3];
    public Image back;

    AudioSource audio;


    private void Start()
    {
        SoundManager.instance.effect = true;
        #region �̾�ϱ� ���� �Ǵ�
        // ���Կ� ������ �������� Ȯ��
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
            print($"{i+1}��° ���Կ� �����Ͱ� ����? : {dataSlot[i]} ");
        }
        audio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        // ��Ʈ�� ��ŵ
        if (intro[0].activeSelf)
        {
            introTime += Time.deltaTime;
            
            if (Input.anyKeyDown && introSkip == false || introTime >= 14f && introSkip == false)
            {
                introSkip = true;
                introTime = 0;
                print("��ŵ�ߵ�!");
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
        // escŰ�� ����â �˾�
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
    // ����â �˾�
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
    // ������ ���� â �˾� // ������ ������ ���� �ٸ��� ���� Ȱ��ȭ
    public void DataWindowPopUp(bool select)
    {
        newgame = select;
        popUp.SetActive(true);
        title.text = newgame ? "������ ������ �����ϼ���" : "�ҷ��� �����͸� �����ϼ���";
        for (int i = 0; i < dataSlot.Length; i++)
        {
            if (dataSlot[i])
            {
                createPlayer[i].SetActive(false);
                loadPlayer[i].SetActive(true);
                // �÷��̾� �̸� ��������
                string s = File.ReadAllText(GameManager.instance.filePath + i.ToString());
                PlayerData temp = JsonUtility.FromJson<PlayerData>(s);
                playerNameText[i].text = $"�÷��̾�� : {temp.name}\n������ �ð� : {temp.time}";
                //timeText[i].text = temp.time;
            }
            else
            {
                loadPlayer[i].SetActive(false);
                createPlayer[i].SetActive(true);
            }
        }
    }

    // ����ִ� ������ Ŭ������ �� ����ִٸ� �Է�â Ȱ��ȭ�ϰ�,
    // ����� �����Ͱ� �����Ѵٸ�, �����ϳİ� ���´�.
    public void DataSelect(int num)
    {
        if (newgame) // ���ο� ������ ���� ���
        {
            if (createPlayer[num].activeSelf) // ������̸� ������ ����
            {
                // �÷��̾� �г��� �Է� Ȱ��ȭ
                inputPlayerName[num].SetActive(true);
                GameManager.slotNum = num;
                // �� �̿��� ���Թ�ư ���� ��Ȱ��ȭ
                slotBtn[0].interactable = false;
                slotBtn[1].interactable = false;
                slotBtn[2].interactable = false;
            }
            else // �����Ͱ� �ִ� ���� ��ġ������ ������ �����ϳİ� ���
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
        else // �̾��ϱ⸦ ���� ���
        {
            if (createPlayer[num].activeSelf) // �� ������ ��ġ�� ���
            {
                inSlotText[num].text = "�����Ͱ� �����ϴ�!!\n�����Ͱ� ����� ������ �������ּ���!!";
            }
            else // �����Ͱ� �ִ� ���� ��ġ
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
        // �÷��̾� �г��� �Է� Ȱ��ȭ
        inputPlayerName[num].SetActive(true);
        GameManager.slotNum = num;
        // �� �̿��� ���Թ�ư ���� ��Ȱ��ȭ
        slotBtn[0].interactable = false;
        slotBtn[1].interactable = false;
        slotBtn[2].interactable = false;
    }

    // �������� ���� ��ư Ȥ�� ������ �ε� ��ư�� ����
    public void CreateAndLoadData(int num)
    {
        if (newgame)
        {
            // �÷��̾� ���� ����
            PlayerData player = new PlayerData();
            // ���� �÷��̾ ����
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
    // ���� ������ ���� �Ѱܾ� �� ��.
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
