using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System;
using Unity.Services.Leaderboards.Models;

public class HamsterdleScreen : MonoBehaviour
{
    [SerializeField] TMP_Text StateText;
    [SerializeField] TMP_Text NumberText;
    [SerializeField] SpriteRenderer[] Hearts;
    [SerializeField] Sprite[] heartSprites;

    public void ResetScreen()
    {
        StateText.text = "";
        NumberText.text = "";
    }

    public void UpdateHamsterdleScreen(int attempts, int hamsterdleNumber)
    {
        NumberText.text = "#" + hamsterdleNumber;

        foreach (var heart in Hearts)
        {
            heart.sprite = heartSprites[0];
        }
        if (attempts == 1) { Hearts[2].sprite = heartSprites[1]; }
        if (attempts == 2) { Hearts[1].sprite = heartSprites[1]; }
        if (attempts == 3) { 
            Hearts[0].sprite = heartSprites[1];
            StateText.text = "Come Back Tomorrow!";
        } else
        {
            StateText.text = "Tap Anywhere To Start";
        }
    }

    public void ErrorScreen()
    {
        StateText.text = "Connection Error Occurred!!";
    }
}
