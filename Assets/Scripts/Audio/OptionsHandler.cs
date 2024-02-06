using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsHandler : MonoBehaviour
{
    public AudioMixer mixer;
    public void SetLevel(float vol)
    {
        if (vol == .001f) // Minimum = mute
        {
            mixer.SetFloat("Master Vol", -80f);
        }
        else
        {
            mixer.SetFloat("Master Vol", Mathf.Log10(vol) * 20);
        }
    }
}
