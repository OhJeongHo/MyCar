using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string name;
    public int money = 100;
    public string time = System.DateTime.Now.ToString();
    public bool kart2, kart3;
    public int kartstate = 0;
    // 각 카트별 업그레이드 수치값 저장
    public int kart1_maxSpeed, kart1_axel, kart1_boost;
    public int kart2_maxSpeed, kart2_axel, kart2_boost;
    public int kart3_maxSpeed, kart3_axel, kart3_boost;
}



