using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicBox : MonoBehaviour
{
    [SerializeField]
    SerializeDictionary<string, AudioClip> mAudioClip;

    private string mPlayMusicName = null;


    static MusicBox instance = null;
    public static MusicBox Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // 유일하게 존재할 수 있도록 에러 처리
        if (instance != null)
        {
            Debug.LogError("SystemManager is initialized twice!");
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Scene 이동간에 사라지지 않도록 처리
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 어떤 입력이 들어올 때 해당하는 소리를 들려줘라
        if (mPlayMusicName != null)
        {
            PlayMusic();
        }
        
    }

    private void PlayMusic()
    {
        AudioSource audio = GetComponent<AudioSource>();

        if (!mAudioClip.ContainsKey(mPlayMusicName))
        {
            Debug.Log("none key");
            mPlayMusicName = null;
            return;
        }
        audio.clip = mAudioClip[mPlayMusicName];
        audio.Play();

        mPlayMusicName = null;
    }

    public void InputPlayMusicName(string musicName)
    {
        if (musicName != null)
        {
            mPlayMusicName = musicName;
        }

        return;
    }



}
