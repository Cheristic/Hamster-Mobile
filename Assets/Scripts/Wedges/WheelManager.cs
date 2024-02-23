using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Threading.Tasks;

public class WheelManager : MonoBehaviour
{
    public static WheelManager Main { get; private set; }
    [Header("Links")]
    [SerializeField] WheelSeed defaultWheelSeed;
    [SerializeField] WheelSeed expertWheelSeed;
    [SerializeField] WedgeSpawner wedgeSpawner;
    [SerializeField] DailyHamsterdle dailyHamsterdle;

    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        } else { Main = this; }
        ChooseDefaultSeed();
    }
    public void ChooseDefaultSeed()
    {
        wedgeSpawner.seed = defaultWheelSeed.seedData;
        Random.InitState((int)DateTime.Now.Ticks);
    }
    public void ChooseExpertSeed()
    {
        wedgeSpawner.seed = expertWheelSeed.seedData;
        Random.InitState((int)DateTime.Now.Ticks);

    }
    public async void RunDailyHamstedle()
    {
        /*DailyHamsterdle.HamsterdleStatus status = CloudManager.cloud.HasCompletedDaily();

        if (status == DailyHamsterdle.HamsterdleStatus.CannotConnectToGoogle)
        {
            Debug.LogError("Cannot connect to Google servers");
            return;
        }
        if (status == DailyHamsterdle.HamsterdleStatus.HasCompletedHamsterdle)
        {
            Debug.Log("Already completed today's hamsterdle");
            return;
        }*/
        // Has not completed hamsterdle, proceed

        SeedData d = await dailyHamsterdle.RetrieveDailySeed();
        if (d == null)
        {
            Debug.LogError("Not connected to Internet or API servers down");
            return;
        }
    }

}
