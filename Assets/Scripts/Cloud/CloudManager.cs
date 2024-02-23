using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
using System;

public class CloudManager : MonoBehaviour
{
    public static CloudManager cloud { get; private set; }

    [Serializable]
    public enum Platform {
        Android,
        iOS,
        Itch
    }
    public Platform platform;

    private GPGSManager gpgs;
    
    private void Awake()
    {
        if (cloud != null && cloud != this)
        {
            Destroy(this);
        } else
        {
            cloud = this;
        }

        gpgs = GetComponentInChildren<GPGSManager>();
    }

    public void ShowLeaderboard()
    {
        switch (platform)
        {
            case Platform.Android:
                gpgs.ShowLeaderboard();
                break;
        }
    }

    public DailyHamsterdle.HamsterdleStatus HasCompletedDaily()
    {
        if (platform == Platform.Android)
        {
            return gpgs.HasCompletedDaily();
        }
        return DailyHamsterdle.HamsterdleStatus.CannotConnectToGoogle;
    }
}
