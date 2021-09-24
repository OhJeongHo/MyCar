using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public static AudioSource effectSound;
    public AudioClip crush, resetSound, lapSound, goalSound, startSound_01, startSound_02, upgradeSound, skidSound;
    public bool effect = true;
    public bool bgm = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        effectSound = GetComponent<AudioSource>();
    }
}
