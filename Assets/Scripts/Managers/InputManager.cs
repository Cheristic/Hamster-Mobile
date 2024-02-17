using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Main { get; private set; }
    public HamsterInput input;
    public static event Action<Vector2, float> OnTouchStart;
    public static event Action<Vector2, float> OnTouchEnd;
    public Transform TouchBackground;
    internal float touchDividerLine;
    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        }
        else
        {
            Main = this;
        }
        input = new();
        touchDividerLine = TouchBackground.position.y;
    }
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        input.Touch.TouchPress.started += ctx => StartTouch(ctx);
        input.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    }

    private void StartTouch(InputAction.CallbackContext c)
    {
        OnTouchStart?.Invoke(input.Touch.TouchPosition.ReadValue<Vector2>(), (float)c.startTime);
    }
    private void EndTouch(InputAction.CallbackContext c)
    {
        OnTouchEnd?.Invoke(input.Touch.TouchPosition.ReadValue<Vector2>(), (float)c.time);
    }

}
