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
    FirstTimeClass openingFt = new FirstTimeClass(), bgmFt = new FirstTimeClass();
    enum BGM {
        opening,
        bgm
    };
    BGM bgmStatus;
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = opening;
        bgmStatus = BGM.opening;
    }

    void Update()
    {
        switch(bgmStatus) {
            case BGM.bgm: {
                if(bgmFt) {
                    source.Play();
                }
                break;
            }
            case BGM.opening: {
                if (!(MapController.systemStatus == MapController.SystemStatus.WaitPlayOpening)) {
                    break;
                }
                if(source.isPlaying) {
                    break; /* 再生中なら帰る */
                }

                if (openingFt) {
                    source.Play();
                    break;
                }

                /* ゲームのステータスをすすめる */
                MapController.systemStatus = MapController.SystemStatus.WaitOtherPlayer;

                /* sourceをbgm用に変更 */
                source.Stop();
                source.clip = bgm;
                source.loop = true;
                bgmStatus = BGM.bgm;

                break;
            }
        }
    }
}


