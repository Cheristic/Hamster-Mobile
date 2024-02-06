using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsHandler : MonoBehaviour
{
    public static OptionsHandler Main { get; private set; }

    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        }
        else
        {
            Main = this;
        }
    }

    public bool zoomedMode = false;
    public void SetZoomedMode(bool mode) { zoomedMode = mode; }

    public AudioMixer mixer;

    public void SetVolumeLevel(float vol)
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
