using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// Identifiers for UI screens.
public enum ScreenType
{
    MainMenu,
    GameUI,
    EndScreen
}

/// A script to manage and control all interactions with Canvas UI.
/// Derive from singleton to ensure this manager only exists once.
public class CanvasManager : Singleton<CanvasManager>
{
    private ScreenController _lastActiveScreen;

    // A list of scripts from the child objects (screens) of the canvas
    private List<ScreenController> _screenControllers;

    protected override void Awake()
    {
        base.Awake();
        _screenControllers = GetComponentsInChildren<ScreenController>().ToList();
        _screenControllers = GetComponentsInChildren<ScreenController>().ToList();
        _screenControllers.ForEach(x => x.gameObject.SetActive(false));

        // All of our screens are active so we want to deactivate them all except for the main menu
        SwitchScreen(ScreenType.MainMenu);
    }

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