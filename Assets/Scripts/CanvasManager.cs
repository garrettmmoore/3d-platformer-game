using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// Identifiers for UI screens.
public enum ScreenType
{
    EndScreen,
    GameUI,
    MainMenu,
    PauseMenu
}

/// A script to manage and control all interactions with Canvas UI.
/// Derive from singleton to ensure this manager only exists once.
public class CanvasManager : Singleton<CanvasManager>
{
    private ScreenController _lastActiveScreen;

    // A list of scripts from the child objects (screens) of the canvas
    private List<ScreenController> _screenControllers;

    // Deactivate all screens except for the main menu
    protected override void Awake()
    {
        base.Awake();
        _screenControllers = GetComponentsInChildren<ScreenController>(true).ToList();
        _screenControllers.ForEach(x => x.gameObject.SetActive(false));
        SwitchScreen(ScreenType.MainMenu);
    }

    /// Switch to the specified UI screen.
    /// <param name="screenType"> The type of screen to switch to. </param>
    public void SwitchScreen(ScreenType screenType)
    {
        if (_lastActiveScreen != null)
        {
            _lastActiveScreen.gameObject.SetActive(false);
        }

        ScreenController desiredScreen = _screenControllers.Find(x => x.screenType == screenType);

        if (desiredScreen != null)
        {
            desiredScreen.gameObject.SetActive(true);
            _lastActiveScreen = desiredScreen;
        }
        else
        {
            Debug.LogWarning("No screen with type " + screenType + " was found.");
        }
    }
}