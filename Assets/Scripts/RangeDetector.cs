using UnityEngine;

// This script can be used by any tower. It finds the parent tower
// and sends it a message when an enemy enters or leaves its range.
public class RangeDetector : MonoBehaviour
{
    // This function is called when a 2D collider enters the trigger zone.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Send a message up to the parent GameObject (the tower).
        // This will call the "OnEnemyEnteredRange" method on any script attached to the parent.
        transform.parent.SendMessage("OnEnemyEnteredRange", other.gameObject, SendMessageOptions.DontRequireReceiver);
    }

    // This function is called when a 2D collider leaves the trigger zone.
    void OnTriggerExit2D(Collider2D other)
    {
        // Send a message up to the parent GameObject to call the "OnEnemyExitedRange" method.
        transform.parent.SendMessage("OnEnemyExitedRange", other.gameObject, SendMessageOptions.DontRequireReceiver);
    }
}

