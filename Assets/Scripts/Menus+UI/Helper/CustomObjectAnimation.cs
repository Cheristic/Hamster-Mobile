using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class CustomObjectAnimation : MonoBehaviour
{
    internal bool inProgress = false;
    internal float timeProgressed = 0f;

    public float animTime;
    public AnimationCurve ToCurve;
    public AnimationCurve FromCurve;

    [Header("Position")]
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 movedPos;

    [Header("Fade")]
    [SerializeField] CanvasGroup canvasGroup = null;

    public IEnumerator EasePosition(bool goToMoved, float speedMultiplier = 1)
    {
        inProgress = true;

        if (goToMoved)
        {
            while (timeProgressed < animTime)
            {
                var percentCompleted = Mathf.Clamp01(timeProgressed / animTime);
                var curveAmount = ToCurve.Evaluate(percentCompleted);
                gameObject.transform.localPosition = Vector2.Lerp(startPos, movedPos, curveAmount);

                yield return null;
                timeProgressed += Time.deltaTime * speedMultiplier;
            }
        } else
        {
            while (timeProgressed > 0)
            {
                var percentCompleted = Mathf.Clamp01(timeProgressed / animTime);
                var curveAmount = FromCurve.Evaluate(percentCompleted);
                gameObject.transform.localPosition = Vector2.Lerp(startPos, movedPos, curveAmount);

                yield return null;
                timeProgressed -= Time.deltaTime * speedMultiplier;
            }
        }
        
        gameObject.transform.localPosition = goToMoved ? movedPos : startPos;
        timeProgressed = goToMoved ? animTime : 0f;

        inProgress = false;
    }

    public IEnumerator EaseOpacity(bool fadeIn)
    {
        inProgress = true;

        if (fadeIn)
        {
            while (timeProgressed < animTime)
            {
                var percentCompleted = Mathf.Clamp01(timeProgressed / animTime);
                var curveAmount = ToCurve.Evaluate(percentCompleted);
                canvasGroup.alpha = Mathf.Lerp(0, 1, curveAmount);

                yield return null;
                timeProgressed += Time.deltaTime;
            }
        }
        else
        {
            while (timeProgressed > 0)
            {
                var percentCompleted = Mathf.Clamp01(timeProgressed / animTime);
                var curveAmount = FromCurve.Evaluate(percentCompleted);
                canvasGroup.alpha = Mathf.Lerp(0, 1, curveAmount);

                yield return null;
                timeProgressed -= Time.deltaTime;
            }
        }

        canvasGroup.alpha = fadeIn ? 1 : 0;
        timeProgressed = fadeIn ? animTime : 0f;

        inProgress = false;
    }
}