using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{

    public MenuGroup[] menuGroups;
    [HideInInspector] public List<MM_Menu> menus;
    private MM_Menu currMenu;
    [Serializable]
    public struct MenuGroup
    {
        public GameObject[] menus;
    }

    internal MoreOptionsUI moreOptionsUI;

    void Start()
    {
        menus = new()
        {
            new MM_Hub(),
            new MM_Settings(),
            new MM_Credits()
        };
        moreOptionsUI = menuGroups[0].menus[0].GetComponent<MoreOptionsUI>();
    }

    public void EnterMainMenu()
    {
        ChangeMenu(0);
        moreOptionsUI.ShowOptions();
    }

    public void ExitMainMenu()
    {
        moreOptionsUI.HideOptions();
        currMenu.OnMenuExit();
        currMenu = null;
    }

    public void ChangeMenu(int menuIndex)
    {
        if (currMenu != null)
        {
            currMenu.OnMenuExit();
        }
        currMenu = menus[menuIndex];
        currMenu.OnMenuEnter(this);
    }
}

public abstract class MM_Menu
{
    protected MainMenuUI ui;
    public void OnMenuEnter(MainMenuUI _ui)
    {
        ui = _ui;
        OnEnter();
    }
    protected virtual void OnEnter() { }
    public void OnMenuExit()
    {
        OnExit();
    }
    protected virtual void OnExit() { }
}

public class MM_Hub : MM_Menu
{
    protected override void OnEnter()
    {
        ui.menuGroups[0].menus[1].SetActive(true); // Enable tap
        ui.menuGroups[0].menus[2].SetActive(true); // Enable World Space UI
    }

    protected override void OnExit()
    {
        ui.menuGroups[0].menus[1].SetActive(false);
        ui.menuGroups[0].menus[2].SetActive(false);
    }
}

public class MM_Settings : MM_Menu
{
    public static event Action<bool> EnterSettingsMenu;
    protected override void OnEnter()
    {
        EnterSettingsMenu?.Invoke(true);
        ui.menuGroups[1].menus[0].SetActive(true);
        ui.menuGroups[1].menus[1].SetActive(true);
        ui.moreOptionsUI.CollapseOptions();
    }

    protected override void OnExit()
    {
        EnterSettingsMenu?.Invoke(false);
        ui.menuGroups[1].menus[0].SetActive(false);
        ui.menuGroups[1].menus[1].SetActive(false);
    }
}

public class MM_Credits : MM_Menu
{
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }
}
