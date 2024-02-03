using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas[] menuCanvases;
    [HideInInspector] public List<Menu> menus;
    private Menu currMenu;

    // Start is called before the first frame update
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
        ui.menuCanvases[0].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuCanvases[0].gameObject.SetActive(false);
    }
}

public class OptionsMenu : Menu
{
    protected override void OnEnter()
    {
        ui.menuCanvases[1].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuCanvases[1].gameObject.SetActive(false);
    }
}

public class CreditsMenu : Menu
{
    protected override void OnEnter()
    {
        ui.menuCanvases[2].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuCanvases[2].gameObject.SetActive(false);
    }
}

public class InGameMenu : Menu
{
    protected override void OnEnter()
    {
        ui.menuCanvases[3].gameObject.SetActive(true);
        ui.menuCanvases[5].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuCanvases[3].gameObject.SetActive(false);
    }
}

public class GameOverMenu : Menu
{
    protected override void OnEnter()
    {
        ui.menuCanvases[4].gameObject.SetActive(true);
    }

    protected override void OnExit()
    {
        ui.menuCanvases[4].gameObject.SetActive(false);
        ui.menuCanvases[5].gameObject.SetActive(false);
    }
}
