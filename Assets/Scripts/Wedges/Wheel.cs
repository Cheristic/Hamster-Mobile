using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float startingRotationSpeed;
    private float rotationSpeed;
    public float speedUpAmount;

    public Vector3 origin;
    private void Start()
    {
        origin = new Vector3(0, 0, 0);
        GameManager.newGame += OnNewGame;
        ScoreManager.SpeedUp += OnSpeedUp;
        rotationSpeed = startingRotationSpeed;
    }

    void FixedUpdate()
    {
        transform.RotateAround(origin, Vector3.forward, -rotationSpeed);
    }

    void OnNewGame()
    {
        rotationSpeed = startingRotationSpeed;
    }

    void OnSpeedUp()
    {
        rotationSpeed += speedUpAmount;
    }

}
