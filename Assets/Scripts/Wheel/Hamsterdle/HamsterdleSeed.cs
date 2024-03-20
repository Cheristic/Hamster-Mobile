using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterdleSeed : MonoBehaviour
{
    public HamsterdleWedgeSeedData seedData = new();

    [ContextMenu("Save Seed to Json")]
    public void SaveIntoJson()
    {
        string wheelSeedData = JsonUtility.ToJson(seedData);
        System.IO.File.WriteAllText("C:/Users/ethan/Desktop/DailySeedData.json", wheelSeedData);
    }
}

/// <summary>
/// Details of the seed's wedge categories
/// </summary>
[Serializable]
public class HamsterdleWedgeSeedData
{
    public int Number; // If not hamsterdle, both 0/blank.
    public DateTime Date;

    public bool ReturnTo1; // Should the game always cycle back to the first category

    public List<HamsterdleWedgeCategory> wedgeCategories;
}

[Serializable]
public class HamsterdleWedgeCategory
{
    public WedgeCategoryName category;
    public int min;
    public int max;
    public int[] wedgeSelections;
}