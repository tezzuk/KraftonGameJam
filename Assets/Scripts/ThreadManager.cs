using UnityEngine;
using UnityEngine.InputSystem;

public class ThreadManager : MonoBehaviour
{
    [Header("Setup")]
    public Camera mainCamera;
    public LayerMask clickableLayer;

    // This will now hold a permanent reference to the single Time Crystal in the scene.
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
        // --- Connection Logic (Only works during Build Phase) ---
        if (GameManager.instance.currentState == GameManager.GameState.BuildPhase)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                ProcessConnectionClick();
            }
        }

        // --- Rewind Activation Logic (Works ANY time) ---
        // This now checks for the automatically found crystal.
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
            // We no longer need to select the crystal, so we only check for towers.
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

