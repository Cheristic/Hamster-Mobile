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

    // Misc variables
    bool CanFall = false;
    bool IsJumpHolding = false;
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

    private void StartTouch(Vector2 pos, float time)
    {
        HandleInput();
        StartCoroutine(HampsterJumpHold());
    }


    // Return true when jump successfully, false otherwise
    public bool HandleInput()
    {

        if (IsGrounded)
        {
            HampsterJump();
            return true;
        }
        else
        {
            if (CanFall)
            {
                TryFall();
            }
            //StartCoroutine(BufferJump());
            return false;
        }
    }

    private void HampsterJump()
    {
        CanFall = true;
        rb.velocity = new Vector2(0, jumpHeight);
        StartCoroutine(JumpBoost());
    }

    // Input for when player holds down screen and waits for hampster to land in order to buffer jump
    private IEnumerator HampsterJumpHold()
    {
        if (IsJumpHolding) yield break;

        IsJumpHolding = true;

        while (GameManager.Main.gameRunning && TouchPress.inProgress)
        {
            if (rb.velocity.y <= 0 && IsGrounded)
            {
                HampsterJump();
            }
            yield return null;
        }
        IsJumpHolding = false;
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

    private void TryFall()
    {
        rb.velocity = new Vector2(rb.velocity.x, -jumpHeight);
        Debug.Log("ham fall");
        HamsterFall?.Invoke();
        CanFall = false;
    }

    /*
    private IEnumerator BufferJump()
    {
        float time = 0f;
        while (!IsGrounded)
        {
            if (time >= bufferTime)
            {
                // If player cannot meet conditions within buffer window, cancel input
                yield break;
            }
            time += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("Buffer Jump " + time);
        if (time >= bufferTime) yield break;
        // if conditions are met, call HandleInput() once more to activate it
        HandleInput();
    }*/


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
