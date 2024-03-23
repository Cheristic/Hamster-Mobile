using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using Unity.VisualScripting;
public class DailyHamsterdle : MonoBehaviour
{
    List<HamsterdleWedgeSeedData> DailyHamsterdleData;
    DateTime launchDate0;
    [SerializeField] int launchMonth;
    [SerializeField] int launchDay;

    public enum HamsterdleStatus
    {
        CannotConnectToGoogle,
        HasNotCompletedHamsterdle,
        HasCompletedHamsterdle
    }
    private void Start()
    {
        string s = Resources.Load<TextAsset>("GameModeSeeds/HamsterdleSeeds").text;
        DailyHamsterdleData = JsonConvert.DeserializeObject<List<HamsterdleWedgeSeedData>>(s);
        launchDate0 = new(2024, launchMonth, launchDay);
    }

    struct ServerDateTime
    {
        public string datetime;
    }
    internal WedgeSeedData dailyWedgeSeedData;
    public async Task<WedgeSeedData> RetrieveDailySeed()
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
        int HamsterdleNumber = (currDate - launchDate0).Days;

        // LINQ query the dailyhamsterdledata to find the day's corresponding seed data and return
        var seedExistQuery =
            from seed in DailyHamsterdleData
            where seed.Number == HamsterdleNumber
            select seed;

        WedgeSeedData workingSeed = null;
        foreach (var seed in seedExistQuery) // Should only be 1 seed
        {
            // Convert from json HamsterdleSeed to normal seed
            // ### EVENTUALLY CHECK IF DATA IS ALREADY LOADED TO AVOID EXTRA PROCESSING ###
            Debug.Log("FOUND SEED " + seed.Number);
            workingSeed = new WedgeSeedData(seed);
        }

        if (workingSeed == null) // No seed found in data, generate new seed
        {
            Random.InitState((int)currDate.Ticks);
            Debug.Log("Generating new seed");
            workingSeed = GenerateWedgeSeedData();
            // Add seed's current date + number to data holder
            workingSeed.Date = currDate;
            workingSeed.Number = HamsterdleNumber;
        }

        // Use Unity's CDN to provide disconnect if game gets bigger and security is an issue

        return workingSeed;
    }

    // If no seed data is found in file, generate data based on day's date to ensure consistency
    WedgeSeedData GenerateWedgeSeedData()
    {
        WedgeSeedData data = new();
        data.wedgeCategories = new();

        WedgeCategory standard = GenerateSeededCategory(WedgeCategoryName.Standard, .80f, 1, 12, 4, 15);
        if (standard != null) data.wedgeCategories.Add(standard);

        WedgeCategory beam = GenerateSeededCategory(WedgeCategoryName.Beam, .50f, 2, 7, 4, 10);
        if (beam != null) data.wedgeCategories.Add(beam);

        WedgeCategory divided = GenerateSeededCategory(WedgeCategoryName.Divided, .50f, 1, 3, 3, 7);
        if (divided != null) data.wedgeCategories.Add(divided);

        if (data.wedgeCategories.Count == 0) // If by some statistical improbability, none are chosen, choose standard
        {
            standard = GenerateSeededCategory(WedgeCategoryName.Standard, 1f, 1, 12, 4, 15);
            data.wedgeCategories.Add(standard);
        }

        data.ReturnTo1 = Random.Range(0, 2) == 1 ? true : false; // 50% chance for either

        return data;
    }

    WedgeCategory GenerateSeededCategory(WedgeCategoryName name, float chance, int min_low, int min_high, int max_low, int max_high)
    {
        if (Random.Range(0f, 1f) <= chance) // chance for category
        {
            WedgeCategory cat = new(name);
            cat.min = Random.Range(min_low, min_high+1);
            cat.max = 0;
            while (cat.max < cat.min)
            {
                cat.max = Random.Range(max_low, max_high+1);
            }

            List<GameObject> wedgesOfCategory = WedgeDatabase.Main.categories[(int)name].wedges;

            int numWedgeTypes = Random.Range(1, wedgesOfCategory.Count+1);
            cat.wedges = new();
            ChooseWedgeCombinations();

            void ChooseWedgeCombinations()
            {
                numWedgeTypes--;
                GameObject wedgeToAdd;
                do
                {
                    wedgeToAdd = wedgesOfCategory[Random.Range(0, wedgesOfCategory.Count)];
                } while (cat.wedges.Contains(wedgeToAdd));
                cat.wedges.Add(wedgeToAdd);
                if (numWedgeTypes > 0) ChooseWedgeCombinations();
            }

            return cat;
        }
        return null;
    }

    

}

