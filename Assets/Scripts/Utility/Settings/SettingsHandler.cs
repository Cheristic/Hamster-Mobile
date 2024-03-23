using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsHandler : MonoBehaviour
{
    public static SettingsHandler Main { get; private set; }

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
        Debug.Log("loaded");
    }

    internal bool zoomedMode = false;
    public void SetZoomedMode(bool mode) { zoomedMode = mode; }

    public AudioMixer mixer;
    public GameObject FastFallZone;

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

    public void EnableFastFallZone(bool enable)
    {
        if (enable)
        {
            InputManager.Main.EnableFastFallZone();
            FastFallZone.SetActive(true);
        }
        else
        {
            InputManager.Main.DisableFastFallZone();
            FastFallZone.SetActive(false);
        }
    }
}
