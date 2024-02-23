using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSeed : MonoBehaviour
{
    public string seedName; // Used just for editor
    public SeedData seedData = new();

    [ContextMenu("Save Seed to Json")]
    public void SaveIntoJson()
    {
        string wheelSeedData = JsonUtility.ToJson(seedData);
        System.IO.File.WriteAllText("C:/Users/ethan/Desktop/DailySeedData.json", wheelSeedData);
    }
}

[Serializable]
public class SeedData
{
    public WedgeCategory[] wedgeCategories;
}


[Serializable]
public struct WedgeCategory
{
    public WedgeCategoryName category;
    public int min;
    public int max;
    public GameObject[] wedges;
    internal List<GameObject>[] pooledWedges;
}

public enum WedgeCategoryName
{
    Test,
    Normal,
    Divided,
    Beam,
    Slopes
}
