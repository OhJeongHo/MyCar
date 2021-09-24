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
    // �������
    public GameState state = GameState.Ready;
    Rigidbody rb;
    //UI
    public Text SystemText;
    public Text ErrorText;
    // �ð�
    public Text timeUI;
    float time;
    int min;
    int minisec;
    // ���� ��
    public Text lap;
    int nowlap = 1;
    // �ӵ� ����
    public Text KartSpeed;
    // īƮ�� �Ҹ�
    public AudioSource kartSound;

    // üũ����Ʈ
    public GameObject[] checkPoint;
    bool[] checking;
    public GameObject goal;
    float bestLapTime = float.MaxValue;
    public GameObject goalImage;
    public Transform goalPos;

    // �̵����� ����
    Vector3 lastPos;
    Vector3 currentPos;
    float PosTime;
    int xzPos;
    Vector3 beforePos;

    // ���
    public GameObject RecordWindow;
    public Text myRecord;
    public Text BestRecord;

    public GameObject needle;
    private float startPosition = 220f, endPosition = -49f;
    private float desiredPosition;

    #region ��� ���� ��� ����
    // ���� �÷��̾��� �̸��� ����� ����
    Data bestdata = new Data();
    // bestdata�� ��ϵ��� ���� ���� => ���߿� sort�ؼ� ���������� �����ϸ� ������ ǥ���� �� �ִ�.
    Record rankRecord = new Record();
    // �ð������� ������ �Ϸ�� ������ ����Ʈ
    List<Data> sortedData = new List<Data>();
    // ����ǥ�� �̸�
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
        // �Ʒ����ʹ� play�ܰ迡���� ����
        if (state != GameState.Play)
        {
            return;
        }
        KartSound();

        // ���̽� �ð�
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

        // īƮ ����
        KartReset();
        if (Input.GetKeyDown(KeyCode.G))
        {
            rb.velocity = Vector3.zero;
            transform.position = checkPoint[checking.Length - 1].transform.position;
            transform.rotation = checkPoint[checking.Length - 1].transform.rotation;
        }
        // ������ �˻�
        Reverse();
        UpdateNeedle();
    }

    public void UpdateNeedle()
    {
        desiredPosition = startPosition - endPosition;
        float temp = (rb.velocity.magnitude * 2.6f) / 180;
        needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition));
    }

    // īƮ ��ġ �ʱ�ȭ
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
                // ���� ���� ��� -> ������ üũ����Ʈ�� �������� ���� ������ ���� ���.
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

    // �̵��� Ȯ���ϱ�
    // x : 0, -x : 1, z : 2, -z: 3
    void Reverse()
    {
        currentPos = transform.position;
        switch (xzPos)
        {
            case 0: // x �� �����ؾ���
                if (lastPos.x > currentPos.x)
                {
                    if (beforePos.x - currentPos.x > 2)
                    {
                        PosTime += Time.deltaTime;
                        if (PosTime > 2f)
                        {
                            ErrorText.gameObject.SetActive(true);
                            ErrorText.text = "������";
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
            case 1: // x �� �����ؾ���
                if (lastPos.x < currentPos.x)
                {
                    if (currentPos.x - beforePos.x > 2)
                    {
                        PosTime += Time.deltaTime;
                        if (PosTime > 2f)
                        {
                            ErrorText.gameObject.SetActive(true);
                            ErrorText.text = "������";
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
            case 2: // z �� �����ؾ���
                if (lastPos.z > currentPos.z)
                {
                    if (beforePos.z - currentPos.z > 2f)
                    {
                        PosTime += Time.deltaTime;
                        ErrorText.gameObject.SetActive(true);
                        ErrorText.text = "������";
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
            case 3: // z �� �����ؾ���
                if (lastPos.z < currentPos.z)
                {
                    if (currentPos.z - beforePos.z > 2)
                    {
                        PosTime += Time.deltaTime;
                        if (PosTime > 2f)
                        {
                            ErrorText.gameObject.SetActive(true);
                            ErrorText.text = "������";
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
                    print($"{i}��°�� true�� ����!");
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
                    // 1�������� 3�������� ���� Text �޾ƿ��� �ű⿡ ������ �ֱ�
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
            print("X����");
            xzPos = 0;
        }
        if (other.tag == "Reverse-X")
        {
            print("-X����");
            xzPos = 1;
        }
        if (other.tag == "ReverseZ")
        {
            print("Z����");
            xzPos = 2;
        }
        if (other.tag == "Reverse-Z")
        {
            print("-Z����");
            xzPos = 3;
        }
    }
    public void RankLoad()
    {
        // ������ ����� �ڷᰡ �ִٸ� �̸� �ҷ��Ͷ�
        if (File.Exists(GameManager.instance.recordPath))
        {
            string str = File.ReadAllText(GameManager.instance.recordPath);
            rankRecord = JsonUtility.FromJson<Record>(str);
        }
    }
    void BestRecordSaveAndSort()
    {
        // ���� ������ �÷��̾��� �̸��� �ִܱ���� bestdata�� ����
        bestdata.name = GameManager.nowPlayer.name;
        bestdata.time = bestLapTime;
        // ���Ӿ��� ���۵� �� ������ ����Ǿ��ִ� rankRecord�� �ҷ�����. => ���⿡ ��� ����� bestdata�� �߰���.
        rankRecord.rank.Add(bestdata);

        // ��ϵ� �ڷ���� �ð������� �����Ѵ�.
        rankRecord.rank = rankRecord.rank.OrderBy(x => x.time).ToList();
        // ������ ���� ��ϵ��� �����Ѵ�.
        if (rankRecord.rank.Count > 3)
        {
            rankRecord.rank.RemoveRange(3, rankRecord.rank.Count - 3);
        }
        // ����� �����Ѵ�.
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
                SystemText.text = "�ι�° ����";
                break;
            case 3:
                SystemText.gameObject.SetActive(true);
                SystemText.text = "������ ����";
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
        //UI ��������.
        yield return new WaitForSeconds(1.2f);
        RecordWindow.SetActive(true);
    }
}
