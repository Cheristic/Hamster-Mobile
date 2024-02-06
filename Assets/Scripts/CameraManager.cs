using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float sceneWidth;
    Camera _camera;
    Animator animator;
    void Start()
    {
        _camera = GetComponent<Camera>();
        animator = GetComponent<Animator>();
        OptionsMenu.EnterOptionsMenu += ChangeCameraAngle;
    }

    private void ChangeCameraAngle(bool zoom)
    {
        Debug.Log(animator);
        animator.SetTrigger("OptionsZoom");
    }

    // Update is called once per frame
    void Update()
    {
        float unitsPerPixel = sceneWidth / Screen.width;
        _camera.orthographicSize = 0.5f * unitsPerPixel * Screen.height;
    }
}
