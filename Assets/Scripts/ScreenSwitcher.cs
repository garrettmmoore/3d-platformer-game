using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ScreenSwitcher : MonoBehaviour, IPointerEnterHandler
{
    public ScreenType desiredScreenType;
    private CanvasManager _canvasManager;
    private Button _menuButton;
    private PauseController _pauseController;

    private void Start()
    {
        _menuButton = GetComponent<Button>();
        _menuButton.onClick.AddListener(OnButtonClick);
        _canvasManager = Singleton<CanvasManager>.GetInstance();
        _pauseController = Singleton<PauseController>.GetInstance();
    }

    /// Select button on mouse hover
    /// <param name="eventData"> </param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    private void OnButtonClick()
    {
        if (desiredScreenType == ScreenType.GameUI && _pauseController.isPaused)
        {
            _pauseController.HandlePause();
        }
        else
        {
            _canvasManager.SwitchScreen(desiredScreenType);
        }
    }
}