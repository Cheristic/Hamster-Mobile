using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HamsterControls : MonoBehaviour
{
    [Header("Hamster Values")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float isGroundedCheckDistance;
    [SerializeField] LayerMask obstaclesLayerMask;

    [Header("Controls")]
    [SerializeField] private float jumpLimitTime = 1f;
    [SerializeField] private float jumpBoost = 200f;
    [SerializeField] private float bufferTime;

    // Links
    Rigidbody2D rb;
    Collider2D collide;
    InputAction TouchPress;
    InputAction TouchPosition;

    // Misc variables
    bool CanFall = false;
    bool IsHolding = false;
    public static event Action HamsterFall;
    Vector3 leftGroundedChecker = Vector3.zero;
    Vector3 rightGroundedChecker = Vector3.zero;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collide = GetComponent<Collider2D>();

        rightGroundedChecker = transform.InverseTransformPoint(new Vector3(collide.bounds.max.x, collide.bounds.center.y - collide.bounds.extents.y, 0));
        leftGroundedChecker = transform.InverseTransformPoint(new Vector3(collide.bounds.min.x, collide.bounds.center.y - collide.bounds.extents.y, 0));


        GameManager.gameStart += OnGameStart;
        GameManager.gameEnd += OnGameEnd;
        GameManager.newGame += OnNewGame;

        TouchPress = InputManager.Main.input.Touch.TouchPress;
        TouchPosition = InputManager.Main.input.Touch.TouchPosition;

        StartCoroutine(HamsterReadyChecker());
    }

    private void OnGameStart()
    {
        InputManager.OnTouchStart += StartTouch;
    }

    private void OnGameEnd()
    {
        InputManager.OnTouchStart -= StartTouch;
        gameObject.SetActive(false);
    }
    private void OnNewGame()
    {
        transform.position = new Vector2(0, 7);
        gameObject.SetActive(true);
        rb.velocity = new Vector2(0, -5);
        StartCoroutine(HamsterReadyChecker());
    }
    public static event Action TriggerHamsterReady;
    private IEnumerator HamsterReadyChecker()
    {
        yield return new WaitUntil(() => IsGrounded);
        TriggerHamsterReady?.Invoke();
    }

    private void StartTouch()
    {
        HandleInput();
        StartCoroutine(HoldingChecks());
    }


    // The bool returned is just for title screen
    public bool HandleInput()
    {
        float touchYPos = Camera.main.ScreenToWorldPoint(TouchPosition.ReadValue<Vector2>()).y;

        if (touchYPos >= InputManager.Main.fastFallDividerLine) // ABOVE FAST FALL LINE - Will always be above line with fast fall zone disabled
        {
            if (IsGrounded)
            {
                ExecuteJump();
                return true;
            }
            else
            {
                if (CanFall && !InputManager.Main.fastFallZoneEnabled)
                {
                   TryFall();
                }
                return false;
            }
        } else // BELOW FAST FALL LINE
        {
            if (CanFall)
            {
                TryFall();
            }
            return true;
        }
    }
    private IEnumerator HoldingChecks()
    {
        if (IsHolding) yield break;
        IsHolding = true;

        while (GameManager.Main.gameRunning && TouchPress.inProgress)
        {
            float touchYPos = Camera.main.ScreenToWorldPoint(TouchPosition.ReadValue<Vector2>()).y;
            if (touchYPos >= InputManager.Main.fastFallDividerLine) // Input to jump when in jump zone
            {
                if (rb.velocity.y <= 0 && IsGrounded)
                {
                    ExecuteJump();
                }
            }
            else
            {
                if (CanFall) // Input to fall when finger slides into fast fall zone
                {
                    TryFall();
                }
            }
            yield return null;
        }

        IsHolding = false;
    }

    private void ExecuteJump()
    {
        CanFall = true;
        rb.velocity = new Vector2(0, jumpHeight);
        StartCoroutine(JumpBoost());
    }

    private IEnumerator JumpBoost()
    {
        float jumpTime = 0f;
        // while moving up && finger is tapped && less than limit
        while (rb.velocity.y > 0 && TouchPress.inProgress && jumpTime < jumpLimitTime)
        {
            rb.AddForce(new Vector2(0, jumpBoost));
            jumpTime += Time.deltaTime;
            yield return null;
        }
    }

    /*
    // Input to wait if player originally jumps but slides finger into zone
    private IEnumerator WaitForFastFallZone()
    {
        while (GameManager.Main.gameRunning && TouchPress.inProgress)
        {
            float touchYPos = Camera.main.ScreenToWorldPoint(TouchPosition.ReadValue<Vector2>()).y;
            if (touchYPos < InputManager.Main.fastFallDividerLine)
            {
                TryFall();
            }
            yield return null;
        }
    }

    // Input for when player holds down screen and waits for hampster to land in order to buffer jump
    private IEnumerator PreHoldJump()
    {
        if (IsJumpHolding) yield break;

        IsJumpHolding = true;

        while (GameManager.Main.gameRunning && TouchPress.inProgress)
        {
            if (rb.velocity.y <= 0 && IsGrounded && )
            {
                ExecuteJump();
            }
            yield return null;
        }
        IsJumpHolding = false;
    }*/


    private void TryFall()
    {
        rb.velocity = new Vector2(rb.velocity.x, -jumpHeight);
        HamsterFall?.Invoke();
        CanFall = false;
    }


    private void FixedUpdate()
    {
        if (!GameManager.Main.gameRunning) return;

        // Move hamster back to center
        if (Mathf.Abs(transform.position.x) > .01)
        {
            float vel = Mathf.Abs(transform.position.x) > 5 ? transform.position.x : transform.position.x*1.5f;
            rb.velocity = new Vector2(-vel, rb.velocity.y);
        }

    }

    private bool IsGrounded 
    {
        get
        {

            RaycastHit2D hit2 = Physics2D.Raycast(transform.TransformPoint(rightGroundedChecker), Vector2.down, isGroundedCheckDistance, obstaclesLayerMask);
            Debug.DrawRay(transform.TransformPoint(rightGroundedChecker), Vector2.down * isGroundedCheckDistance, Color.green, 1f);
            if (hit2.collider != null)
            {
                return true;
            }
            RaycastHit2D hit1 = Physics2D.Raycast(transform.TransformPoint(leftGroundedChecker), Vector2.down, isGroundedCheckDistance, obstaclesLayerMask);
            Debug.DrawRay(transform.TransformPoint(leftGroundedChecker), Vector2.down * isGroundedCheckDistance, Color.green, 1f);
            if (hit1.collider != null)
            {
                return true;
            }

            return false;
        }
    }

}
