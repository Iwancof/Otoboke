using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    AudioMixer mixer = default;

    public void SetBGMVolume(float vol) {
        mixer.SetFloat("BGMVolume", vol);
    }
    public void SetSEVolume(float vol) {
        mixer.SetFloat("SEVolume", vol);
    }
    public void SetMasterVolume(float vol) {
        mixer.SetFloat("MasterVolume", vol);
    }
}
