using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI diamondText;

    private void Start()
    {
        diamondText = GetComponent<TextMeshProUGUI>();
    }
    
    /// Update the diamond text with the current number of diamonds in the player's inventory.
    /// <param name="playerInventory"></param>
    public void UpdateDiamondText(PlayerInventory playerInventory)
    {
        diamondText.text = playerInventory.NumberOfDiamonds.ToString();
    }
}
