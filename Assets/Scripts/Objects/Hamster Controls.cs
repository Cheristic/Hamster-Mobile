using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HamsterControls : MonoBehaviour
{
    [Header("Hamster Values")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float bufferDistance = 0.2f;
    [SerializeField] LayerMask obstaclesLayerMask;

    [Header("Controls")]
    [SerializeField] private float touchDistanceToFall = 30f;
    [SerializeField] private float jumpLimitTime = 1f;
    [SerializeField] private float jumpBoost = 200f;

    Rigidbody2D rb;
    Collider2D collide;
    bool canJump;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collide = GetComponent<Collider2D>();
        canJump = true;
        GameManager.gameStart += OnGameStart;
        GameManager.gameEnd += OnGameEnd;
        GameManager.newGame += OnNewGame;
    }

    private void OnGameStart()
    {
        InputManager.OnTouchStart += StartTouch;
        InputManager.OnTouchEnd += EndTouch;
    }
    private void OnGameEnd()
    {
        InputManager.OnTouchStart -= StartTouch;
        InputManager.OnTouchEnd -= EndTouch;
        gameObject.SetActive(false);
    }
    private void OnNewGame()
    {
        transform.position = new Vector2(0, 7);
        gameObject.SetActive(true); 
    }

    private Vector2 touchStartPos;
    private void StartTouch(Vector2 pos, float time)
    {
        touchStartPos = pos;
    }
    private void Update()
    {
        if (GameManager.Main.gameRunning && InputManager.Main.input.Touch.TouchPress.inProgress)
        {
            StartCoroutine(TapJumpDelay());
        }
    }
    private IEnumerator TapJumpDelay()
    {
        yield return new WaitForSeconds(0.05f);
        if (GameManager.Main.gameRunning && InputManager.Main.input.Touch.TouchPress.inProgress)
        {
            TryJump();
        }
    }

    private void EndTouch(Vector2 pos, float time)
    {
        Vector2 dif = touchStartPos - pos;
        if (dif.magnitude >= touchDistanceToFall)
        {
            TryFall();
        } else
        {
            TryJump();
        }
    }



    public bool TryJump()
    {
        if (canJump && IsGrounded)
        {
            canJump = false;
            rb.velocity = new Vector2(0, jumpHeight);
            StartCoroutine(Jump());
            return true;
        }
        return false;
    }
    bool addJumpBoost;
    private IEnumerator Jump()
    {
        float jumpTime = 0f;
        // while moving up && finger is tapped && less than limit
        while(!canJump && InputManager.Main.input.Touch.TouchPress.inProgress && jumpTime < jumpLimitTime)
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
        rb.velocity = new Vector2(0, -jumpHeight);
        HamsterFall?.Invoke();
    }


    private void FixedUpdate()
    {
        if (addJumpBoost) rb.AddForce(new Vector2(0, jumpBoost));
        if (rb.velocity.y <= 0)
        {
            canJump = true;
        }
    }

    private bool IsGrounded 
    {
        get
        {
            Vector3 pos = new Vector3(collide.bounds.center.x, collide.bounds.center.y - collide.bounds.extents.y, 0);
            //Debug.DrawRay(pos, Vector2.down*(bufferDistance), Color.green
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, bufferDistance, obstaclesLayerMask);
            if (hit.collider != null && !hit.collider.CompareTag("Damager"))
            {
                return true;
            }
            return false;
        }
    }

}
