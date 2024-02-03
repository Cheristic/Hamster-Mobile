using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Main { get; private set; }
    private int score;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] float pointsTilSpeedUp = 3;

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
        score = 0;
        GameManager.newGame += OnNewGame;
    }

    private void OnNewGame()
    {
        score = 0;
        scoreText.text = score.ToString();
    }

    public static event Action SpeedUp;
    public void IncreaseScore()
    {
        if (!GameManager.Main.gameRunning) return;
        score++;
        scoreText.text = score.ToString();
        if (score % pointsTilSpeedUp == 0)
        {
            SpeedUp?.Invoke();
        }
    }
}
