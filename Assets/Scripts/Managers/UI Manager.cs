using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;


public class UIManager : MonoBehaviour
{
    public MenuGroup[] menuGroups;
    [HideInInspector] public List<Menu> menus;
    private Menu currMenu;

    [Serializable]
    public struct MenuGroup
    {
        public Canvas[] menus;
    }


    void Start()
    {
        menus = new()
        {
            new MainMenu(),
            new OptionsMenu(),
            new CreditsMenu(),
            new InGameMenu(),
            new GameOverMenu()
        };

        ChangeMenu(0);
        GameManager.gameStart += OnGameStart;
        GameManager.gameEnd += OnGameEnd;
        GameManager.newGame += OnNewGame;
    }
    private void OnGameStart() => ChangeMenu(3);
    private void OnGameEnd() => ChangeMenu(4);

    public void ChangeMenu(int menuIndex)
    {
        if (currMenu != null)
        {
            currMenu.OnMenuExit();
        }
        currMenu = menus[menuIndex];
        currMenu.OnMenuEnter(this);
    }
    public void CloseMenu(int menuIndex)
    {
        if (currMenu != null)
        {
            currMenu.OnMenuExit();
        }
    }

    // Wait until hamster lands to allow tapping
    private void OnNewGame() => StartCoroutine(WaitForHamster());
    private IEnumerator WaitForHamster()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => GameManager.Main.hamster.transform.position.y < -8.37f);
        ChangeMenu(0);
    }
}

public abstract class Menu
{
    protected UIManager ui;
    public void OnMenuEnter(UIManager _ui)
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

public class MainMenu : Menu
{
    protected override void OnEnter()
    {
        ui.menuGroups[0].menus[0].gameObject.SetActive(true);
        ui.menuGroups[0].menus[1].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuGroups[0].menus[0].gameObject.SetActive(false);
        ui.menuGroups[0].menus[1].gameObject.SetActive(false);
    }
}

public class OptionsMenu : Menu
{
    public static event Action<bool> EnterOptionsMenu;
    protected override void OnEnter()
    {
        ui.menuGroups[1].menus[0].gameObject.SetActive(true);
        ui.menuGroups[1].menus[1].gameObject.SetActive(true);
        EnterOptionsMenu.Invoke(true);
    }

    protected override void OnExit()
    {
        ui.menuGroups[1].menus[0].gameObject.SetActive(false);
        ui.menuGroups[1].menus[1].gameObject.SetActive(false);
        EnterOptionsMenu.Invoke(false);
    }
}

public class CreditsMenu : Menu
{
    protected override void OnEnter()
    {
        ui.menuGroups[2].menus[0].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuGroups[2].menus[0].gameObject.SetActive(false);
    }
}

public class InGameMenu : Menu
{
    protected override void OnEnter()
    {
        ui.menuGroups[3].menus[0].gameObject.SetActive(true);
        ui.menuGroups[5].menus[0].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuGroups[3].menus[0].gameObject.SetActive(false);
    }
}

public class GameOverMenu : Menu
{
    protected override void OnEnter()
    {
        ui.menuGroups[4].menus[0].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuGroups[4].menus[0].gameObject.SetActive(false);
        ui.menuGroups[5].menus[0].gameObject.SetActive(false);
    }
}
