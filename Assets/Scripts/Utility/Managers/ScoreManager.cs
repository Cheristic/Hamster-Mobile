using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Main { get; private set; }
    public int score;
    internal bool highscoreEnabled;
    internal int highscore;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] NewScoreText newScoreText;
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
        GameManager.gameEnd += OnGameEnd;
        GameManager.newGame += OnNewGame;
    }

    private void OnGameEnd()
    {
        if (highscoreEnabled && score > highscore)
        {
            highscore = score;
            newScoreText.ShowNewText();
        }
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
