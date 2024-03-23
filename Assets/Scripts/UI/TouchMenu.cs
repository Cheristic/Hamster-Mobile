using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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

    InputAction TouchPosition;

    void Start()
    {
        collide = GetComponent<Collider2D>();
        TouchPosition = InputManager.Main.input.Touch.TouchPosition;
    }

    private void OnEnable()
    {
        InputManager.OnTouchEnd += CheckTouch;
    }
    private void OnDisable()
    {
        InputManager.OnTouchEnd -= CheckTouch;
    }

    private void CheckTouch()
    {
        Ray ray = Camera.main.ScreenPointToRay(TouchPosition.ReadValue<Vector2>());
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
