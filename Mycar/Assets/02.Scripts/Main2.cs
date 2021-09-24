using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main2 : MonoBehaviour
{
    public Text playerName;
    public Text money;

    public Image back;
    public Transform cameraCurvePos;
    public Transform cameraOriginPos;
    public GameObject screen;
    Vector3 screenOriginPos;
    public Transform SelectCamPos;
    public GameObject selectUI;
    public GameObject[] KartGroup;
    public Text kartName;
    public GameObject[] maxSpeed;
    public GameObject[] axel;
    public GameObject[] boost;
    public GameObject kartLock;
    public Text unlockMoney;
    public GameObject warning;
    public Text warningText;
    bool upgrade;

    // map
    public GameObject mapInfo;
    public GameObject[] sideButton;
    public Transform mapOriginPos;
    public Transform sidePos;
    public Transform mapCamPos;
    public Image map_01;
    public Text lapText;
    public Text modeText;
    public GameObject modeNo;

    public GameObject setting;
    public GameObject settingPanel;


    KartState state = KartState.KART_01;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.bgm.volume = 0;
        if (GameManager.instance.bgm.clip != GameManager.instance.mainbgm)
        {
            GameManager.instance.bgm.clip = GameManager.instance.mainbgm;
            GameManager.instance.bgm.Play();
        }
        StartCoroutine(SoundFadeIn(GameManager.instance.bgm, 0.3f));
        StartCoroutine(FadeIn(back, 1f));
        StartCoroutine(MoveToPosition(Camera.main.transform, cameraCurvePos.position, 0.8f));
        screenOriginPos = screen.transform.position;
        screen.transform.position += new Vector3(0, -5, 0);
        StartCoroutine(MoveToPosition(screen.transform, screenOriginPos, 1f));
        StartCoroutine(MoveToPosition(Camera.main.transform, cameraOriginPos.position, 0.5f, 0.9f));
        playerName.text = GameManager.nowPlayer.name;
    }

    // Update is called once per frame
    void Update()
    {
        money.text = GameManager.nowPlayer.money.ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingPopUp();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.nowPlayer.money += 9999999;
        }
    }
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
    public void MaxSpeedUpgrade()
    {
        switch (state)
        {
            case KartState.KART_01:
                if (GameManager.nowPlayer.money >= 50)
                {
                    GameManager.nowPlayer.money -= 50;
                    GameManager.nowPlayer.kart1_maxSpeed++;
                    upgrade = true;
                }
                else
                {
                    warning.SetActive(true);
                    warningText.text = "업그레이드는 50원이 필요합니다\n게임머니를 획득하세요";
                }
                break;
            case KartState.KART_02:
                if (GameManager.nowPlayer.money >= 50)
                {
                    GameManager.nowPlayer.money -= 50;
                    GameManager.nowPlayer.kart2_maxSpeed++;
                    upgrade = true;
                }
                else
                {
                    warning.SetActive(true);
                    warningText.text = "업그레이드는 50원이 필요합니다\n게임머니를 획득하세요";
                }
                break;
            case KartState.KART_03:
                if (GameManager.nowPlayer.money >= 50)
                {
                    GameManager.nowPlayer.money -= 50;
                    GameManager.nowPlayer.kart3_maxSpeed++;
                    upgrade = true;
                }
                else
                {
                    warning.SetActive(true);
                    warningText.text = "업그레이드는 50원이 필요합니다\n게임머니를 획득하세요";
                }
                break;
        }
        for (int i = 0; i < maxSpeed.Length; i++)
        {
            if (upgrade && maxSpeed[i].gameObject.activeSelf == false)
            {
                maxSpeed[i].SetActive(true);
                upgrade = false;
                return;
            }
        }
        GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
    }
    public void AxelUpgrade()
    {
        switch (state)
        {
            case KartState.KART_01:
                if (GameManager.nowPlayer.money >= 50)
                {
                    GameManager.nowPlayer.money -= 50;
                    GameManager.nowPlayer.kart1_axel++;
                    upgrade = true;
                }
                else
                {
                    warning.SetActive(true);
                    warningText.text = "업그레이드는 50원이 필요합니다\n게임머니를 획득하세요";
                }
                break;
            case KartState.KART_02:
                if (GameManager.nowPlayer.money >= 50)
                {
                    GameManager.nowPlayer.money -= 50;
                    GameManager.nowPlayer.kart2_axel++;
                    upgrade = true;
                }
                else
                {
                    warning.SetActive(true);
                    warningText.text = "업그레이드는 50원이 필요합니다\n게임머니를 획득하세요";
                }
                break;
            case KartState.KART_03:
                if (GameManager.nowPlayer.money >= 50)
                {
                    GameManager.nowPlayer.money -= 50;
                    GameManager.nowPlayer.kart3_axel++;
                    upgrade = true;
                }
                else
                {
                    warning.SetActive(true);
                    warningText.text = "업그레이드는 50원이 필요합니다\n게임머니를 획득하세요";
                }
                break;
        }
        for (int i = 0; i < axel.Length; i++)
        {
            if (upgrade && axel[i].gameObject.activeSelf == false)
            {
                axel[i].SetActive(true);
                upgrade = false;
                return;
            }
        }
        GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
    }
    public void BoostUpgrade()
    {
        switch (state)
        {
            case KartState.KART_01:
                if (GameManager.nowPlayer.money >= 50)
                {
                    GameManager.nowPlayer.money -= 50;
                    GameManager.nowPlayer.kart1_boost++;
                    upgrade = true;
                }
                else
                {
                    warning.SetActive(true);
                    warningText.text = "업그레이드는 50원이 필요합니다\n게임머니를 획득하세요";
                }
                break;
            case KartState.KART_02:
                if (GameManager.nowPlayer.money >= 50)
                {
                    GameManager.nowPlayer.money -= 50;
                    GameManager.nowPlayer.kart2_boost++;
                    upgrade = true;
                }
                else
                {
                    warning.SetActive(true);
                    warningText.text = "업그레이드는 50원이 필요합니다\n게임머니를 획득하세요";
                }
                break;
            case KartState.KART_03:
                if (GameManager.nowPlayer.money >= 50)
                {
                    GameManager.nowPlayer.money -= 50;
                    GameManager.nowPlayer.kart3_boost++;
                    upgrade = true;
                }
                else
                {
                    warning.SetActive(true);
                    warningText.text = "업그레이드는 50원이 필요합니다\n게임머니를 획득하세요";
                }
                break;
        }
        for (int i = 0; i < boost.Length; i++)
        {
            if (upgrade && boost[i].gameObject.activeSelf == false)
            {
                boost[i].SetActive(true);
                upgrade = false;
                return;
            }
        }
        GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
    }

    public void GetKart()
    {
        if (state == KartState.KART_02)
        {
            if (GameManager.nowPlayer.money >= 100)
            {
                GameManager.nowPlayer.money -= 100;
                GameManager.nowPlayer.kart2 = true;
                kartLock.SetActive(false);
                GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
            }
            else
            {
                warning.SetActive(true);
                warningText.text = "게임머니가 부족합니다";
            }
        }
        else if (state == KartState.KART_03)
        {
            if (GameManager.nowPlayer.money >= 200)
            {
                GameManager.nowPlayer.money -= 200;
                GameManager.nowPlayer.kart3 = true;
                kartLock.SetActive(false);
                GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
            }
            else
            {
                warning.SetActive(true);
                warningText.text = "게임머니가 부족합니다";
            }
        }
    }
    public void WarningClose()
    {
        warning.SetActive(false);
    }
    public void SelectKart()
    {
        StartCoroutine(MoveToPosition(Camera.main.transform, SelectCamPos.position, 0.5f));
        StartCoroutine(MoveToPosition(screen.transform, screen.transform.position + new Vector3(0, 6, 0), 0.5f));
        StartCoroutine(DelayActive(selectUI, 0.5f));
        LoadUpgradeData(0);
    }

    public void SelectFinish()
    {
        StartCoroutine(MoveToPosition(Camera.main.transform, cameraOriginPos.position, 0.5f));
        StartCoroutine(MoveToPosition(screen.transform, screenOriginPos, 0.5f));
        selectUI.SetActive(false);
        GameManager.nowPlayer.kartstate = (int)state;
        GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
    }
    public void MapScene()
    {
        StartCoroutine(MoveToPosition(Camera.main.transform, mapCamPos.position, 0.7f));
        StartCoroutine(DelayActive(mapInfo, 0.7f));
        map_01.gameObject.SetActive(true);
    }
    public void RaceStart()
    {
        SceneManager.LoadScene("Game_01");
    }
    public void ModeRight()
    {
        GameManager.instance.modeCnt++;
        if (GameManager.instance.modeCnt > 2)
        {
            GameManager.instance.modeCnt = 0;
        }
        switch (GameManager.instance.modeCnt)
        {
            case 0:
                modeText.text = "1인모드";
                modeNo.SetActive(false);
                break;
            case 1:
                modeText.text = "고스트";
                modeNo.SetActive(false);
                break;
            case 2:
                modeText.text = "멀티대전";
                modeNo.SetActive(true);
                break;
        }
    }
    public void ModeLeft()
    {
        GameManager.instance.modeCnt--;
        if (GameManager.instance.modeCnt < 0)
        {
            GameManager.instance.modeCnt = 2;
        }
        switch (GameManager.instance.modeCnt)
        {
            case 0:
                modeText.text = "1인모드";
                modeNo.SetActive(false);
                break;
            case 1:
                modeText.text = "고스트";
                modeNo.SetActive(false);
                break;
            case 2:
                modeText.text = "멀티대전";
                modeNo.SetActive(true);
                break;
        }
    }
    public void LapPlus()
    {
        print(GameManager.instance.laps);
        if (GameManager.instance.laps == 3)
        {
            return;
        }
        GameManager.instance.laps++;
        print(GameManager.instance.laps);
        lapText.text = GameManager.instance.laps.ToString();
    }
    public void LapMinus()
    {
        if (GameManager.instance.laps == 1)
        {
            return;
        }
        GameManager.instance.laps--;
        lapText.text = GameManager.instance.laps.ToString();
    }
    public void MapClose()
    {
        if (sideButton[0].activeSelf)
        {
            StartCoroutine(MoveToPosition(mapInfo.transform, sidePos.position, 0.3f));
            sideButton[1].SetActive(true);
            sideButton[0].SetActive(false);
        }
        else
        {
            StartCoroutine(MoveToPosition(mapInfo.transform, mapOriginPos.position, 0.3f));
            sideButton[1].SetActive(false);
            sideButton[0].SetActive(true);
        }
    }
    public void MapReturn()
    {
        mapInfo.SetActive(false);
        StartCoroutine(MoveToPosition(Camera.main.transform, cameraCurvePos.position, 0.7f));
        StartCoroutine(MoveToPosition(Camera.main.transform, cameraOriginPos.position, 0.4f, 0.8f));
    }
    // 업그레이드 정도 불러오기
    void LoadUpgradeData(int kartnum)
    {
        for (int i = 0; i < maxSpeed.Length; i++)
        {
            maxSpeed[i].SetActive(false);
            axel[i].SetActive(false);
            boost[i].SetActive(false);
        }
        switch (kartnum)
        {
            case 0:
                for (int i = 0; i < GameManager.nowPlayer.kart1_maxSpeed; i++)
                {
                    if (maxSpeed.Length-1 >= i)
                    {
                        maxSpeed[i].SetActive(true);
                    }
                }
                for (int i = 0; i < GameManager.nowPlayer.kart1_axel; i++)
                {
                    if (maxSpeed.Length - 1 >= i)
                    {
                        axel[i].SetActive(true);
                    }
                }
                for (int i = 0; i < GameManager.nowPlayer.kart1_boost; i++)
                {
                    if (maxSpeed.Length - 1 >= i)
                    {
                    boost[i].SetActive(true);

                    }
                }
                break;
            case 1:
                for (int i = 0; i < GameManager.nowPlayer.kart2_maxSpeed; i++)
                {
                    if (maxSpeed.Length - 1 >= i)
                    {
                    maxSpeed[i].SetActive(true);

                    }
                }
                for (int i = 0; i < GameManager.nowPlayer.kart2_axel; i++)
                {
                    if (maxSpeed.Length - 1 >= i)
                    {
                    axel[i].SetActive(true);

                    }
                }
                for (int i = 0; i < GameManager.nowPlayer.kart2_boost; i++)
                {
                    if (maxSpeed.Length - 1 >= i)
                    {
                    boost[i].SetActive(true);

                    }
                }
                break;
            case 2:
                for (int i = 0; i < GameManager.nowPlayer.kart3_maxSpeed; i++)
                {
                    if (maxSpeed.Length - 1 >= i)
                    {
                    maxSpeed[i].SetActive(true);

                    }
                }
                for (int i = 0; i < GameManager.nowPlayer.kart3_axel; i++)
                {
                    if (maxSpeed.Length - 1 >= i)
                    {
                    axel[i].SetActive(true);

                    }
                }
                for (int i = 0; i < GameManager.nowPlayer.kart3_boost; i++)
                {
                    if (maxSpeed.Length - 1 >= i)
                    {
                    boost[i].SetActive(true);

                    }
                }
                break;
        }
    }
    public void RightRotate()
    {
        state++;
        StartCoroutine(MoveToRotation(KartGroup[0].transform, Quaternion.Euler(0, -120, 0), 0.2f));
        switch (state)
        {
            case 0:
                kartName.text = "기본카트";
                kartLock.SetActive(false);
                break;
            case (KartState)1:
                kartName.text = "중급카트";
                if (GameManager.nowPlayer.kart2 == false)
                {
                    kartLock.SetActive(true);
                    unlockMoney.text = "100";
                }
                else
                {
                    kartLock.SetActive(false);
                }
                break;
            case (KartState)2:
                kartName.text = "고급카트";
                if (GameManager.nowPlayer.kart3 == false)
                {
                    kartLock.SetActive(true);
                    unlockMoney.text = "200";
                }
                else
                {
                    kartLock.SetActive(false);
                }
                break;
            default:
                kartName.text = "기본카트";
                kartLock.SetActive(false);
                break;
        }
        if ((int)state > 2)
        {
            state = 0;
        }
        else if ((int)state < 0)
        {
            state = (KartState)2;
        }
        LoadUpgradeData((int)state);
    }
    public void LeftRotate()
    {
        state--;
        StartCoroutine(MoveToRotation(KartGroup[0].transform, Quaternion.Euler(0, 120, 0), 0.2f));
        switch (state)
        {
            case 0:
                kartName.text = "기본카트";
                kartLock.SetActive(false);
                break;
            case (KartState)1:
                kartName.text = "중급카트";
                if (GameManager.nowPlayer.kart2 == false)
                {
                    kartLock.SetActive(true);
                    unlockMoney.text = "100";
                }
                else
                {
                    kartLock.SetActive(false);
                }
                break;
            case (KartState)2:
                kartName.text = "고급카트";
                if (GameManager.nowPlayer.kart3 == false)
                {
                    kartLock.SetActive(true);
                    unlockMoney.text = "200";
                }
                else
                {
                    kartLock.SetActive(false);
                }
                break;
            default:
                kartName.text = "고급카트";
                if (GameManager.nowPlayer.kart3 == false)
                {
                    kartLock.SetActive(true);
                    unlockMoney.text = "200";
                }
                else
                {
                    kartLock.SetActive(false);
                }
                break;
        }
        if ((int)state > 2)
        {
            state = 0;
        }
        else if ((int)state < 0)
        {
            state = (KartState)2;
        }
        LoadUpgradeData((int)state);
    }
    IEnumerator FadeIn(Image target, float time)
    {
        //if (GameManager.instance.bgm.clip != GameManager.instance.mainbgm)
        //{
        //    GameManager.instance.bgm.clip = GameManager.instance.mainbgm;
        //    GameManager.instance.bgm.Play();
        //}
        var t = 0f;
        float a = 1;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            target.color = new Color(0, 0, 0, a);
            a -= Time.deltaTime * 5f;
            yield return null;
        }
    }
    IEnumerator SoundFadeIn(AudioSource sound, float volume)
    {
        while (sound.volume < volume)
        {
            sound.volume += 0.01f;
            yield return null;
        }
        yield return null;
    }
    IEnumerator SoundFadeOut(AudioSource sound, float volume)
    {
        while (sound.volume > volume)
        {
            sound.volume -= 0.01f;
            yield return null;
        }
        yield return null;
    }
    IEnumerator FadeOut(Image target, float time)
    {
        var t = 0f;
        float a = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            target.color = new Color(0, 0, 0, a);
            a += Time.deltaTime * 5.0f;
            GameManager.instance.bgm.volume -= 0.003f;
            yield return null;
        }
        SceneManager.LoadScene("Select_02");
    }
    IEnumerator DelayActive(GameObject obj, float t)
    {
        yield return new WaitForSeconds(t);
        obj.SetActive(true);
    }
    IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove, float waitTime = 0)
    {
        yield return new WaitForSeconds(waitTime);
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }
    IEnumerator MoveToRotation(Transform transform, Quaternion rot, float timeToMove)
    {
        var currentRot = transform.localRotation;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.localRotation = Quaternion.Lerp(currentRot, currentRot*rot, t);
            yield return null;
        }
    }
}
