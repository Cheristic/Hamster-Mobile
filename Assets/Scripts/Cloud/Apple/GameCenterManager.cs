using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using System;

#if UNITY_IOS 
using Apple.Core;
using Apple.GameKit;
#endif

public class GameCenterManager : PlatformManager
{
    internal const string hamsterHighScoreLeaderboardId = "CgklsPe6cQSEAIQAA";
#if UNITY_IOS
    bool connectedToGameCenter;
    string Signature;
    string TeamPlayerID;
    string Salt;
    string PublicKeyUrl;
    ulong Timestamp;


    public async Task LoginToGameCenter()
    {
        if (!GKLocalPlayer.Local.IsAuthenticated)
        {
            try
            {
                var player = GKLocalPlayer.Authenticate();
                var localPlayer = GKLocalPlayer.Local;

                var fetchItemsResponse = await localPlayer.FetchItems();

                Signature = Convert.ToBase64String(fetchItemsResponse.GetSignature());
                TeamPlayerID = localPlayer.TeamPlayerId;
                Salt = Convert.ToBase64String(fetchItemsResponse.GetSalt());
                PublicKeyUrl = fetchItemsResponse.PublicKeyUrl;
                Timestamp = fetchItemsResponse.Timestamp;

                connectedToGameCenter = true;

                await CloudServicesManager.cloud.unityServices.SignInWithAppleGameCenterAsync(Signature, TeamPlayerID, PublicKeyUrl, Salt, Timestamp);

            } 
            catch (Exception e)
            {
                Debug.Log("Could not authenticate iPhone - " + e);
            }
        }
    }

    public override async Task<DailyHamsterdle.HamsterdleStatus> HasCompletedDaily()
    {
        return DailyHamsterdle.HamsterdleStatus.HasCompletedHamsterdle;
    }

    public void UploadScore()
    {
        if (connectedToGameCenter)
        {
            Social.ReportScore(ScoreManager.Main.score, hamsterHighScoreLeaderboardId, (success) =>
            {
                if (success) Debug.Log("Successfully uploaded score to Game Center");
                else Debug.Log("Failed to upload score to Game Center");
            });
        }
    }
    public override void ShowLeaderboard() 
    { 
        if (connectedToGameCenter)
        {
            Social.ShowLeaderboardUI();
        }
    } 
#endif

}
