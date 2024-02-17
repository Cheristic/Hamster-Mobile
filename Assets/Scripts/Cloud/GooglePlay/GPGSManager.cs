using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;

public class GPGSManager : MonoBehaviour
{
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
            
            PlayGamesPlatform.Instance.LoadScores(
                GPGSIds.leaderboard_hamster_high_score,
                LeaderboardStart.PlayerCentered,
                1,
                LeaderboardCollection.Public,
                LeaderboardTimeSpan.AllTime,
                (LeaderboardScoreData data) =>
                {
                    ScoreManager.Main.highscore = (int)data.PlayerScore.value;
                });
            ScoreManager.Main.highscoreEnabled = true;
        }
        else connectedToGooglePlay = false;
    }

    private void OnGameEnd()
    {     
        if (connectedToGooglePlay)
        {
            Social.ReportScore(ScoreManager.Main.score, GPGSIds.leaderboard_hamster_high_score, UpdateLeaderboard);
        }
    }

    private void UpdateLeaderboard(bool success)
    {
        if (success) Debug.Log("Successfully updated leaderboard.");
        else Debug.Log("Failed to update leadeboard.");
    }

    // Called by CloudManager
    public void ShowLeaderboard()
    {
        if (connectedToGooglePlay)
        {
            Social.ShowLeaderboardUI();
        }
    }

}
