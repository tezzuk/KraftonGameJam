using UnityEngine;
using UnityEngine.InputSystem;

public class ThreadManager : MonoBehaviour
{
    [Header("Setup")]
    public Camera mainCamera;
    public LayerMask clickableLayer;

    // This will hold a permanent reference to the single Time Crystal in the scene.
    private TimeCrystal timeCrystal;

    void Start()
    {
        // At the start of the game, automatically find the Time Crystal object.
        timeCrystal = FindObjectOfType<TimeCrystal>();
        if (timeCrystal == null)
        {
            Debug.LogError("FATAL ERROR: No TimeCrystal object found in the scene! Rewind and thread systems will not work.");
        }
    }

    void Update()
    {
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        // --- Rewind Activation Logic (Works ANY time) ---
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (timeCrystal != null)
            {
                Debug.Log("Rewind key ('R') pressed. Activating the Time Crystal.");
                timeCrystal.ActivateRewind();
            }
            else
            {
                Debug.LogWarning("Rewind key ('R') pressed, but no Time Crystal was found in the scene.");
            }
        }

        // --- Connection Logic (Only works during Build Phase AND if BuildManager is idle) ---
        if (GameManager.instance.currentState != GameManager.GameState.BuildPhase) return;

        // --- THE FIX ---
        // If the BuildManager is busy placing or repositioning a tower, ignore any clicks for thread connections.
        if (BuildManager.instance.IsPlacingOrRepositioning) return;
        // --- END FIX ---

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ProcessConnectionClick();
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
            // We only check for towers since the crystal is found automatically.
            if (hit.collider.TryGetComponent<Turret>(out Turret turret))
            {
                if (timeCrystal != null)
                {
                    if (timeCrystal.connectedTurrets.Contains(turret))
                        timeCrystal.RemoveConnection(turret);
                    else
                        timeCrystal.AddConnection(turret);
                }
            }
            else if (hit.collider.TryGetComponent<Mortar>(out Mortar mortar))
            {
                if (timeCrystal != null)
                {
                    if (timeCrystal.connectedMortars.Contains(mortar))
                        timeCrystal.RemoveConnection(mortar);
                    else
                        timeCrystal.AddConnection(mortar);
                }
            }
             else if (hit.collider.TryGetComponent<Flamethrower>(out Flamethrower flamethrower))
            {
                if (timeCrystal != null)
                {
                    if (timeCrystal.connectedFlamethrowers.Contains(flamethrower))
                        timeCrystal.RemoveConnection(flamethrower);
                    else
                        timeCrystal.AddConnection(flamethrower);
                }
            }
        }
    }
}

