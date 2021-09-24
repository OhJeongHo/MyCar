using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidSoundDestory : MonoBehaviour
{
    private float destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyTime >= 1f)
        {
            Destroy(gameObject);
        }
        else
        {
            destroyTime += Time.deltaTime;
        }
    }
}
