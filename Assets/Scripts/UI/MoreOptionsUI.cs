using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreOptionsUI : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] CustomObjectAnimation[] options;
    [SerializeField] CustomObjectAnimation[] optionsFlavorText;
    private CustomObjectAnimation moreOptionsVisibleAnim;

    bool expanded = false;
    internal bool interactable = true;

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
        if (!expanded || !interactable) return;
        expanded = false;
        for (int i = 0; i < options.Length; i++)
        {
            var item = options[i];
            if (item.inProgress) item.StopAllCoroutines(); // Interrupt animation
            item.StartCoroutine(item.EasePosition(false));
            item = optionsFlavorText[i];
            if (item.inProgress) item.StopAllCoroutines();
            item.StartCoroutine(item.EaseOpacity(false));
        }


    }
    public void ExpandOptions()
    {
        if (expanded || !interactable) return;
        expanded = true;
        for (int i = 0; i < options.Length; i++)
        {
            var item = options[i];
            if (item.inProgress) item.StopAllCoroutines(); // Interrupt animation
            item.StartCoroutine(item.EasePosition(true));
            item = optionsFlavorText[i];
            if (item.inProgress) item.StopAllCoroutines();
            item.StartCoroutine(item.EaseOpacity(true));
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
