using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WedgeSpawner : MonoBehaviour
{
    internal SeedData seed;

    private void Start()
    {
        if (seed.wedgeCategories[0].pooledWedges == null)
        {
            for (int w = 0; w < seed.wedgeCategories.Length; w++)
            {
                seed.wedgeCategories[w].pooledWedges = new List<GameObject>[seed.wedgeCategories[w].wedges.Length];
                for (int e = 0; e < seed.wedgeCategories[w].wedges.Length; e++)
                {
                    seed.wedgeCategories[w].pooledWedges[e] = new List<GameObject>();
                }
            }
        }
        GameManager.gameStart += StartWedges;
        GameManager.gameEnd += StopWedges;
    }


    public void StartWedges()
    {
        StartCoroutine(SpawnWedge());
    }

    public void StopWedges()
    {
        StopCoroutine(SpawnWedge());
    }

    IEnumerator SpawnWedge()
    {
        bool firstType = true;
        bool normalTypeNext = true;
        // Always start with normal
        WedgeCategory currWedgeChoice = seed.wedgeCategories[0];


        while (GameManager.Main.gameRunning)
        {
            if (!normalTypeNext) // Ensure a different wedge category is chosen
            {
                int newWedge;
                do
                {
                    newWedge = Random.Range(1, seed.wedgeCategories.Length);
                    // Change this part later with gamemodes
                } while (newWedge == (int)currWedgeChoice.category);
                // Choose wedge category and number of consecutive wedges of category to spawn
                currWedgeChoice = seed.wedgeCategories[newWedge];
                normalTypeNext = true;
            } else
            {
                // Debug Option
                currWedgeChoice = seed.wedgeCategories[0];
                normalTypeNext = false;
            }

            int wedgesToSpawn = Random.Range(currWedgeChoice.min, currWedgeChoice.max + 1);
            for (int i = 0; GameManager.Main.gameRunning && i < wedgesToSpawn ; i++)
            {
                GameObject w = GetWedge(currWedgeChoice);
                Wedge wed = w.GetComponent<Wedge>();
                if (firstType && i == 0) // Make first wedge spawn closer
                {
                    wed.Spawn(-30f);
                    firstType = false;
                } else
                {
                    wed.Spawn(0);
                }
                yield return new WaitUntil(() => !GameManager.Main.gameRunning ||
                w.transform.rotation.z < wed.rotationTilSpawnNext);
            }
        }
    }

    public GameObject GetWedge(WedgeCategory w)
    {
        // Choose wedge of wedge category
        int wedge = Random.Range(0, w.wedges.Length); 
        for (int i = 0; i < seed.wedgeCategories[(int)w.category].pooledWedges[wedge].Count; i++)
        {
            // Loop through wedge prefab type's corresponding pool
            if (!seed.wedgeCategories[(int)w.category].pooledWedges[wedge][i].activeInHierarchy)
            {
                return seed.wedgeCategories[(int)w.category].pooledWedges[wedge][i];
            }
        }
        // None available, create new
        GameObject temp = Instantiate(seed.wedgeCategories[(int)w.category].wedges[wedge], transform);
        temp.SetActive(false);
        seed.wedgeCategories[(int)w.category].pooledWedges[wedge].Add(temp);
        return temp;
    }
}
