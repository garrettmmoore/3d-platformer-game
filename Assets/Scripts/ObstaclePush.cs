using UnityEngine;

public class ObstaclePush : MonoBehaviour
{
    [SerializeField]
    private float forceMagnitude;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null)
        {
            Vector3 position = transform.position;
            Vector3 forceDirection = hit.gameObject.transform.position - position;
            forceDirection.y = 0;
            forceDirection.Normalize();
            
            // Apply the force instantly (via Impulse) to the rigidBody
            rb.AddForceAtPosition(forceDirection * forceMagnitude, position, ForceMode.Impulse);
        }
    }
}
