using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manually created Wheel Seed for default and expert modes
/// </summary>
public class WheelSeed : MonoBehaviour
{
    public string seedName; // Used just for editor
    public WedgeSeedData seedData = new();

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
public class WedgeSeedData
{
    public int Number; // If not hamsterdle, both 0/blank.
    public DateTime Date;

    public bool ReturnTo1; // Should the game always cycle back to the first category

    public List<WedgeCategory> wedgeCategories;

    public WedgeSeedData() { }

    public WedgeSeedData(HamsterdleWedgeSeedData data)
    {
        Number = data.Number;
        Date = data.Date;
        ReturnTo1 = data.ReturnTo1;
        wedgeCategories = new();
        for (int i = 0; i < data.wedgeCategories.Count; i++)
        {
            WedgeCategory w = new(data.wedgeCategories[i]);
            wedgeCategories.Add(w);
        }
    }
}

public enum WedgeCategoryName
{
    Standard,
    Beam,
    Divided,
    Slopes
}

/// <summary>
/// A wedge category holds a list of wedges under that category, the minimum + max to spawn of this category, and to
/// avoid creating a new data type, it stores the pooled wedges of each wedge type in the category.
/// </summary>
[Serializable]
public class WedgeCategory
{
    public WedgeCategoryName category;
    public int min;
    public int max;
    public List<GameObject> wedges;

    public WedgeCategory(WedgeCategoryName cat)
    {
        category = cat;
    }

    public WedgeCategory(HamsterdleWedgeCategory cat)
    {
        category = cat.category;
        min = cat.min;
        max = cat.max;
        wedges = new();
        Debug.Log(cat.wedgeSelections.Length);
        for (int i = 0; i < cat.wedgeSelections.Length; i++) 
        {
            Debug.Log(cat.wedgeSelections[i]);
            // Add the corresponding wedge from database based on its index stored in json
            GameObject g = WedgeDatabase.Main.categories[(int)category].wedges[cat.wedgeSelections[i]];
            wedges.Add(g);
            Debug.Log(cat.wedgeSelections[i] + " " + g.name);
        }
    }
}
