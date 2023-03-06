using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ScreenSwitcher : MonoBehaviour
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

    // Update is called once per frame
    private void OnButtonClick()
    {
        _canvasManager.SwitchScreen(desiredScreenType);
    }
}