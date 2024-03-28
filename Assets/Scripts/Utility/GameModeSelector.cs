using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using TMPro;

public class GameModeSelector : MonoBehaviour
{
    public static GameModeSelector Main { get; private set; }
    [Header("Links")]
    [SerializeField] WheelSeed defaultWheelSeed;
    [SerializeField] WedgeSpawner wedgeSpawner;
    internal DailyHamsterdleGenerator dailyHamsterdle;

    internal bool hamsterdleSelected;

    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        } else { Main = this; }

        // Start with default game
        ChooseDefaultSeed();
        dailyHamsterdle = GetComponent<DailyHamsterdleGenerator>();
    }
    public void ChooseDefaultSeed()
    {
        wedgeSpawner.seed = defaultWheelSeed.seedData;
        hamsterdleSelected = false;
    }

    public void ChooseDailyHamsterdle(WedgeSeedData d)
    {
        wedgeSpawner.seed = d;
        hamsterdleSelected = true;
    }

}
