using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    enum driveType
    {
        FRONTDRIVE,
        REARDRIVE,
        ALLDRIVE
    }

    [SerializeField]driveType drive;

    public GameObject[] Kart;
    InputManager IM;
    KartInfomation info;

    public TrailRenderer[] skidMark;

    public WheelCollider[] wheels = new WheelCollider[4];
    GameObject[] wheelMesh = new GameObject[4];
    GameObject head;
    GameObject boost;
    public float power = 40000;
    public float rot = 45f;
    Rigidbody rb;
    public float downForceValue = 50f;
    public float radius = 6;

    WheelFrictionCurve fFrictionBL;
    WheelFrictionCurve sFrictionBL;
    WheelFrictionCurve fFrictionBR;
    WheelFrictionCurve sFrictionBR;
    float slipRate = 1.0f;
    float handBreakSlipRate = 0.01f;

    // 부스터
    public Slider driftGauge;    // 게이지 표현
    bool onBoosting;            // 현재 부스터를 사용하고 있는 중인지
    float driftOnTime;       // 드리프트를 지속하고 있는 시간
    float shortBoostCheck;      // 순간부스터를 사용할수 있는 시간인지
    float shortBoostlimitTime;      // 순간 부스터를 사용할 수 있는 시간 제한
    bool shortpossible;
    int boostCnt = 0;       // 부스터 사용 가능 개수
    float boostEffectTime;  // 부스터를 지속하는 시간
    public GameObject[] boostIcon;  //  부스터 아이콘
    bool startBoostCheck = true;
    float startBoost;
    float originMaxSpeed;


    void Start()
    {
        // 선택된 차량만 활성화
        KartActive(GameManager.nowPlayer.kartstate);
        IM = GetComponent<InputManager>();
        info = GetComponent<KartInfomation>();
        rb = GetComponent<Rigidbody>();
        boost = GameObject.FindGameObjectWithTag("Boost");
        boost.SetActive(false);
        rb.centerOfMass = new Vector3(0, -1, 0);
        KartValueSetting();
        wheelMesh = GameObject.FindGameObjectsWithTag("WheelMesh");
        head = GameObject.FindGameObjectWithTag("Head");
        originMaxSpeed = GameManager.instance.maxSpeed;
        for (int i = 0; i < wheelMesh.Length; i++)
        {
            wheels[i].transform.position = wheelMesh[i].transform.position;
        }
        fFrictionBL = wheels[2].forwardFriction;
        sFrictionBL = wheels[2].sidewaysFriction;
        fFrictionBR = wheels[3].forwardFriction;
        sFrictionBR = wheels[3].sidewaysFriction;
    }
    void KartValueSetting()
    {
        switch (GameManager.nowPlayer.kartstate)
        {
            case 0:
                GameManager.instance.maxSpeed = 120;
                GameManager.instance.maxSpeed += GameManager.nowPlayer.kart1_maxSpeed * 5;
                power += GameManager.nowPlayer.kart1_axel * 1000;
                break;
            case 1:
                GameManager.instance.maxSpeed = 130;
                GameManager.instance.maxSpeed += GameManager.nowPlayer.kart2_maxSpeed * 5;
                power += GameManager.nowPlayer.kart2_axel * 1000;
                break;
            case 2:
                GameManager.instance.maxSpeed = 140;
                GameManager.instance.maxSpeed += GameManager.nowPlayer.kart3_maxSpeed * 5;
                power += GameManager.nowPlayer.kart3_axel * 1000;
                break;
        }
    }
    private void Update()
    {
        if (info.state == GameState.GameOver)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = int.MaxValue;
                wheels[i].motorTorque = 0;
            }
            return;
        }
        
        SteerVehicle();
        WheelPosAndAni();
        if (info.state != GameState.Play)
        {
            return;
        }

        if (startBoostCheck == true)
        {
            startBoost += Time.deltaTime;
            if (startBoost <= 1)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartCoroutine(MoveToRotation(head.transform, Quaternion.Euler(0, 0, 15), 0.1f));
                    StartCoroutine(Boost(1.5f));
                    print("시작부스터가 인식되었따");
                    //StartCoroutine("ShortBoost");
                    onBoosting = true;
                    startBoostCheck = false;
                    //shortpossible = true;
                }
            }
        }

        DriftGauge();
        SkidAndSound();
        AddDownForce();
        KartMove();
        Drift();
    }
    void SkidAndSound()
    {
        if (IM.handbrake)
        {
            print("드리프트 인식");
            if (SoundManager.effectSound.isPlaying == false)
            {
                print("드리프트사운드넣는중1");
                SoundManager.effectSound.clip = SoundManager.instance.skidSound;
                SoundManager.effectSound.Play();
            }
            for (int i = 0; i < skidMark.Length; i++)
            {
                skidMark[i].emitting = true;
            }
        }
        else if (!IM.handbrake)
        {
            if (SoundManager.effectSound.clip == SoundManager.instance.skidSound && SoundManager.effectSound.isPlaying)
            {
                SoundManager.effectSound.Stop();
            }
            for (int i = 0; i < skidMark.Length; i++)
            {
                skidMark[i].emitting = false;
            }
        }
    }
    void DriftGauge()
    {
        driftOnTime += Time.deltaTime * 1/200;
        driftGauge.value = driftOnTime;
        BoosterCntCheck();
    }
    void BoosterCntCheck()
    {
        if (driftOnTime >= 1)
        {
            driftOnTime = 0;
            driftGauge.value = 0;
            if (boostCnt <= 1)
            {
                boostCnt++;
                for (int i = 0; i < boostCnt; i++)
                {
                    boostIcon[i].SetActive(true);
                    // 생성된 부스터가 깜빡이는 효과 및 사운드
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && onBoosting == false)
        {
            // 부스터 효과 발동
            if (boostCnt > 0)
            {
                for (int i = 0; i < boostIcon.Length; i++)
                {
                    boostIcon[i].SetActive(false);
                }
                for (int i = 0; i < boostCnt-1; i++)
                {
                    boostIcon[i].SetActive(true);
                }
                StartCoroutine(MoveToRotation(head.transform, Quaternion.Euler(0, 0, 15), 0.1f));
                onBoosting = true;
                StartCoroutine(Boost(3f));
            }
            boostCnt--;
        }
    }
    // 선택된 카트 활성화
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

    void AddDownForce()
    {
        rb.AddForce(-transform.up * downForceValue * rb.velocity.magnitude);
    }
    void KartMove()
    {
        if (IM.handbrake)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = int.MaxValue;
                wheels[i].motorTorque = 0;
            }
            return;
        }
        if (drive == driveType.ALLDRIVE)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                if (rb.velocity.magnitude * 2.6f > GameManager.instance.maxSpeed)
                {
                    wheels[i].brakeTorque = int.MaxValue;
                    wheels[i].motorTorque = 0;
                    print("속도 제한!");
                }
                else
                {
                    wheels[i].brakeTorque = 0;
                    wheels[i].motorTorque = IM.vertical * (power/4);
                    print("주행중!");
                }
            }
            if (IM.vertical == 0)
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].brakeTorque = int.MaxValue;
                    wheels[i].motorTorque = 0;
                }
            }
            else
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    if (rb.velocity.magnitude * 2.6f < GameManager.instance.maxSpeed)
                    {
                        wheels[i].brakeTorque = 0;
                    }
                }
            }
        }
        else if (drive == driveType.REARDRIVE)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                if (rb.velocity.magnitude * 2.6f > GameManager.instance.maxSpeed)
                {
                    wheels[i].brakeTorque = int.MaxValue;
                    wheels[i].motorTorque = 0;
                }
                else
                {
                    wheels[i].brakeTorque = 0;
                    wheels[i].motorTorque = IM.vertical * (power / 2);
                }
            }
            if (IM.vertical == 0)
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].brakeTorque = power;
                    wheels[i].motorTorque = 0;
                }
            }
            else
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    if (rb.velocity.magnitude * 2.6f < GameManager.instance.maxSpeed)
                    {
                        wheels[i].brakeTorque = 0;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                if (rb.velocity.magnitude * 2.6f > GameManager.instance.maxSpeed)
                {
                    wheels[i].brakeTorque = int.MaxValue;
                }
                else
                {
                    wheels[i].brakeTorque = 0;
                    wheels[i].motorTorque = IM.vertical * (power / 2);
                }
            }
            if (IM.vertical == 0)
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].brakeTorque = power;
                }
            }
            else
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].brakeTorque = 0;
                }
            }
        }
    }

    void Drift()
    {
        if (IM.handbrake)
        {
            shortBoostCheck = 0;
            driftOnTime += Time.deltaTime*1/5;

            fFrictionBL.stiffness = handBreakSlipRate;
            wheels[2].forwardFriction = fFrictionBL;

            sFrictionBL.stiffness = handBreakSlipRate;
            wheels[2].sidewaysFriction = sFrictionBL;

            fFrictionBR.stiffness = handBreakSlipRate;
            wheels[3].forwardFriction = fFrictionBR;

            sFrictionBR.stiffness = handBreakSlipRate;
            wheels[3].sidewaysFriction = sFrictionBR;
        }
        else
        {
            shortBoostCheck += Time.deltaTime;
            fFrictionBL.stiffness = slipRate;
            wheels[2].forwardFriction = fFrictionBL;

            sFrictionBL.stiffness = slipRate;
            wheels[2].sidewaysFriction = sFrictionBL;

            fFrictionBR.stiffness = slipRate;
            wheels[3].forwardFriction = fFrictionBR;

            sFrictionBR.stiffness = slipRate;
            wheels[3].sidewaysFriction = sFrictionBR;
        }
        // 순간부스터
        if (Input.GetKeyDown(KeyCode.Space) && onBoosting == false && shortBoostCheck <= 1f && startBoostCheck == false)
        {
            print("tnsrksdasdlfqoiwdjfqoawdfasdf");
            onBoosting = true;
            StartCoroutine(MoveToRotation(head.transform, Quaternion.Euler(0, 0, 15), 0.1f));
            StartCoroutine(Boost(0.5f));
        }
        
    }
    void SteerVehicle()
    {
        // 애커만 조향
        //steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        if (IM.horizontal > 0)
        {   // rear tracks size is set to 1.5f          wheel base has been set to 2.55f
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
        }
        else if (IM.horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
            // transform.Rotate(Vector3.up * steerHelping)
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }
    }

    void WheelPosAndAni()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }

    IEnumerator Boost(float time)
    {
        boost.SetActive(true);
        float count = 0;
        while (count < time)
        {
            yield return new WaitForSeconds(0.1f);
            rb.AddForce(transform.forward * 40000);
            print("부스터 가속!");
            print(count);
            count += 0.1f;
        }
        Invoke("BoostReset", time);
        yield return null;
    }
    //void ShortBoost(float boostSetTime)
    //{
    //    boost.SetActive(true);
    //    float t = 0f;
    //    float s = 0f;
    //    while (t < 4)
    //    {
    //        t += Time.deltaTime;
    //        s += Time.deltaTime;
    //        if (s > 1f)
    //        {
    //            rb.AddForce(transform.forward * 10000);
    //            s = 0;
    //        }
    //    }
    //    Invoke("BoostReset", boostSetTime);
    //}

    //void Boost()
    //{
    //    boost.SetActive(true);
    //    float t = 0f;
    //    float s = 0f;
    //    while (t < 4)
    //    {
    //        t += Time.deltaTime;
    //        s += Time.deltaTime;
    //        if (s > 1f)
    //        {
    //            rb.AddForce(transform.forward * 10000);
    //            s = 0;
    //        }
    //    }
    //    Invoke("BoostReset", 4f);
    //}
    void BoostReset()
    {
        boost.SetActive(false);
        onBoosting = false;
        StartCoroutine(MoveToRotation(head.transform, Quaternion.Euler(0, 0, -15), 0.1f));
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
            transform.localRotation = Quaternion.Lerp(currentRot, currentRot * rot, t);
            yield return null;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obs")
        {
            if (SoundManager.effectSound.isPlaying == false)
            {
                SoundManager.effectSound.clip = SoundManager.instance.crush;
                SoundManager.effectSound.Play();
            }
        }
    }
}