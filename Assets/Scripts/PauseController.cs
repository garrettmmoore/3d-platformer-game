using UnityEngine;
using UnityEngine.Events;

public class PauseController : MonoBehaviour
{
    public UnityEvent gamePaused;
    public UnityEvent gameResumed;

    private bool _isPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                Time.timeScale = 0;
                Singleton<CanvasManager>.GetInstance().SwitchScreen(ScreenType.EndScreen);
                gamePaused.Invoke();
            }
            else
            {
                Time.timeScale = 1;
                Singleton<CanvasManager>.GetInstance().SwitchScreen(ScreenType.GameUI);
                gameResumed.Invoke();
            }
        }
    }
}