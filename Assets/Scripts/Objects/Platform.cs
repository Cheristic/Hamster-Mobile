using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Collider2D _collider;
    // Start is called before the first frame update
    void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        HamsterControls.HamsterFall += DisablePlatform;
        _collider.enabled = true;
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
