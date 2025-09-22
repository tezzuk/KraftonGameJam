using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    // Singleton pattern
    public static BuildManager instance;

    [Header("Setup")]
    public LayerMask invalidPlacementLayer;
    public LayerMask clickableLayer; // This layer is for all clickable objects (Towers, Crystal)

    // State variables
    private TowerData selectedTower;
    private GameObject towerPreviewInstance;
    private GameObject towerToReposition;
    private SpriteRenderer previewSpriteRenderer;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.BuildPhase)
        {
            CancelActions();
            return;
        }

        if (towerToReposition != null)
        {
            HandleRepositioning();
        }
        else if (towerPreviewInstance != null)
        {
            HandleNewTowerPlacement();
        }
        else
        {
            CheckForRepositionStart();
        }
    }
    
    void CheckForRepositionStart()
    {
        // Add a check to ensure both keyboard and mouse are present
        if (Keyboard.current == null || Mouse.current == null) return;

        // --- UPDATED LOGIC: Only check for a right mouse click ---
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Debug.Log("INPUT SUCCESS: Right Click detected. Raycasting for tower to reposition...");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, clickableLayer);

            if (hit.collider != null)
            {
                Debug.Log("RAYCAST SUCCESS: Hit object '" + hit.collider.name + "'. Checking if it's a tower...");
                if (hit.collider.GetComponent<Turret>() != null || hit.collider.GetComponent<Mortar>() != null)
                {
                    Debug.Log("OBJECT IDENTIFIED: It's a tower. Starting reposition.");
                    StartRepositioning(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("OBJECT IS NOT A TOWER. Reposition cancelled.");
                }
            }
            else
            {
                Debug.Log("RAYCAST FAILED: No object with a collider was found on the 'Clickable' layer.");
            }
        }
    }

    void StartRepositioning(GameObject tower)
    {
        towerToReposition = tower;
        previewSpriteRenderer = towerToReposition.GetComponentInChildren<SpriteRenderer>();

        if (tower.TryGetComponent<Turret>(out Turret turret)) turret.enabled = false;
        if (tower.TryGetComponent<Mortar>(out Mortar mortar)) mortar.enabled = false;
        RangeDetector detector = tower.GetComponentInChildren<RangeDetector>();
        if (detector != null) detector.enabled = false;

        Debug.Log("Repositioning " + tower.name);
    }

    void HandleRepositioning()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
        towerToReposition.transform.position = mousePosition;

        bool canPlace = IsValidPlacement(mousePosition);
        previewSpriteRenderer.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        
        // --- FIXED INPUT ---
        // Now uses the new Input System for consistency.
        if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
        {
            FinalizeRepositioning();
        }
    }

    void FinalizeRepositioning()
    {
        if (towerToReposition.TryGetComponent<Turret>(out Turret turret)) turret.enabled = true;
        if (towerToReposition.TryGetComponent<Mortar>(out Mortar mortar)) mortar.enabled = true;
        RangeDetector detector = towerToReposition.GetComponentInChildren<RangeDetector>();
        if (detector != null) detector.enabled = true;

        previewSpriteRenderer.color = Color.white;
        Debug.Log(towerToReposition.name + " has been repositioned.");

        towerToReposition = null;
        previewSpriteRenderer = null;
    }

    public void SelectTowerToBuild(TowerData tower)
    {
        if (GameManager.instance.currency < tower.buildCost)
        {
            Debug.Log("Not enough currency!");
            return;
        }
        
        CancelActions();

        selectedTower = tower;
        towerPreviewInstance = Instantiate(selectedTower.towerPrefab);
        previewSpriteRenderer = towerPreviewInstance.GetComponentInChildren<SpriteRenderer>();

        if (towerPreviewInstance.TryGetComponent<Turret>(out Turret turret)) turret.enabled = false;
        if (towerPreviewInstance.TryGetComponent<Mortar>(out Mortar mortar)) mortar.enabled = false;
        RangeDetector detector = towerPreviewInstance.GetComponentInChildren<RangeDetector>();
        if (detector != null) detector.enabled = false;
    }

    void HandleNewTowerPlacement()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
        towerPreviewInstance.transform.position = mousePosition;

        bool canPlace = IsValidPlacement(mousePosition);
        previewSpriteRenderer.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        
        // --- FIXED INPUT ---
        // Now uses the new Input System for consistency.
        if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceTower(towerPreviewInstance.transform.position);
        }
    }

    void PlaceTower(Vector3 position)
    {
        GameManager.instance.SpendCurrency(selectedTower.buildCost);
        Instantiate(selectedTower.towerPrefab, position, Quaternion.identity);
        
        CancelActions();
        Debug.Log("Tower Placed!");
    }

    bool IsValidPlacement(Vector3 position)
    {
        Collider2D overlap = Physics2D.OverlapCircle(position, 0.5f, invalidPlacementLayer);
        return overlap == null;
    }

    void CancelActions()
    {
        if (towerPreviewInstance != null)
        {
            Destroy(towerPreviewInstance);
            towerPreviewInstance = null;
        }
        if (towerToReposition != null)
        {
            FinalizeRepositioning();
        }
        selectedTower = null;
    }
}

