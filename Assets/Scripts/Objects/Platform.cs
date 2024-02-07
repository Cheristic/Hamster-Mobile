using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Collider2D _collider;
    void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        HamsterControls.HamsterFall += DisablePlatform;
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
    private void OnDisable()
    {
        HamsterControls.HamsterFall -= DisablePlatform;
    }

    private void DisablePlatform()
    {
        _collider.enabled = false;
        StartCoroutine(ReenablePlatform());
    }
    private IEnumerator ReenablePlatform()
    {
        yield return new WaitForSeconds(0.2f);
        _collider.enabled = true;
    }
}
