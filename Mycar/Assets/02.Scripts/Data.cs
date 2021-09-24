using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public string name;
    public float time;
}

[System.Serializable]
public class Record
{
    public List<Data> rank = new List<Data>();
}
