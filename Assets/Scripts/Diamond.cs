using UnityEngine;

public class Diamond : MonoBehaviour
{
    // Called whenever something collides with a diamond
    private void OnTriggerEnter(Collider other)
    {
        var playerInventory = other.GetComponent<PlayerInventory>();

        if (playerInventory)
        {
            playerInventory.DiamondCollected();
            gameObject.SetActive(false);
        }

    }
}
