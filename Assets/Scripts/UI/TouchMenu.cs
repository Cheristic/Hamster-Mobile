using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TouchMenu : MonoBehaviour
{
    Collider2D collide;
    [Serializable]
    public struct TouchBox
    {
        public Collider2D collide;
        public UnityEvent onTouch;
    };

    public TouchBox[] touchBoxes;

    void Start()
    {
        collide = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        InputManager.OnTouchEnd += CheckTouch;
    }
    private void OnDisable()
    {
        InputManager.OnTouchEnd -= CheckTouch;
    }

    private void CheckTouch(Vector2 pos, float time)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        foreach (var box in touchBoxes)
        {
            // If player touches box, invoke event
            if (box.collide.bounds.IntersectRay(ray))
            {
                box.onTouch.Invoke();
                return;
            }
        }
    }
}
