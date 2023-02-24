using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public UnityEvent<PlayerInventory> onDiamondCollected;
    public int NumberOfDiamonds { get; private set; }

    public void DiamondCollected()
    {
        NumberOfDiamonds++;
        onDiamondCollected.Invoke(this);
    }
}