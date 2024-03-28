using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Mathematics;






#if UNITY_ANDROID

using GooglePlayGames;
using GooglePlayGames.BasicApi;

#endif


public class GPGSManager : PlatformManager
{
#if UNITY_ANDROID

    string AuthorizationToken;

    public override void Activate()
    {
        Debug.Log("Activating PlayGamesPlatform");
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    public override Task<bool> GetAuthenticationResults()
    {
        Debug.Log("GetAuthenticationResults() started");
        var tcs = new TaskCompletionSource<bool>();

        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {

                PlayGamesPlatform.Instance.LoadScores(
                    GPGSIds.leaderboard_hamster_high_score,
                    LeaderboardStart.PlayerCentered,
                    1,
                    LeaderboardCollection.Public,
                    LeaderboardTimeSpan.AllTime,
                    (LeaderboardScoreData data) =>
                    {
                        ScoreManager.Main.highscore = (int)data.PlayerScore.value;
                    }
                );

                ScoreManager.Main.highscoreEnabled = true; // Only update high score if connected to google
                connectedToPlatform = true;
            }
            else
            {
                Debug.Log("GPGSManager failed to Authenticate");
                connectedToPlatform = false;
                AuthorizationToken = null;
            }
            tcs.TrySetResult(connectedToPlatform);
            Debug.Log("Returning GetAuthenticationResults()");
        });

        Debug.Log("Done authenticating GPGS status: " + connectedToPlatform);
        return tcs.Task;
    }

    public override Task<bool> ManuallyAuthenticate()
    {
        Debug.Log("ManuallyAuthenticate() started");
        var tcs = new TaskCompletionSource<bool>();

        PlayGamesPlatform.Instance.ManuallyAuthenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    AuthorizationToken = code;
                });
                Debug.Log(AuthorizationToken + " " + Time.time);


                PlayGamesPlatform.Instance.LoadScores(
                    GPGSIds.leaderboard_hamster_high_score,
                    LeaderboardStart.PlayerCentered,
                    1,
                    LeaderboardCollection.Public,
                    LeaderboardTimeSpan.AllTime,
                    (LeaderboardScoreData data) =>
                    {
                        ScoreManager.Main.highscore = (int)data.PlayerScore.value;
                    }
                );

                ScoreManager.Main.highscoreEnabled = true; // Only update high score if connected to google
                connectedToPlatform = true;
            }
            else
            {
                Debug.Log("GPGSManager failed to Authenticate");
                connectedToPlatform = false;
                AuthorizationToken = null;
            }
            tcs.TrySetResult(connectedToPlatform);
            Debug.Log("Returning ManuallyAuthenticate()");
        });

        Debug.Log("Done authenticating GPGS status: " + connectedToPlatform);
        return tcs.Task;
    }

    public override async Task<bool> AuthenticateWithUnity()
    {
        Debug.Log("Attempting to sign into Unity Services with Google Play");
        if (!await GetAuthorizationToken()) return false;
        return await CloudServicesManager.cloud.unityServices.SignInWithGooglePlayGamesAsync(AuthorizationToken);
    }

    private Task<bool> GetAuthorizationToken()
    {
        var tcs = new TaskCompletionSource<bool>();
        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
        {
            AuthorizationToken = code;
            Debug.Log("Got authorization code " + code);
            if (code == null) tcs.TrySetResult(false);
            else tcs.TrySetResult(true);

        });

        return tcs.Task;
    }

    public override bool IsConnected()
    {
        return connectedToPlatform;
    }


    public override void AddScore()
    {     
        if (connectedToPlatform)
        {
            if (GameModeSelector.Main.hamsterdleSelected)
            {
                Social.ReportScore(ScoreManager.Main.score, GPGSIds.leaderboard_daily_hamsterdle, UpdateLeaderboard);
            } else
            {
                Social.ReportScore(ScoreManager.Main.score, GPGSIds.leaderboard_hamster_high_score, UpdateLeaderboard);
            }
        }
    }

    private void UpdateLeaderboard(bool success)
    {
        if (success) Debug.Log("Successfully updated leaderboard.");
        else Debug.Log("Failed to update leadeboard.");
    }

    // Called by CloudManager
    public override void ShowLeaderboard()
    {
        if (connectedToPlatform)
        {
            Social.ShowLeaderboardUI();
        }
    }

#endif
}
