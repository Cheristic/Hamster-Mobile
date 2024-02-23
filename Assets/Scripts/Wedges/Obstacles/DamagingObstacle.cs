using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObstacle : MonoBehaviour
{
    private Collider2D _collider;
    void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _collider.enabled = false;
        StartCoroutine(HandleColliderActive());
    }
    private IEnumerator HandleColliderActive()
    {
        yield return new WaitUntil(() => transform.position.x > 0);
        _collider.enabled = true;
        yield return new WaitUntil(() => transform.position.x < 0);
        yield return new WaitUntil(() => transform.position.x > 0);
        _collider.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hamster"))
        {
            GameManager.Main.EndGame();
        }
    }
}
