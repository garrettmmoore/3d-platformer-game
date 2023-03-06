using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenController : MonoBehaviour
{
    public ScreenType screenType;
    public GameObject selectedButton;

    private void OnEnable()
    {
        HandleSelectedObject();
    }

    private void HandleSelectedObject()
    {
        // Clear selected object
        EventSystem.current.SetSelectedGameObject(null);

        // Set selected object
        EventSystem.current.SetSelectedGameObject(selectedButton);
    }
}