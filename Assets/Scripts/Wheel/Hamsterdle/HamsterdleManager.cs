using UnityEngine;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Leaderboards;

public class HamsterdleManager : MonoBehaviour
{
    public static HamsterdleManager Main { get; private set; }

    public static event Action EnterHamsterdleScreen;
    [Header("Main Menu Hub Links")]
    [SerializeField] DailyHamsterdleGenerator hamsterdleGenerator;
    [SerializeField] TMP_Text hamsterdleMMText;
    [SerializeField] WedgeSpawner wedgeSpawner;
    [SerializeField] MainMenuUI mainMenuUI;

    [Header("Screen Links")]
    [SerializeField] HamsterdleScreen hamsterdleScreen;
    [SerializeField] GameObject TapToStart;

    int localAttempts;
    int currentHamsterdleNumber;
    bool cycledOverToNewDay;

    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        }
        else
        {
            Main = this;
        }
    }

    public async void TryEnterHamsterdleScreen()
    {
        void ErrorState()
        {
            Debug.LogWarning("TryEnterHamsterdleScreen() Not connected to Internet or API servers down");
            hamsterdleScreen.ErrorScreen();
            return;
        }

        hamsterdleScreen.ResetScreen();
        mainMenuUI.ChangeMenu(2);


        if (!CloudServicesManager.cloud.IsConnected)
        {
            // Failed to authenticate Unity or Platform Services
            ErrorState();
            return;
        }

        // Get the latest hamsterdle data
        WedgeSeedData seedData = await hamsterdleGenerator.RetrieveDailySeed();
        if (seedData == null) { 
            ErrorState();
            return;
        }
        currentHamsterdleNumber = seedData.Number;

        // Verify the players current number of attempts of the current hamsterdle
        LeaderboardEntry entry = await CloudServicesManager.cloud.unityServices.GetHamsterdleScore();
        if (entry == null) {
            localAttempts = 0;
        } else
        {
            localAttempts = Int32.Parse(entry.Metadata);
        }

        Debug.Log("enter hamsterdle menu!!!!");

        // NO MORE CHECKS, GOOD TO ENTER

        GameModeSelector.Main.ChooseDailyHamsterdle(seedData);
        if (localAttempts < 3) TapToStart.SetActive(true);
        cycledOverToNewDay = false;
        GameManager.newGame += NextAttempt;
        hamsterdleScreen.UpdateHamsterdleScreen(localAttempts, currentHamsterdleNumber);

    }

    public void ExitHamsterdleScreen()
    {
        TapToStart.SetActive(false);
        GameManager.newGame -= NextAttempt;
        mainMenuUI.ChangeMenu(0);
    }


    public async void AddScoreToHamsterdle()
    {
        localAttempts++;

        void ErrorState()
        {
            Debug.LogWarning("AddScoreToHamsterdle() Not connected to Internet or API servers down");
            hamsterdleScreen.ErrorScreen();
            TapToStart.SetActive(false);
            return;
        }

        // Ensure still connected
        if (!CloudServicesManager.cloud.IsConnected)
        {
            // Failed to authenticate
            ErrorState();
            return;
        }
        // Get the latest hamsterdle data
        WedgeSeedData seedData = await hamsterdleGenerator.RetrieveDailySeed();
        if (seedData == null)
        {
            ErrorState();
            return;
        }

        if (seedData.Number != currentHamsterdleNumber)
        {
            // We know its a new attempt
            cycledOverToNewDay = true;
        }

        if (cycledOverToNewDay) return; // Don't update scores to cloud if wrong day

        CloudServicesManager.cloud.unityServices.AddScoreToHamsterdleBoard(localAttempts);
        CloudServicesManager.cloud.platformMan.AddScore();
    }

    public async void NextAttempt()
    {
        if (GameModeSelector.Main.hamsterdleSelected)
        {
            void ErrorState()
            {
                Debug.LogWarning("NextAttempt() Not connected to Internet or API servers down");
                hamsterdleScreen.ErrorScreen();
                TapToStart.SetActive(false);
                return;
            }

            // Ensure still connected
            if (!CloudServicesManager.cloud.IsConnected)
            {
                // Failed to authenticate
                ErrorState();
                return;
            }

            // Get the latest hamsterdle data
            WedgeSeedData seedData = await hamsterdleGenerator.RetrieveDailySeed();
            if (seedData == null)
            {
                ErrorState();
                return;
            }
            if (seedData.Number != currentHamsterdleNumber)
            {
                // We know its a new attempt
                cycledOverToNewDay = true;
            } 
            hamsterdleScreen.UpdateHamsterdleScreen(localAttempts, currentHamsterdleNumber);

            // NO MORE CHECKS, GOOD TO ENTER

        }
    }
}