using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class CustomObjectAnimation : MonoBehaviour
{
    internal bool inProgress = false;
    internal float timeProgressed = 0f;
    public float animTime;
    public Vector3 startPos;
    public Vector3 movedPos;
    public AnimationCurve ToMoveCurve;
    public AnimationCurve ToStartCurve;

    public IEnumerator EasePosition(bool goToMoved)
    {
        inProgress = true;

        if (goToMoved)
        {
            while (timeProgressed < animTime)
            {
                var percentCompleted = Mathf.Clamp01(timeProgressed / animTime);
                var curveAmount = ToMoveCurve.Evaluate(percentCompleted);
                gameObject.transform.localPosition = Vector2.Lerp(startPos, movedPos, curveAmount);

                yield return null;
                timeProgressed += Time.deltaTime;
            }
        } else
        {
            while (timeProgressed > 0)
            {
                var percentCompleted = Mathf.Clamp01(timeProgressed / animTime);
                var curveAmount = ToStartCurve.Evaluate(percentCompleted);
                gameObject.transform.localPosition = Vector2.Lerp(startPos, movedPos, curveAmount);

                yield return null;
                timeProgressed -= Time.deltaTime;
            }
        }
        
        gameObject.transform.localPosition = goToMoved ? movedPos : startPos;
        timeProgressed = goToMoved ? animTime : 0f;

        inProgress = false;
    }
}