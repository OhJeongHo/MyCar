using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    // �̱���
    public static GameManager instance = null;
    // ���� ���
    public string filePath;
    // ���� ��� ���� ���
    public string recordPath;
    // ���õ� ���� ������ ��ȣ
    public static int slotNum;
    // ���� ���� ���� �÷��̾��� ����
    public static PlayerData nowPlayer = new PlayerData();

    public AudioSource bgm;
    public AudioClip startbgm, mainbgm, trackbgm;

    #region ���̽� ����
    public int laps;
    public int modeCnt = 0; // ���õ� ��尡 ��������
    public float maxSpeed;  // ���� �ִ� �ӵ� ����
    #endregion

    private void Awake()
    {
        #region �̱���
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion
        // ���� ��� ĳ��
        filePath = Application.persistentDataPath;
        recordPath = filePath + "/record";
        print(filePath);
        print(recordPath);
        bgm = GetComponent<AudioSource>();
    }

    // ���õ� ������ ������ ���� ���, ���� �÷��̾��� ����
    public void Save(PlayerData data, string slotNum)
    {
        data.time = System.DateTime.Now.ToString(); // �����Ҷ� �ð� ����
        string jdata = JsonUtility.ToJson(data);
        File.WriteAllText(filePath + slotNum, jdata);
    }

    // �ҷ�����
    public void Load(string slotNum)
    {
        string str = File.ReadAllText(filePath + slotNum);
        nowPlayer = JsonUtility.FromJson<PlayerData>(str);
        nowPlayer.time = System.DateTime.Now.ToString();
    }

    // ���� ������ �����ϱ�
    public void Delete(string slotNum)
    {
        File.Delete(filePath + slotNum);
    }

    // ���� ����
    public void Exit()
    {
        Save(nowPlayer, slotNum.ToString());
        Application.Quit();
    }
}
