using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WedgeSpawner : MonoBehaviour
{
    private List<GameObject>[] wedgePool;
    public GameObject[] wedgePrefabs;
    public float rotationTilNewWedge; // Amount latest wedge should rotate before spawning new one

    private void Start()
    {
        GameManager.gameStart += StartWedges;
        GameManager.gameEnd += StopWedges;
    }

    public void StartWedges()
    {
        Debug.Log("start");
        if (wedgePool == null)
        {
            wedgePool = new List<GameObject>[wedgePrefabs.Length];
            for (int wedgeType = 0; wedgeType < wedgePrefabs.Length; wedgeType++)
            {
                wedgePool[wedgeType] = new();
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
        while (GameManager.Main.gameRunning)
        {
            GameObject w = GetWedge(Random.Range(0, wedgePrefabs.Length));
            w.GetComponent<Wedge>().Spawn();
            yield return new WaitUntil(() => !GameManager.Main.gameRunning || 
            w.transform.rotation.z < -rotationTilNewWedge);
        }
    }

    public GameObject GetWedge(int wedgeType)
    {
        for (int i = 0; i < wedgePool[wedgeType].Count; i++)
        {
            if (!wedgePool[wedgeType][i].activeInHierarchy)
            {
                return wedgePool[wedgeType][i];
            }
        }
        // None available, create new
        GameObject temp = Instantiate(wedgePrefabs[wedgeType], transform);
        temp.SetActive(false);
        wedgePool[wedgeType].Add(temp);
        return temp;
    }
}
