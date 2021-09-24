using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public enum GameState
{
    Ready,
    Play,
    GameOver
}
public class KartInfomation : MonoBehaviour
{
    // 현재상태
    public GameState state = GameState.Ready;
    Rigidbody rb;
    //UI
    public Text SystemText;
    public Text ErrorText;
    // 시간
    public Text timeUI;
    float time;
    int min;
    int minisec;
    // 바퀴 수
    public Text lap;
    int nowlap = 1;
    // 속도 측정
    public Text KartSpeed;
    // 카트의 소리
    public AudioSource kartSound;

    // 체크포인트
    public GameObject[] checkPoint;
    bool[] checking;
    public GameObject goal;
    float bestLapTime = float.MaxValue;
    public GameObject goalImage;
    public Transform goalPos;

    // 이동방향 측정
    Vector3 lastPos;
    Vector3 currentPos;
    float PosTime;
    int xzPos;
    Vector3 beforePos;

    // 기록
    public GameObject RecordWindow;
    public Text myRecord;
    public Text BestRecord;

    public GameObject needle;
    private float startPosition = 220f, endPosition = -49f;
    private float desiredPosition;

    #region 기록 저장 기능 구현
    // 현재 플레이어의 이름과 기록을 저장
    Data bestdata = new Data();
    // bestdata의 기록들을 전부 저장 => 나중에 sort해서 빠른순서로 정렬하면 순위를 표현할 수 있다.
    Record rankRecord = new Record();
    // 시간순으로 정렬이 완료된 데이터 리스트
    List<Data> sortedData = new List<Data>();
    // 순위표의 이름
    public Text[] rankName;
    public Text[] rankTime;
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lap.text = $"LAPS : {nowlap} / {GameManager.instance.laps}";
        StartCoroutine(ReadyToPlay());
        checking = new bool[checkPoint.Length];
        lastPos = transform.position;
        RankLoad();
    }
    IEnumerator ReadyToPlay()
    {
        SystemText.text = "3";
        SoundManager.effectSound.clip = SoundManager.instance.startSound_01;
        SoundManager.effectSound.volume = 0.4f;
        SoundManager.effectSound.Play();
        yield return new WaitForSeconds(1f);
        SystemText.text = "2";
        SoundManager.effectSound.Play();
        yield return new WaitForSeconds(1f);
        SystemText.text = "1";
        SoundManager.effectSound.Play();
        yield return new WaitForSeconds(1f);
        SystemText.text = "GO!!";
        SoundManager.effectSound.clip = SoundManager.instance.startSound_02;
        SoundManager.effectSound.Play();
        SoundManager.effectSound.volume = 0.8f;
        state = GameState.Play;
        yield return new WaitForSeconds(1f);
        SystemText.gameObject.SetActive(false);
        timeUI.gameObject.SetActive(true);
    }
    void KartSound()
    {
        kartSound.volume = (rb.velocity.magnitude / GameManager.instance.maxSpeed) * 0.2f;
    }
    private void FixedUpdate()
    {
        // 아래부터는 play단계에서만 가능
        if (state != GameState.Play)
        {
            return;
        }
        KartSound();

        // 레이싱 시간
        time += Time.fixedDeltaTime;
        if (time >= 60)
        {
            min = (int)(time / 60);
            time %= 60;
        }
        minisec = (int)((time * 100) % 100);
        timeUI.text = $"{min:00}:{time:00}:{minisec:00}";

        
        if ((GameManager.instance.maxSpeed - rb.velocity.magnitude*2.6f) <= 1)
        {
            KartSpeed.text = $"{GameManager.instance.maxSpeed:000}";
        }
        else
        {
            KartSpeed.text = $"{rb.velocity.magnitude * 2.6f:000}";
        }

        // 카트 리셋
        KartReset();
        if (Input.GetKeyDown(KeyCode.G))
        {
            rb.velocity = Vector3.zero;
            transform.position = checkPoint[checking.Length - 1].transform.position;
            transform.rotation = checkPoint[checking.Length - 1].transform.rotation;
        }
        // 역주행 검사
        Reverse();
        UpdateNeedle();
    }

    public void UpdateNeedle()
    {
        desiredPosition = startPosition - endPosition;
        float temp = (rb.velocity.magnitude * 2.6f) / 180;
        needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition));
    }

    // 카트 위치 초기화
    void KartReset(bool reset = false)
    {
        if (Input.GetKeyDown(KeyCode.R) || reset == true)
        {
            if (checking[0] == false)
            {
                rb.velocity = Vector3.zero;
                transform.position = goal.transform.position;
                transform.rotation = goal.transform.rotation;
                Camera.main.gameObject.transform.position = transform.position;
                return;
            }
            else if (checking[checking.Length - 1] == true)
            {
                // 전부 참인 경우 -> 마지막 체크포인트를 지났으나 아직 골인은 못한 경우.
                rb.velocity = Vector3.zero;
                transform.position = checkPoint[checkPoint.Length - 1].transform.position;
                transform.rotation = checkPoint[checkPoint.Length - 1].transform.rotation;
                Camera.main.gameObject.transform.position = transform.position;
                return;
            }
            else
            {
                for (int i = 1; i < checking.Length; i++)
                {
                    if (checking[i] == false)
                    {
                        rb.velocity = Vector3.zero;
                        transform.position = checkPoint[i - 1].transform.position;
                        transform.rotation = checkPoint[i - 1].transform.rotation;
                        Camera.main.gameObject.transform.position = transform.position;
                        return;
                    }
                }
            }
        }
    }

    public void MyBestRecord(Text text, bool str = false)
    {
        if (bestLapTime >= 60)
        {
            min = (int)(bestLapTime / 60);
            bestLapTime %= 60;
        }
        minisec = (int)((bestLapTime * 100) % 100);
        if (str == true)
        {
            text.text = $"Best : {min:00} : {bestLapTime:00} : {minisec:00}";
        }
        else
        {
            text.text = $" {min:00} : {bestLapTime:00} : {minisec:00}";
        }
    }

    public void LobbyGo()
    {
        GameManager.nowPlayer.money += 50 * GameManager.instance.laps;
        GameManager.instance.Save(GameManager.nowPlayer, GameManager.slotNum.ToString());
        SceneManager.LoadScene("Main_02");
    }

    void SavePos()
    {
        beforePos = transform.position;
    }

    // 이동값 확인하기
    // x : 0, -x : 1, z : 2, -z: 3
    void Reverse()
    {
        currentPos = transform.position;
        switch (xzPos)
        {
            case 0: // x 가 증가해야함
                if (lastPos.x > currentPos.x)
                {
                    if (beforePos.x - currentPos.x > 2)
                    {
                        PosTime += Time.deltaTime;
                        if (PosTime > 2f)
                        {
                            ErrorText.gameObject.SetActive(true);
                            ErrorText.text = "역주행";
                        }
                        if (PosTime > 7f)
                        {
                            KartReset(true);
                            PosTime = 0;
                        }
                    }
                }
                else
                {
                    ErrorText.gameObject.SetActive(false);
                    PosTime = 0;
                }
                break;
            case 1: // x 가 감소해야함
                if (lastPos.x < currentPos.x)
                {
                    if (currentPos.x - beforePos.x > 2)
                    {
                        PosTime += Time.deltaTime;
                        if (PosTime > 2f)
                        {
                            ErrorText.gameObject.SetActive(true);
                            ErrorText.text = "역주행";
                        }
                        if (PosTime > 7f)
                        {
                            KartReset(true);
                        }
                    }
                }
                else
                {
                    ErrorText.gameObject.SetActive(false);
                    PosTime = 0;
                }
                break;
            case 2: // z 가 증가해야함
                if (lastPos.z > currentPos.z)
                {
                    if (beforePos.z - currentPos.z > 2f)
                    {
                        PosTime += Time.deltaTime;
                        ErrorText.gameObject.SetActive(true);
                        ErrorText.text = "역주행";
                        if (PosTime > 7f)
                        {
                            KartReset(true);
                            PosTime = 0;
                        }
                    }
                }
                else
                {
                    ErrorText.gameObject.SetActive(false);
                    PosTime = 0;
                }
                break;
            case 3: // z 가 감소해야함
                if (lastPos.z < currentPos.z)
                {
                    if (currentPos.z - beforePos.z > 2)
                    {
                        PosTime += Time.deltaTime;
                        if (PosTime > 2f)
                        {
                            ErrorText.gameObject.SetActive(true);
                            ErrorText.text = "역주행";
                        }
                        if (PosTime > 7f)
                        {
                            KartReset(true);
                            PosTime = 0;
                        }
                    }
                }
                else
                {
                    ErrorText.gameObject.SetActive(false);
                    PosTime = 0;
                }
                break;
            default:
                ErrorText.gameObject.SetActive(false);
                PosTime = 0;
                break;
        }
        lastPos = currentPos;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CheckPoint")
        {
            for (int i = 0; i < checkPoint.Length; i++)
            {
                if (checkPoint[i].gameObject == other.gameObject)
                {
                    checking[i] = true;
                    print($"{i}번째를 true로 변경!");
                }
            }
        }
        if (other.gameObject == goal.gameObject)
        {
            if (checking[checking.Length - 1])
            {
                if (nowlap < GameManager.instance.laps)
                {
                    nowlap++;
                    lap.text = $"LAPS : {nowlap} / {GameManager.instance.laps}";
                    SoundManager.effectSound.clip = SoundManager.instance.lapSound;
                    StartCoroutine(LapSetting(1f));
                    SoundManager.effectSound.Play();
                    if (time < bestLapTime)
                    {
                        bestLapTime = time;
                    }
                    MyBestRecord(BestRecord, true);
                    time = 0;
                }
                else if (nowlap == GameManager.instance.laps)
                {
                    if (time < bestLapTime)
                    {
                        bestLapTime = time;
                    }
                    state = GameState.GameOver;
                    print(state);
                    SoundManager.effectSound.clip = SoundManager.instance.goalSound;
                    SoundManager.effectSound.Play();
                    MyBestRecord(BestRecord, true);
                    BestRecordSaveAndSort();
                    // 1순위부터 3순위까지 넣을 Text 받아오고 거기에 데이터 넣기
                    ViewRanking();
                    StartCoroutine(MoveToPosition(goalImage.transform, goalPos.position, 0.4f));
                    MyBestRecord(myRecord);
                }
            }
            for (int i = 0; i < checking.Length; i++)
            {
                checking[i] = false;
            }
        }
        if (other.tag == "ReverseX")
        {
            print("X방향");
            xzPos = 0;
        }
        if (other.tag == "Reverse-X")
        {
            print("-X방향");
            xzPos = 1;
        }
        if (other.tag == "ReverseZ")
        {
            print("Z방향");
            xzPos = 2;
        }
        if (other.tag == "Reverse-Z")
        {
            print("-Z방향");
            xzPos = 3;
        }
    }
    public void RankLoad()
    {
        // 기존에 저장된 자료가 있다면 이를 불러와라
        if (File.Exists(GameManager.instance.recordPath))
        {
            string str = File.ReadAllText(GameManager.instance.recordPath);
            rankRecord = JsonUtility.FromJson<Record>(str);
        }
    }
    void BestRecordSaveAndSort()
    {
        // 현재 완주한 플레이어의 이름과 최단기록을 bestdata에 저장
        bestdata.name = GameManager.nowPlayer.name;
        bestdata.time = bestLapTime;
        // 게임씬이 시작될 때 이전에 저장되어있던 rankRecord를 불러왔음. => 여기에 방금 기록한 bestdata를 추가함.
        rankRecord.rank.Add(bestdata);

        // 기록된 자료들을 시간순으로 정렬한다.
        rankRecord.rank = rankRecord.rank.OrderBy(x => x.time).ToList();
        // 순위권 밖인 기록들은 제거한다.
        if (rankRecord.rank.Count > 3)
        {
            rankRecord.rank.RemoveRange(3, rankRecord.rank.Count - 3);
        }
        // 기록을 저장한다.
        string jdata = JsonUtility.ToJson(rankRecord);
        File.WriteAllText(GameManager.instance.recordPath, jdata);
    }
    void ViewRanking()
    {
        for (int i = 0; i < rankRecord.rank.Count; i++)
        {
            rankName[i].text = rankRecord.rank[i].name;

            if (rankRecord.rank[i].time >= 60)
            {
                min = (int)(rankRecord.rank[i].time / 60);
                rankRecord.rank[i].time %= 60;
            }
            minisec = (int)((rankRecord.rank[i].time * 100) % 100);
            rankTime[i].text = $" {min:00} : {rankRecord.rank[i].time:00} : {minisec:00}";
        }
    }

    IEnumerator LapSetting(float waitTime)
    {
        switch (nowlap)
        {
            case 2:
                SystemText.gameObject.SetActive(true);
                SystemText.fontSize = 100;
                SystemText.text = "두번째 바퀴";
                break;
            case 3:
                SystemText.gameObject.SetActive(true);
                SystemText.text = "마지막 바퀴";
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(waitTime);
        SystemText.gameObject.SetActive(false);
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
        yield return new WaitForSeconds(1.2f);
        //UI 꺼버리기.
        yield return new WaitForSeconds(1.2f);
        RecordWindow.SetActive(true);
    }
}
