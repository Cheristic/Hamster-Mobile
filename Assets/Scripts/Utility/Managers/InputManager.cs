using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Main { get; private set; }
    public HamsterInput input;
    public static event Action OnTouchStart;
    public static event Action OnTouchEnd;

    // 
    public Transform FastFallZone;
    internal float fastFallDividerLine;
    internal bool fastFallZoneEnabled;
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
        DisableFastFallZone(); // After Camera manager sets position
    }

    private void StartTouch(InputAction.CallbackContext c)
    {
        OnTouchStart?.Invoke();
    }
    private void EndTouch(InputAction.CallbackContext c)
    {
        OnTouchEnd?.Invoke();
    }


    public void EnableFastFallZone()
    {
        fastFallDividerLine = FastFallZone.position.y;
        fastFallZoneEnabled = true;
    }

    public void DisableFastFallZone()
    {
        fastFallDividerLine = -500;
        fastFallZoneEnabled = false;
    }

}
