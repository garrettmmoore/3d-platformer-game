using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A script to pause the game using Unity events and time scale.
/// </summary>
public class PauseController : Singleton<PauseController>
{
    public UnityEvent gamePaused;
    public UnityEvent gameResumed;

    public bool isPaused;
    private CanvasManager _canvasManager;

    /// <inheritdoc />
    protected override void Awake()
    {
        base.Awake();
        _canvasManager = Singleton<CanvasManager>.GetInstance();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            HandlePause();
        }
    }

    /// <summary>
    /// A method to pause or resume the game and switch to the appropriate screen.
    /// </summary>
    public void HandlePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            gamePaused.Invoke();
            _canvasManager.SwitchScreen(ScreenType.PauseMenu);
        }
        else
        {
            Time.timeScale = 1;
            gameResumed.Invoke();
            _canvasManager.SwitchScreen(ScreenType.GameUI);
        }
    }
}