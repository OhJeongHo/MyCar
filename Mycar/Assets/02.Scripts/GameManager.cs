using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    // 싱글톤
    public static GameManager instance = null;
    // 파일 경로
    public string filePath;
    // 경주 기록 저장 경로
    public string recordPath;
    // 선택된 저장 슬롯의 번호
    public static int slotNum;
    // 현재 게임 중인 플레이어의 정보
    public static PlayerData nowPlayer = new PlayerData();

    public AudioSource bgm;
    public AudioClip startbgm, mainbgm, trackbgm;

    #region 레이싱 정보
    public int laps;
    public int modeCnt = 0; // 선택된 모드가 무엇인지
    public float maxSpeed;  // 허용된 최대 속도 제한
    #endregion

    private void Awake()
    {
        #region 싱글톤
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
        // 파일 경로 캐싱
        filePath = Application.persistentDataPath;
        recordPath = filePath + "/record";
        print(filePath);
        print(recordPath);
        bgm = GetComponent<AudioSource>();
    }

    // 선택된 슬롯의 데이터 저장 경로, 현재 플레이어의 정보
    public void Save(PlayerData data, string slotNum)
    {
        data.time = System.DateTime.Now.ToString(); // 저장할때 시간 저장
        string jdata = JsonUtility.ToJson(data);
        File.WriteAllText(filePath + slotNum, jdata);
    }

    // 불러오기
    public void Load(string slotNum)
    {
        string str = File.ReadAllText(filePath + slotNum);
        nowPlayer = JsonUtility.FromJson<PlayerData>(str);
        nowPlayer.time = System.DateTime.Now.ToString();
    }

    // 저장 데이터 삭제하기
    public void Delete(string slotNum)
    {
        File.Delete(filePath + slotNum);
    }

    // 게임 종료
    public void Exit()
    {
        Save(nowPlayer, slotNum.ToString());
        Application.Quit();
    }
}
