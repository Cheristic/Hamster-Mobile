using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float rotationSpeed;

    public Vector3 origin;
    private void Start()
    {
        origin = new Vector3(0, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(origin, Vector3.forward, -rotationSpeed);
    }
}
