using UnityEngine;
using UnityEngine.InputSystem;

public class ThreadManager : MonoBehaviour
{
    [Header("Setup")]
    public Camera mainCamera;
    public LayerMask clickableLayer;

    private TimeCrystal selectedCrystal;

    void Update()
    {
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        // --- Connection Logic (Only works during Build Phase) ---
        // This part allows the player to connect/disconnect threads.
        if (GameManager.instance.currentState == GameManager.GameState.BuildPhase)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                ProcessConnectionClick();
            }
        }

        // --- Rewind Activation Logic (Works ANY time) ---
        // This part listens for the 'R' key press to activate the rewind.
        // It is outside the build phase check so it works during a wave.
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (selectedCrystal != null)
            {
                Debug.Log("Rewind key ('R') pressed. Activating crystal.");
                selectedCrystal.ActivateRewind();
            }
            else
            {
                Debug.Log("Rewind key ('R') pressed, but no Time Crystal is selected.");
            }
        }
    }

    private void ProcessConnectionClick()
    {
        if (mainCamera == null)
        {
            Debug.LogError("FATAL ERROR: The 'Main Camera' slot is NOT ASSIGNED on the ThreadManager script!");
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero, Mathf.Infinity, clickableLayer);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<TimeCrystal>(out TimeCrystal crystal))
            {
                selectedCrystal = crystal;
                Debug.Log("Time Crystal selected.");
            }
            else if (hit.collider.TryGetComponent<Turret>(out Turret turret))
            {
                if (selectedCrystal != null)
                {
                    if (selectedCrystal.connectedTurrets.Contains(turret))
                    {
                        selectedCrystal.RemoveConnection(turret);
                    }
                    else
                    {
                        selectedCrystal.AddConnection(turret);
                    }
                }
            }
            else if (hit.collider.TryGetComponent<Mortar>(out Mortar mortar))
            {
                if (selectedCrystal != null)
                {
                    if (selectedCrystal.connectedMortars.Contains(mortar))
                    {
                        selectedCrystal.RemoveConnection(mortar);
                    }
                    else
                    {
                        selectedCrystal.AddConnection(mortar);
                    }
                }
            }
        }
        else
        {
            selectedCrystal = null;
        }
    }
}

