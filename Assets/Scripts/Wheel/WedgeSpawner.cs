using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Low level script solely responsible for spawning wedges in regular intervals
/// </summary>
public class WedgeSpawner : MonoBehaviour
{
    internal WedgeSeedData seed;
    [SerializeField] Transform wheelObject;

    internal List<List<GameObject>>[]PooledWedges; // Matrix goes [Category][Wedge][WedgeInstances]

    private void Start()
    {
        if (PooledWedges == null)
        {
            PooledWedges = new List<List<GameObject>>[WedgeDatabase.Main.categories.Count];
            for (int w = 0; w < WedgeDatabase.Main.categories.Count; w++)
            {
                PooledWedges[w] = new List<List<GameObject>>();
                for (int e = 0; e < WedgeDatabase.Main.categories[w].wedges.Count; e++)
                {
                    PooledWedges[w].Add(new List<GameObject>());
                }
            }
        }
        GameManager.gameStart += StartWedges;
        GameManager.gameEnd += StopWedges;
    }


    public void StartWedges()
    {
        if (seed.Number == 0) // Not Hamsterdle
        {
            Random.InitState((int)DateTime.Now.Ticks);
            Debug.Log("Regular " + (int)DateTime.Now.Ticks);
        } else // Is Hamsterdle
        {
            Debug.Log(seed.Date.Ticks);
            Debug.Log((int)(seed.Date.Ticks));
            Random.InitState((int)(seed.Date.Ticks));
        }
        StartCoroutine(SpawnWedge());
    }

    public void StopWedges()
    {
        StopCoroutine(SpawnWedge());
    }

    IEnumerator SpawnWedge()
    {
        bool firstType = true;
        bool standardTypeNext = true;
        // Always start with first category
        WedgeCategory currWedgeChoice = seed.wedgeCategories[0];


        while (GameManager.Main.gameRunning)
        {
            // ### BEGIN CYCLE ###

            // ### STAGE 1 - CHOOSE CATEGORY ###

            if (seed.wedgeCategories.Count > 1) // Only 1 category, just choose again
            {
                if (seed.ReturnTo1) // Flip between the first category and a random category
                {
                    if (!standardTypeNext) 
                    {
                        int newWedgeCategory;
                        do
                        {
                            // Ensure a different wedge category is chosen
                            newWedgeCategory = Random.Range(1, seed.wedgeCategories.Count);
                            // Change this part later with gamemodes
                        } while (newWedgeCategory == (int)currWedgeChoice.category);
                        // Choose wedge category and number of consecutive wedges of category to spawn
                        currWedgeChoice = seed.wedgeCategories[newWedgeCategory];
                        standardTypeNext = true;
                    }
                    else
                    {
                        currWedgeChoice = seed.wedgeCategories[0];
                        standardTypeNext = false;
                    }
                }
                else // Choose a random category each cycle
                {
                    int newWedgeCategory;
                    do
                    {
                        newWedgeCategory = Random.Range(1, seed.wedgeCategories.Count);
                        // Change this part later with gamemodes
                    } while (newWedgeCategory == (int)currWedgeChoice.category);
                    // Choose wedge category and number of consecutive wedges of category to spawn
                    currWedgeChoice = seed.wedgeCategories[newWedgeCategory];
                }
            }


            // ### STAGE 2 - SPAWN WEDGES IN CATEGORY ###

            int wedgesToSpawn = Random.Range(currWedgeChoice.min, currWedgeChoice.max + 1);
            for (int i = 0; GameManager.Main.gameRunning && i < wedgesToSpawn ; i++)
            {
                GameObject w = GetWedge(currWedgeChoice);
                WedgeObject wed = w.GetComponent<WedgeObject>();
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
        // Choose wedge of wedge category and retrieve from local pooled wedges

        GameObject wedge = w.wedges[Random.Range(0, w.wedges.Count)];
        int wedgeIndex = wedge.name[wedge.name.Length - 1] - '0'; // Get the corresponding database index for the wedge
        // WEDGE FORMAT: NAME_INDEX (Ex: Standard_1)
        Debug.Log(wedge.name + " " + wedgeIndex);

        List<GameObject> wedgePool = PooledWedges[(int)w.category][wedgeIndex];
        for (int i = 0; i < wedgePool.Count; i++)
        {
            // Loop through wedge prefab type's corresponding pool
            if (!wedgePool[i].activeInHierarchy)
            {
                return wedgePool[i];
            }
        }

        Debug.Log("creating wedge " + wedge.name);
        // None available, create new
        GameObject temp = Instantiate(wedge, wheelObject);
        temp.SetActive(false);
        wedgePool.Add(temp);
        return temp;
    }
}
