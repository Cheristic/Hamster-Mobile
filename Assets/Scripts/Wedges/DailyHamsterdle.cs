using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
public class DailyHamsterdle : MonoBehaviour
{
    List<SeedData> DailyHamsterdleData;
    DateTime launchDate;

    public enum HamsterdleStatus
    {
        CannotConnectToGoogle,
        HasNotCompletedHamsterdle,
        HasCompletedHamsterdle
    }
    private void Start()
    {
        string s = Resources.Load<TextAsset>("Hamsterdle/combined").text;
        DailyHamsterdleData = JsonConvert.DeserializeObject<List<SeedData>>(s);
        launchDate = new(2024, 2, 20);
    }

    struct ServerDateTime
    {
        public string datetime;
    }
    internal SeedData dailySeedData;
    public async Task<SeedData> RetrieveDailySeed()
    {
        // https://www.youtube.com/watch?v=OJhFlWRMGJg
        /* Call APIs to retrieve current world time in UTC-7 (Leaderboard reset time)
            If fail -> Return error "Not connected to Internet or API servers are down"
            If success -> continue */

        UnityWebRequest timeRequest = UnityWebRequest.Get("https://worldtimeapi.org/api/timezone/etc/GMT+7");
        timeRequest.SendWebRequest();
        while (!timeRequest.isDone) { await Task.Yield(); }

        if (timeRequest.result != UnityWebRequest.Result.Success)
        {
            // ADD OTHER APIs
            Debug.LogError("Failed to connect to world time API");
            return null;
        }

        ServerDateTime serverDateTime = JsonConvert.DeserializeObject<ServerDateTime>(timeRequest.downloadHandler.text);
        string date = Regex.Match(serverDateTime.datetime, @"^\d{4}-\d{2}-\d{2}").Value;
        DateTime currDate = DateTime.Parse(string.Format("{0}", date));
        int HamsterdleNumber = (currDate - launchDate).Days;

        /*
        Call APIs to retrieve current world time in UTC-7 (Leaderboard reset time)
            If fail -> Return error "Not connected to Internet or API servers are down"
            If success -> continue
        LINQ query the dailyhamsterdledata to find the day's corresponding seed data and return
        Eventually, compare to encrypted + hashed data to ensure no tampering
        */
        Random.InitState((int)currDate.Ticks);
        return null;
    }

    // If no seed data is found in file, generate data based on day's date to ensure consistency
    void GenerateSeedData()
    {

    }
}

