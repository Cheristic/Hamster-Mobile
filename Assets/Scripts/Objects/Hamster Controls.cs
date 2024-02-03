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
    private void EndTouch(Vector2 pos, float time)
    {
        Vector2 dif = touchStartPos - pos;
        if (dif.magnitude < touchDistanceToFall)
        {
            TryJump();
        } else
        {
            TryFall();
        }
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyUp(KeyCode.Space))
        {
            TryJump();
        }
    }

    public bool TryJump()
    {
        if (IsGrounded && canJump)
        {
            rb.velocity = new Vector2(0, jumpHeight);
            canJump = false;
            return true;
        }
        return false;
    }

    private void TryFall()
    {
        Debug.Log("fall");
    }


    private void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            canJump = true;
        }
    }

    private bool IsGrounded 
    {
        get
        {
            Vector3 pos = new Vector3(collide.bounds.center.x, collide.bounds.center.y - collide.bounds.extents.y, 0);
            //Debug.DrawRay(pos, Vector2.down*(bufferDistance), Color.green);
            return Physics2D.Raycast(pos, Vector2.down, bufferDistance, obstaclesLayerMask);
        }
    }

}
