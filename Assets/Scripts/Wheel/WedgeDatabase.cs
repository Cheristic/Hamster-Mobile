using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static database for scripts (mainly the daily hamsterdle) to access for all usable wedges in each category
/// </summary>
public class WedgeDatabase : MonoBehaviour
{
    public static WedgeDatabase Main { get; private set; }
    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        } else
        {
            Main = this;
        }
    }

    [Serializable]
    public struct WedgeDataCategory
    {
        public WedgeCategoryName name;
        public List<GameObject> wedges;
    }

    public List<WedgeDataCategory> categories;
}


