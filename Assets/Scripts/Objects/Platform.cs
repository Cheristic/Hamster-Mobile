using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] Collider2D _collider;
    [SerializeField] Collider2D triggerCollider;
    public static event Action PlayerFellThroughEvent;

    private void OnEnable()
    {
        HamsterControls.HamsterFall += DisablePlatform;
        PlayerFellThroughEvent += PlayerFellThrough;
        _collider.enabled = false;
        StartCoroutine(HandleColliderActive());
    }
    private IEnumerator HandleColliderActive()
    {
        yield return new WaitUntil(() => transform.position.x > 0);
        _collider.enabled = true;
        triggerCollider.enabled = true;
        yield return new WaitUntil(() => transform.position.x < 0);
        yield return new WaitUntil(() => transform.position.x > 0);
        _collider.enabled = false;
        triggerCollider.enabled = false;
    }
    private void OnDisable()
    {
        HamsterControls.HamsterFall -= DisablePlatform;
        PlayerFellThroughEvent -= PlayerFellThrough;
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
    private void PlayerFellThrough()
    {
        _collider.enabled = true;
        StopAllCoroutines();
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Hamster"))
        {
            PlayerFellThroughEvent?.Invoke();
        }
    }
}
