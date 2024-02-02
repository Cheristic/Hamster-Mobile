using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Wedge : MonoBehaviour
{
    [SerializeField] double rotationTilCleared = .99;
    private void Start()
    {
        GameManager.newGame += Despawn;
    }
    public void Spawn()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation to top
        gameObject.SetActive(true);
        StartCoroutine(CheckForRotation());
    }

    IEnumerator CheckForRotation()
    {
        yield return new WaitUntil(() => transform.rotation.z < -.999);
        yield return new WaitUntil(() => transform.rotation.z > -rotationTilCleared);
        ScoreManager.Main.IncreaseScore();
        yield return new WaitUntil(() => transform.rotation.z > 0); // After full rotation
        Despawn();
    }

    private void Despawn()
    {
        gameObject.SetActive(false);
    }
}
