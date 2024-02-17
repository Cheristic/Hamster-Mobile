using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewScoreText : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float distance = 0.002f;
    Animator animator;
    [SerializeField] CanvasGroup group;
    float resetYPos;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        GameManager.newGame += OnNewGame;
        resetYPos = transform.position.y;
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.time * speed);
        transform.position = new Vector2(transform.position.x, transform.position.y + (y * distance));
    }

    private void OnNewGame()
    {
        transform.position = new Vector2(transform.position.x, resetYPos);
    }

    public void ShowNewText()
    {
        animator.SetTrigger("Fade");
    }
}
