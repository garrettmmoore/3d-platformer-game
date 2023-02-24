using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI _diamondText;

    private void Start()
    {
        _diamondText = GetComponent<TextMeshProUGUI>();
    }

    /// Update the diamond text with the current number of diamonds in the player's inventory.
    /// <param name="playerInventory"> </param>
    public void UpdateDiamondText(PlayerInventory playerInventory)
    {
        _diamondText.text = playerInventory.NumberOfDiamonds.ToString();
    }
}