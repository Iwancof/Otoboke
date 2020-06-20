using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource source;
    [SerializeField]
    AudioClip opening = default, bgm = default, poweredBgm = default, backNest = default;
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
    bool returningBgmFt = true;

    void Update()
    {
        switch(bgmStatus) {
            case BGM.bgm: {
                if(PointManager.returningNest) { 
                    if(returningBgmFt) {
                        source.Stop();
                        source.clip = backNest;
                        source.Play();
                        returningBgmFt = false;
                        poweredBgmFt = true;
                        normalBgmFt = true;
                    }
                } else if(PointManager.hasPower) {
                    if(poweredBgmFt) {
                        source.Stop();
                        source.volume = 0.25f;
                        source.clip = poweredBgm;
                        source.Play();
                        poweredBgmFt = false;
                        normalBgmFt = true;
                        returningBgmFt = true;
                    }
                } else {
                    if(normalBgmFt) {
                        source.Stop();
                        source.volume = 0.5f;
                        source.clip = bgm;
                        source.Play();
                        normalBgmFt = false;
                        poweredBgmFt = true;
                        returningBgmFt = true;
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


