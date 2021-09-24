using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    public GameObject setting;
    public GameObject settingPanel;
    public GameObject player;
    KartInfomation info;

    public Transform singlePos;
    public Transform ghostPlayerPos;
    public Transform ghostPos;
    public GameObject ghost;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.bgm.clip = GameManager.instance.trackbgm;
        GameManager.instance.bgm.Play();
        GameManager.instance.bgm.volume = 0.6f;
        info = player.GetComponent<KartInfomation>();
        if (GameManager.instance.modeCnt == 0)
        {
            player.transform.position = singlePos.position;
            ghost.gameObject.SetActive(false);
        }
        else if (GameManager.instance.modeCnt == 1)
        {
            player.transform.position = ghostPlayerPos.position;
            ghost.gameObject.SetActive(true);
            ghost.transform.position = ghostPos.position;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PopUpSetting();
        }
    }

    public void Exit()
    {
        GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
        Application.Quit();
    }

    public void PopUpSetting()
    {
        if (!setting.activeSelf)
        {
            Time.timeScale = 0.0f;
            setting.SetActive(true);
            settingPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            setting.SetActive(false);
            settingPanel.SetActive(false);
        }
    }

    public void Quit()
    {
        GameManager.instance.Exit();
    }
}
