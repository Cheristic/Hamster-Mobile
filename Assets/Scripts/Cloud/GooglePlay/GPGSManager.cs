using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;

public class GPGSManager : MonoBehaviour
{
    private PlayGamesLocalUser clientConfigurations;
    public Text statusText;
    public Text descriptionText;
    bool connectedToGooglePlay;
    void Awake()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        GameManager.gameEnd += OnGameEnd;
    }

    private void Start()
    {
        LoginToGooglePlay();
    }
    private void LoginToGooglePlay()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    private void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            connectedToGooglePlay = true;
        }
        else connectedToGooglePlay = false;
    }

    private void OnGameEnd()
    {
        if (connectedToGooglePlay)
        {
            Social.ReportScore(ScoreManager.Main.score, GPGSIds.leaderboard_hamster_high_score, UpdateLeaderboard);
            Social.ShowLeaderboardUI();
        }
    }

    private void UpdateLeaderboard(bool success)
    {
        if (success) Debug.Log("Successfully updated leaderboard.");
        else Debug.Log("Failed to update leadeboard.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
