using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource source;
    [SerializeField]
    AudioClip opening;
    [SerializeField]
    AudioClip bgm;
    bool firsttime;
    enum BGM {
        opening,
        bgm
    };
    BGM bgmStatus;
    void Start()
    {
        source = GetComponent<AudioSource>();
        firsttime = true;
        Time.timeScale = 0f;
        source.clip = opening;
        bgmStatus = BGM.opening;
    }

    void Update()
    {
        switch(bgmStatus) {
            case BGM.bgm: {
                if(firsttime) {
                    source.Play();
                    firsttime = false;
                }
                break;
            }
            case BGM.opening: {
                if(Map.finishedDrawing) {
                    if(firsttime) {
                        source.Play();
                        firsttime = false;
                    } else if(!source.isPlaying) {
                        Time.timeScale = 1f;
                        source.Stop();
                        source.loop = true;
                        source.clip = bgm;
                        bgmStatus = BGM.bgm;
                        firsttime = true;
                    }
                }
                break;
            }
        }
    }
}
