using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WedgeSpawner : MonoBehaviour
{
    //private List<List<GameObject>>[] wedgePool; // WedgeCategory -> Wedges of type -> spawned wedges of selected wedge

    public WedgeChoice[] wedgeCategories;
    public float rotationTilNewWedge; // Amount latest wedge should rotate before spawning new one

    public enum WedgeCategory
    {
        Test,
        Normal,
        Divided,
        Beam,
        Slopes
    }
    [Serializable]
    public struct WedgeChoice
    {
        public WedgeCategory category;
        public int min;
        public int max;
        public GameObject[] wedges;
        [HideInInspector] public List<GameObject>[] pooledWedges;
    }

    private void Start()
    {
        GameManager.gameStart += StartWedges;
        GameManager.gameEnd += StopWedges;
    }


    public void StartWedges()
    {
        if (wedgeCategories[0].pooledWedges == null)
        {
            for (int w = 0; w < wedgeCategories.Length; w++)
            {
                wedgeCategories[w].pooledWedges = new List<GameObject>[wedgeCategories[w].wedges.Length];
                for (int e = 0; e < wedgeCategories[w].wedges.Length; e++)
                {
                    wedgeCategories[w].pooledWedges[e] = new List<GameObject>();
                }
            }
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
        bool normalTypeNext = true;
        // Always start with test if not empty or normal
        WedgeChoice currWedgeChoice = wedgeCategories[0];


        while (GameManager.Main.gameRunning)
        {
            if (!normalTypeNext) // Ensure a different wedge category is chosen
            {
                int newWedge;
                do
                {
                    newWedge = Random.Range(2, wedgeCategories.Length);
                } while (newWedge == (int)currWedgeChoice.category);
                // Choose wedge category and number of consecutive wedges of category to spawn
                currWedgeChoice = wedgeCategories[newWedge];
                normalTypeNext = true;
            } else
            {
                // Debug Option
                if (wedgeCategories[0].wedges.Length > 0) currWedgeChoice = wedgeCategories[0];
                else currWedgeChoice = wedgeCategories[1];
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

    public GameObject GetWedge(WedgeChoice w)
    {
        // Choose wedge of wedge category
        int wedge = Random.Range(0, w.wedges.Length); 
        for (int i = 0; i < wedgeCategories[(int)w.category].pooledWedges[wedge].Count; i++)
        {
            // Loop through wedge prefab type's corresponding pool
            if (!wedgeCategories[(int)w.category].pooledWedges[wedge][i].activeInHierarchy)
            {
                return wedgeCategories[(int)w.category].pooledWedges[wedge][i];
            }
        }
        // None available, create new
        GameObject temp = Instantiate(wedgeCategories[(int)w.category].wedges[wedge], transform);
        temp.SetActive(false);
        wedgeCategories[(int)w.category].pooledWedges[wedge].Add(temp);
        return temp;
    }
}
