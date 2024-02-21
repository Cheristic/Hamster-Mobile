using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreOptionsUI : MonoBehaviour
{
    [SerializeField] Animator anim;
    public CustomObjectAnimation[] options;
    private CustomObjectAnimation moreOptionsVisibleAnim;

    bool expanded = false;

    private void Awake()
    {
        moreOptionsVisibleAnim = GetComponentInChildren<CustomObjectAnimation>();
    }

    public void HitMoreOptions()
    {
        if (expanded) CollapseOptions();
        else ExpandOptions();
    }

    // Collapses options into More Options (i) button or expands it out
    public void CollapseOptions()
    {
        if (!expanded) return;
        expanded = false;
        foreach (var item in options)
        {
            if (item.inProgress) item.StopAllCoroutines(); // Interrupt animation
            item.StartCoroutine(item.EasePosition(false));
        }


    }
    public void ExpandOptions()
    {
        if (expanded) return;
        expanded = true;
        foreach (var item in options)
        {
            if (item.inProgress) item.StopAllCoroutines(); // Interrupt animation
            item.StartCoroutine(item.EasePosition(true));
        }
    }

    //Slides all options below screen or brings em back up
    public void HideOptions()
    {
        CollapseOptions();
        if (moreOptionsVisibleAnim.inProgress) StopAllCoroutines();
        StartCoroutine(moreOptionsVisibleAnim.EasePosition(false));
    }

    public void ShowOptions()
    {
        if (moreOptionsVisibleAnim.inProgress) StopAllCoroutines();
        StartCoroutine(moreOptionsVisibleAnim.EasePosition(true));
    }
}
