using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Main { get; private set; }
    public bool gameRunning;
    public WedgeSpawner wedgeSpawner;
    public Wheel wheel;

    void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        }
        else
        {
            Main = this;
        }
        gameRunning = false; 
    }

    public static event Action gameStart;
    public static event Action gameEnd;
    public static event Action newGame;
    public void StartGame()
    {
        gameRunning = true;
        gameStart?.Invoke();
    }

    public void EndGame() // Called when hamster hits a damaging obstacle
    {
        if (!gameRunning) return; // Ensure no multiple calls
        gameRunning = false;
        gameEnd?.Invoke();
    }

    public void NewGame()
    {
        newGame?.Invoke();
    }
}
