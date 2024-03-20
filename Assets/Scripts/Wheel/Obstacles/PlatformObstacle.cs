using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObstacle : MonoBehaviour
{
    [SerializeField] Collider2D objectCollider;
    [SerializeField] Collider2D triggerCollider;
    public static event Action PlayerFellThroughEvent;

    private void OnEnable()
    {
        HamsterControls.HamsterFall += DisablePlatform;
        PlayerFellThroughEvent += PlayerFellThrough;
        objectCollider.enabled = false;
        StartCoroutine(HandleColliderActive());
    }
    private IEnumerator HandleColliderActive()
    {
        yield return new WaitUntil(() => transform.position.x > 0);
        objectCollider.enabled = true;
        triggerCollider.enabled = true;
        yield return new WaitUntil(() => transform.position.x < 0);
        yield return new WaitUntil(() => transform.position.x > 0);
        objectCollider.enabled = false;
        triggerCollider.enabled = false;
    }
    private void OnDisable()
    {
        HamsterControls.HamsterFall -= DisablePlatform;
        PlayerFellThroughEvent -= PlayerFellThrough;
    }

    private void DisablePlatform()
    {
        objectCollider.enabled = false;
        StartCoroutine(ReenablePlatform());
    }
    private IEnumerator ReenablePlatform()
    {
        yield return new WaitForSeconds(0.2f);
        objectCollider.enabled = true;
    }
    private void PlayerFellThrough()
    {
        objectCollider.enabled = true;
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
