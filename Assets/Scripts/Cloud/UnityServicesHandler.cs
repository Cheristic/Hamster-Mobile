using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using System.Collections.Generic;
using GooglePlayGames;
using System;
public class UnityServicesHandler : MonoBehaviour
{
    internal const string hamsterHighScoreLeaderboardId = "CgkI-sPe6cQSEAIQBB";
    internal const string dailyHamsterdleLeaderboardId = "CgkI-sPe6cQSEAIQAg";

    internal bool connectedToUnityAuthentication;

    public async Task Activate()
    {
        try
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                var options = new InitializationOptions();

                Debug.Log("Attempting to Activate UnityServiceshandler");
                await UnityServices.InitializeAsync(options);
                Debug.Log("UnityServices are " + UnityServices.State.ToString());
            }
        } catch(Exception ex)
        {
            Debug.Log("UnityServices failed with " + ex);
        }

    }

    public bool IsConnected()
    {
        return UnityServices.State == ServicesInitializationState.Initialized && 
            connectedToUnityAuthentication;
    }


    public async Task<bool> SignInWithGooglePlayGamesAsync(string AuthorizationToken)
    {
        if (UnityServices.State != ServicesInitializationState.Initialized || AuthorizationToken == null )
        {
            Debug.Log("UnityServicesHandler Line 31 " + Time.time + " Token: " + AuthorizationToken);
            return false;
        }
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(AuthorizationToken);
            connectedToUnityAuthentication = true;
            GameManager.gameEnd += AddScoreToHighScore;
        }
        catch (AuthenticationException ex)
        {
            connectedToUnityAuthentication = false;
            Debug.Log("Failed to sign into google play with unity " + ex);
        }
        catch (RequestFailedException ex)
        {
            connectedToUnityAuthentication = false;
            Debug.Log("Failed to sign into google play with unity " + ex);
        }
        return connectedToUnityAuthentication;
    }

    public async Task<bool> SignInWithAppleGameCenterAsync(string signature, string teamPlayerId, string publicKeyURL, string salt, ulong timestamp)
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            return false;
        }

        try
        {
            await AuthenticationService.Instance.SignInWithAppleGameCenterAsync(signature, teamPlayerId, publicKeyURL, salt, timestamp);
            connectedToUnityAuthentication = true;
            GameManager.gameEnd += AddScoreToHighScore;
        }
        catch (AuthenticationException ex)
        {
            connectedToUnityAuthentication = false;
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            connectedToUnityAuthentication = false;
            Debug.LogException(ex);
        }
        return connectedToUnityAuthentication;
    }

    public async void AddScoreToHighScore()
    {
        if (connectedToUnityAuthentication)
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(hamsterHighScoreLeaderboardId, ScoreManager.Main.score);
        }
    }

    public async Task<LeaderboardEntry> GetHamsterdleScore()
    {
        if (connectedToUnityAuthentication)
        {
            var response = await LeaderboardsService.Instance.GetPlayerScoreAsync(dailyHamsterdleLeaderboardId,
                new GetPlayerScoreOptions { IncludeMetadata = true });
            return response;
        }
        return null;


    }

    public async void AddScoreToHamsterdleBoard(int attempts)
    {
        if (connectedToUnityAuthentication)
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(hamsterHighScoreLeaderboardId, ScoreManager.Main.score,
                new AddPlayerScoreOptions { Metadata = attempts});
        }
    }
}