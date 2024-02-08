using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Wedge : MonoBehaviour
{
    public double rotationTilSpawnNext;
    public double rotationTilCleared1;
    public double rotationTilCleared2;
    public double rotationTilSwitchLayer;

    SpriteRenderer[] renderers;
    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        GameManager.newGame += Despawn;
    }
    public void Spawn(float rot)
    {
        transform.rotation = Quaternion.Euler(0, 0, rot); // Reset rotation to top
        gameObject.SetActive(true);
        StartCoroutine(CheckForRotation());
        foreach(var ren in renderers)
        {
            ren.sortingLayerID = 1409643187;
        }
    }

    IEnumerator CheckForRotation()
    {
        yield return new WaitUntil(() => transform.rotation.z < -.999);
        foreach (var ren in renderers)
        {
            ren.sortingLayerID = -1308082041; // Place all obstacles in maskable layer
        }
        yield return new WaitUntil(() => transform.rotation.z > rotationTilCleared1);
        ScoreManager.Main.IncreaseScore();
        if (rotationTilCleared2 != 0) { // For obstacles long enough to reward 2
            yield return new WaitUntil(() => transform.rotation.z > rotationTilCleared2);
            ScoreManager.Main.IncreaseScore();
        }
        yield return new WaitUntil(() => transform.rotation.z > rotationTilSwitchLayer);
        foreach (var ren in renderers)
        {
            ren.sortingLayerID = -33730133; // Place all obstacles in maskable layer
        }
        yield return new WaitUntil(() => transform.rotation.z > .999);
        Despawn();
    }

    private void Despawn()
    {
        gameObject.SetActive(false);
    }
}
