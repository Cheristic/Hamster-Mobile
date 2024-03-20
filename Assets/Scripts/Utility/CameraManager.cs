using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// Resolution manager
public class CameraManager : MonoBehaviour
{
    public float sceneWidth;
    public float zoomedSceneHeight;
    Camera _camera;
    public AnimationCurve MoveCurve;
    [SerializeField] float zoomTime;
    private float baseView, zoomedView;
    [SerializeField] float zoomedXPos;

    [Header("Links")]
    [SerializeField] Transform TouchBackground;
    void Awake()
    {
        _camera = GetComponent<Camera>();

        MM_Settings.EnterSettingsMenu += ChangeCameraAngle; // Zoom in when entering settings

        float unitsPerPixel = sceneWidth / Screen.width;
        baseView = 0.5f * unitsPerPixel * Screen.height;
        zoomedView = zoomedSceneHeight * 0.5f;
        _camera.orthographicSize = baseView;


        // Place halfway between bottom of screen and bottom of wheel
        TouchBackground.position = new Vector2(0, (-9 - baseView)/2 + 0.5f);
    }

    IEnumerator zooming = null;
    private void ChangeCameraAngle(bool entering)
    {
        if (SettingsHandler.Main.zoomedMode) return; // Don't zoom out if on zoomedMode

        if (zooming != null) StopCoroutine(zooming);
        StartCoroutine(zooming = ZoomAnimation(entering));
    }

    // https://stackoverflow.com/questions/64638885/how-do-i-add-easing-to-a-scripted-transform-animation-in-unity
    float timeProgressed = 0f;
    private IEnumerator ZoomAnimation(bool zoomIn)
    {
        if (timeProgressed != 0f)
        {
            // Animation has been interrupted, give inverse
            timeProgressed = zoomTime - timeProgressed;
        }

        while (timeProgressed < zoomTime)
        {
            var percentCompleted = Mathf.Clamp01(timeProgressed / zoomTime);
            var scaledZoomAmount = MoveCurve.Evaluate(percentCompleted);

            if (zoomIn) //Go from base to zoomed
            {
                _camera.orthographicSize = Mathf.Lerp(baseView, zoomedView, scaledZoomAmount);
                transform.position = new Vector3(Mathf.Lerp(0f, zoomedXPos, scaledZoomAmount), 0, transform.position.z);
            } else // Go from zoomed to base
            {
                _camera.orthographicSize = Mathf.Lerp(zoomedView, baseView, scaledZoomAmount);
                transform.position = new Vector3(Mathf.Lerp(zoomedXPos, 0f, scaledZoomAmount), 0, transform.position.z);
            }

            yield return null;
            timeProgressed += Time.deltaTime;
        }

        // Finalize true values
        if (zoomIn) //Go from base to zoomed
        {
            _camera.orthographicSize = zoomedView;
            transform.position = new Vector3(zoomedXPos, 0, transform.position.z);
        }
        else // Go from zoomed to base
        {
            _camera.orthographicSize = baseView;
            transform.position = new Vector3(0, 0, transform.position.z);
        }
        // Reset checker
        zooming = null;
        timeProgressed = 0f;

    }
}
