using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI diamondText;

    private void Start()
    {
        diamondText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateDiamondText(PlayerInventory playerInventory)
    {
        diamondText.text = playerInventory.NumberOfDiamonds.ToString();
    }
}
