using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ScreenSwitcher : MonoBehaviour, IPointerEnterHandler
{
    public ScreenType desiredScreenType;
    private CanvasManager _canvasManager;
    private Button _menuButton;

    // Start is called before the first frame update
    private void Start()
    {
        _menuButton = GetComponent<Button>();
        _menuButton.onClick.AddListener(OnButtonClick);
        _canvasManager = Singleton<CanvasManager>.GetInstance();
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

    // Update is called once per frame
    private void OnButtonClick()
    {
        _canvasManager.SwitchScreen(desiredScreenType);
    }
}