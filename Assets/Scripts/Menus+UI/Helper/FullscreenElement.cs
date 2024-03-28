using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenElement : MonoBehaviour
{
    BoxCollider2D _collider;
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.size = new Vector2(Screen.width, Screen.height);
    }
}
