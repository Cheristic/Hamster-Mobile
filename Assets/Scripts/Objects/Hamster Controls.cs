using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HamsterControls : MonoBehaviour
{
    [Header("Hamster Values")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float isGroundedCheckDistance = 0.2f;
    [SerializeField] LayerMask obstaclesLayerMask;

    [Header("Controls")]
    [SerializeField] private float touchDistanceToFall = 1f;
    [SerializeField] private float jumpLimitTime = 1f;
    [SerializeField] private float jumpBoost = 200f;

    Rigidbody2D rb;
    Collider2D collide;
    bool isAscending;
    bool canFall;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collide = GetComponent<Collider2D>();
        isAscending = false;
        canFall = true;
        GameManager.gameStart += OnGameStart;
        GameManager.gameEnd += OnGameEnd;
        GameManager.newGame += OnNewGame;
        InputManager.OnTouchStart += StartTouch;
        
    }

    private void OnGameStart()
    {
        InputManager.OnTouchEnd += EndTouch;
    }
    private void OnGameEnd()
    {
        InputManager.OnTouchEnd -= EndTouch;
        gameObject.SetActive(false);
    }
    private void OnNewGame()
    {
        transform.position = new Vector2(0, 7);
        gameObject.SetActive(true);
        rb.velocity = new Vector2(0, -5);

    }

    private float touchStartPos;
    private void StartTouch(Vector2 pos, float time)
    {
        touchStartPos = Camera.main.ScreenToWorldPoint(pos).y;
    }
    private void EndTouch(Vector2 pos, float time)
    {
        StopCoroutine("FallInputCheck");
        canFall = true;
    }
    float touchPos;
    
    private void Update()
    {
        if (GameManager.Main.gameRunning && InputManager.Main.input.Touch.TouchPress.inProgress)
        {
            touchPos = Camera.main.ScreenToWorldPoint(InputManager.Main.input.Touch.TouchPosition.ReadValue<Vector2>()).y;
            bool aboveLine = false;
            if (touchPos >= InputManager.Main.touchDividerLine) // If above line, try to jump
            {
                TryJump();
                aboveLine = true;
            }
            if (canFall) StartCoroutine(FallInputCheck(aboveLine));
        }
    }

    // Active while screen is being pressed
    private IEnumerator FallInputCheck(bool aboveLine)
    {
        canFall = false;
        
        // do fall check
        while (true)
        {
            if (aboveLine) // Initial input is above line
            {
                if (touchPos < InputManager.Main.touchDividerLine &&
                    touchPos < touchStartPos - touchDistanceToFall) 
                {
                    TryFall();
                    yield break;
                }
            } 
            else
            {
                if (touchPos < touchStartPos - touchDistanceToFall)
                {
                    TryFall();
                    yield break;
                }
            }

            touchPos = InputManager.Main.input.Touch.TouchPosition.ReadValue<Vector2>().y;
            yield return null;
        }
    }

    public bool TryJump()
    {
        if (!isAscending && IsGrounded)
        {
            isAscending = true;     
            rb.velocity = new Vector2(0, jumpHeight);
            StartCoroutine(Jump());
            return true;
        }
        return false;
    }
    bool addJumpBoost = false;
    private IEnumerator Jump()
    {
        float jumpTime = 0f;
        // while moving up && finger is tapped && less than limit
        while(isAscending && InputManager.Main.input.Touch.TouchPress.inProgress && jumpTime < jumpLimitTime)
        {
            jumpTime += Time.deltaTime;
            addJumpBoost = true;
            yield return null;
        }
        addJumpBoost = false;

    }

    public static event Action HamsterFall;
    private void TryFall()
    {
        rb.velocity = new Vector2(rb.velocity.x, -jumpHeight);
        HamsterFall?.Invoke();
    }


    private void FixedUpdate()
    {
        if (addJumpBoost)
        {
            rb.AddForce(new Vector2(0, jumpBoost));
        }
        if (rb.velocity.y <= 0)
        {
            isAscending = false;
        }
        if (Mathf.Abs(transform.position.x) > .01)
        {
            float vel = Mathf.Abs(transform.position.x) > 5 ? transform.position.x : transform.position.x*1.5f;
            rb.velocity = new Vector2(-vel, rb.velocity.y);
            //transform.position = new Vector2(Mathf.Lerp(transform.position.x, 0, 10f * Time.deltaTime), transform.position.y);
        }

    }

    private bool IsGrounded 
    {
        get
        {
            Vector3 pos = new Vector3(collide.bounds.center.x, collide.bounds.center.y - collide.bounds.extents.y, 0);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, isGroundedCheckDistance, obstaclesLayerMask);
            if (hit.collider != null && !hit.collider.CompareTag("Damager"))
            {
                return true;
            }
            Vector3 pos1 = new Vector3(collide.bounds.max.x, collide.bounds.center.y - collide.bounds.extents.y, 0);
            RaycastHit2D hit1 = Physics2D.Raycast(pos1, Vector2.down, isGroundedCheckDistance, obstaclesLayerMask);
            if (hit1.collider != null && !hit1.collider.CompareTag("Damager"))
            {
                return true;
            }
            Vector3 pos2 = new Vector3(collide.bounds.min.x, collide.bounds.center.y - collide.bounds.extents.y, 0);
            RaycastHit2D hit2 = Physics2D.Raycast(pos2, Vector2.down, isGroundedCheckDistance, obstaclesLayerMask);
            if (hit2.collider != null && !hit2.collider.CompareTag("Damager"))
            {
                return true;
            }
            return false;
        }
    }

}
