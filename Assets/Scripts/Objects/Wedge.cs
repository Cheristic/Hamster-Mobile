using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Wedge : MonoBehaviour
{
    [SerializeField] double rotationTilSwitchLayers;
    public double rotationTilSpawnNext;
    [SerializeField] double rotationTilCleared = .99;
    [SerializeField] double rotationTilLooped;
    
    SpriteRenderer[] renderers;
    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        GameManager.newGame += Despawn;
    }
    public void Spawn()
    {
        Debug.Log(renderers[0].sortingLayerID);
        transform.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation to top
        gameObject.SetActive(true);
        StartCoroutine(CheckForRotation());
        foreach(var ren in renderers)
        {
            ren.sortingLayerID = 1409643187;
        }
    }

    IEnumerator CheckForRotation()
    {
        yield return new WaitUntil(() => transform.rotation.z < -rotationTilSwitchLayers);
        foreach (var ren in renderers)
        {
            ren.sortingLayerID = -1308082041; // Place all obstacles in maskable layer
        }
        yield return new WaitUntil(() => transform.rotation.z < -.999);
        yield return new WaitUntil(() => transform.rotation.z > -rotationTilCleared);
        ScoreManager.Main.IncreaseScore();
        foreach (var ren in renderers)
        {
            ren.sortingLayerID = -33730133; // Place all obstacles in maskable layer
        }
        yield return new WaitUntil(() => transform.rotation.z > rotationTilLooped); // After full rotation
        Despawn();
    }

    private void Despawn()
    {
        gameObject.SetActive(false);
    }
}
