using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource source;
    [SerializeField]
    AudioClip opening, bgm, poweredBgm;
    FirstTimeClass openingFt = new FirstTimeClass(), bgmFt = new FirstTimeClass();
    enum BGM {
        opening,
        bgm,
    };
    BGM bgmStatus;
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = opening;
        bgmStatus = BGM.opening;
    }
    bool poweredBgmFt = true;
    bool normalBgmFt = true;

    void Update()
    {
        switch(bgmStatus) {
            case BGM.bgm: {
                if(PointManager.hasPower) {
                    if(poweredBgmFt) {
                        source.Stop();
                        source.volume = 0.25f;
                        source.clip = poweredBgm;
                        source.Play();
                        poweredBgmFt = false;
                        normalBgmFt = true;
                    }
                } else {
                    if(normalBgmFt) {
                        source.Stop();
                        source.volume = 0.5f;
                        source.clip = bgm;
                        source.Play();
                        normalBgmFt = false;
                        poweredBgmFt = true;
                    }
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


