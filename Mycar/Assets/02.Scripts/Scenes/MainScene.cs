using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum KartState
{
    KART_01,
    KART_02,
    KART_03
}

public class MainScene : MonoBehaviour
{
    public Text playerName;
    public Text money;

    public GameObject[] Kart;
    KartState state = new KartState();

    public GameObject setting;
    public GameObject settingPanel;

    // Start is called before the first frame update
    void Start()
    {
        playerName.text = GameManager.nowPlayer.name;
        money.text = GameManager.nowPlayer.money.ToString();
        state = (KartState)GameManager.nowPlayer.kartstate;
        KartActive((int)state);
        if (GameManager.instance.bgm.clip != GameManager.instance.mainbgm)
        {
            GameManager.instance.bgm.clip = GameManager.instance.mainbgm;
            GameManager.instance.bgm.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingPopUp();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            MoneyPlus();
        }
    }
    public void Exit()
    {
        GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
        Application.Quit();
    }

    void MoneyPlus()
    {
        GameManager.nowPlayer.money += 100;
        money.text = GameManager.nowPlayer.money.ToString();
    }

    public void KartSelect(int num)
    {
        state = (KartState)num;
        GameManager.nowPlayer.kartstate = (int)state;
        KartActive(num);
        GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
    }

    void KartActive(int num)
    {
        for (int i = 0; i < Kart.Length; i++)
        {
            if (num == i)
            {
                Kart[num].SetActive(true);
                continue;
            }
            Kart[i].SetActive(false);
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

    public void SeletSceneGo()
    {
        SceneManager.LoadScene("Select_01");
    }

    public void RaingGo()
    {
        SceneManager.LoadScene("Select_02");
    }
    public void Quit()
    {
        GameManager.instance.Exit();
    }
}
