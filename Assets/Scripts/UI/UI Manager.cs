using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using static SmokeTest.MainGui;


public class UIManager : MonoBehaviour
{
    public MenuGroup[] menuGroups;
    [HideInInspector] public List<Menu> menus;
    private Menu currMenu;

    [Serializable]
    public struct MenuGroup
    {
        public GameObject[] menus;
    }


    void Start()
    {
        menus = new()
        {
            new MainMenu(this),
            new InGameMenu(this),
            new GameOverMenu(this)
        };

        GameManager.gameStart += OnGameStart;
        GameManager.gameEnd += OnGameEnd;
        GameManager.newGame += OnNewGame;
        HamsterControls.TriggerHamsterReady += WaitForHamsterReady;
    }

    // Wait until hamster lands to transition into main scene
    private void WaitForHamsterReady()
    {
        ChangeMenu(0);
    }

    private void OnGameStart() => ChangeMenu(1);
    private void OnGameEnd() => ChangeMenu(2);
    private void OnNewGame() => CloseCurrentMenu(); // Close GameOver Screen

    public void ChangeMenu(int menuIndex)
    {
        if (currMenu != null)
        {
            currMenu.OnMenuExit();
        }
        currMenu = menus[menuIndex];
        currMenu.OnMenuEnter();
    }
    public void CloseCurrentMenu()
    {
        if (currMenu != null)
        {
            currMenu.OnMenuExit();
        }
    }
}

public abstract class Menu
{
    protected UIManager ui;
    public Menu(UIManager _ui)
    {
        ui = _ui;
    } 
    public void OnMenuEnter()
    {
        OnEnter();
    }
    protected virtual void OnEnter() { }
    public void OnMenuExit()
    {
        OnExit();
    }
    protected virtual void OnExit() { }
}

public class MainMenu : Menu
{
    private MainMenuUI mainMenuUI;
    public MainMenu(UIManager _ui) : base(_ui)
    {
        mainMenuUI = ui.menuGroups[0].menus[0].GetComponent<MainMenuUI>();
    }
    protected override void OnEnter()
    {
        mainMenuUI.EnterMainMenu();
    }

    protected override void OnExit()
    {
        mainMenuUI.ExitMainMenu();
    }
}

public class InGameMenu : Menu
{ 
    public InGameMenu(UIManager _ui) : base(_ui) { }
    protected override void OnEnter()
    {
        ui.menuGroups[1].menus[0].gameObject.SetActive(true);
    }

}

public class GameOverMenu : Menu
{
    public GameOverMenu(UIManager _ui) : base(_ui) { }
    protected override void OnEnter()
    {
        ui.menuGroups[2].menus[0].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuGroups[1].menus[0].gameObject.SetActive(false);
        ui.menuGroups[2].menus[0].gameObject.SetActive(false);
    }
}
