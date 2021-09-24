using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SelectScene : MonoBehaviour
{
    public Text playerName;
    public Text money;
    public GameObject setting;
    public GameObject settingPanel;
    public GameObject[] Kart;
    

    KartState state = KartState.KART_01;

    // Start is called before the first frame update
    void Start()
    {
        state = (KartState)GameManager.nowPlayer.kartstate;
        KartActive((int)state);
        playerName.text = GameManager.nowPlayer.name;
        money.text = GameManager.nowPlayer.money.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingPopUp();
        }
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
        if (Kart != null)
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
    public void Exit()
    {
        GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
        Application.Quit();
    }

    public void GameSceneGo()
    {
        //GameManager.instance.mainbgm.Stop();
        SceneManager.LoadScene("Game_01");
    }
    public void MainSceneGo()
    {
        SceneManager.LoadScene("Main_01");
    }
    public void Select02SceneGo()
    {
        SceneManager.LoadScene("Select_02");
    }
}
