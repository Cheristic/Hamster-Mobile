using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Threading.Tasks;

public class GameModeSelector : MonoBehaviour
{
    public static GameModeSelector Main { get; private set; }
    [Header("Links")]
    [SerializeField] WheelSeed defaultWheelSeed;
    [SerializeField] WheelSeed expertWheelSeed;
    [SerializeField] WedgeSpawner wedgeSpawner;
    internal DailyHamsterdle dailyHamsterdle;

    bool hamsterdleSelected;

    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        } else { Main = this; }

        // Start with default game
        ChooseDefaultSeed();
        dailyHamsterdle = GetComponent<DailyHamsterdle>();
    }
    public void ChooseDefaultSeed()
    {
        wedgeSpawner.seed = defaultWheelSeed.seedData;
        hamsterdleSelected = false;
    }

    public async void RunDailyHamstedle()
    {
        // If hit again, return to normal mode
        if (hamsterdleSelected)
        {
            ChooseDefaultSeed();
            Debug.Log("Hamsterdle unselected");
            return;
        }

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

        WedgeSeedData d = await dailyHamsterdle.RetrieveDailySeed();
        if (d == null)
        {
            Debug.LogError("Not connected to Internet or API servers down");
            return;
        }
        hamsterdleSelected = true;
        wedgeSpawner.seed = d;
    }

}
