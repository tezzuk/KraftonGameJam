using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    [Header("Setup")]
    public LayerMask invalidPlacementLayer;
    public LayerMask clickableLayer;
    public GameObject rangePreviewPrefab;
    public UpgradePanelUI upgradePanel;

    public bool IsPlacingOrRepositioning { get; private set; } = false;

    // State variables
    private TowerData selectedTowerToBuild;
    private GameObject towerPreviewInstance;
    private GameObject towerToReposition;
    private GameObject selectedTowerForUpgrade;
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

        if (EventSystem.current.IsPointerOverGameObject())
        {
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
            HandleTowerInteraction();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && towerPreviewInstance == null && towerToReposition == null && selectedTowerForUpgrade != null)
        {
            CancelActions();
        }
    }

    void HandleTowerInteraction()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, clickableLayer);

            if (hit.collider != null && hit.collider.GetComponent<TowerUpgrader>() != null)
            {
                SelectTowerForUpgrade(hit.collider.gameObject);
            }
            else
            {
                CancelActions();
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CheckForRepositionStart();
        }
    }

    public void SelectTowerToBuild(TowerData tower)
    {
        if (GameManager.instance.currency < tower.buildCost)
        {
            Debug.Log("Not enough currency!");
            return;
        }

        CancelActions();

        selectedTowerToBuild = tower;
        towerPreviewInstance = Instantiate(selectedTowerToBuild.towerPrefab);
        previewSpriteRenderer = towerPreviewInstance.GetComponentInChildren<SpriteRenderer>();
        CreateRangePreview(selectedTowerToBuild.upgradeLevels[0].range, towerPreviewInstance.transform);

        if (towerPreviewInstance.TryGetComponent<TowerUpgrader>(out TowerUpgrader upgrader)) upgrader.enabled = false;
        if (towerPreviewInstance.TryGetComponent<Turret>(out Turret turret)) turret.enabled = false;
        if (towerPreviewInstance.TryGetComponent<Mortar>(out Mortar mortar)) mortar.enabled = false;
        if (towerPreviewInstance.TryGetComponent<Flamethrower>(out Flamethrower flamethrower)) flamethrower.enabled = false;
        RangeDetector detector = towerPreviewInstance.GetComponentInChildren<RangeDetector>();
        if (detector != null) detector.enabled = false;
    }

    void PlaceTower(Vector3 position)
    {
        GameManager.instance.SpendCurrency(selectedTowerToBuild.buildCost);
        Destroy(towerPreviewInstance);

        GameObject newTower = Instantiate(selectedTowerToBuild.towerPrefab, position, Quaternion.identity);

        if (newTower.TryGetComponent<TowerUpgrader>(out TowerUpgrader upgrader))
        {
            upgrader.towerData = selectedTowerToBuild;
        }

        CancelActions();
    }

    public void SelectTowerForUpgrade(GameObject tower)
    {
        if (tower == selectedTowerForUpgrade) return;

        CancelActions();
        selectedTowerForUpgrade = tower;
        upgradePanel.Show(selectedTowerForUpgrade.GetComponent<TowerUpgrader>());
        CreateRangePreview(selectedTowerForUpgrade.GetComponent<TowerUpgrader>().GetCurrentLevelStats().range, selectedTowerForUpgrade.transform);
    }

    public void CancelActions()
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
        if (selectedTowerForUpgrade != null)
        {
            foreach (Transform child in selectedTowerForUpgrade.transform)
            {
                if (child.name.Contains("RangeIndicator"))
                {
                    Destroy(child.gameObject);
                }
            }
            selectedTowerForUpgrade = null;
        }

        selectedTowerToBuild = null;
        if (upgradePanel != null) upgradePanel.Hide();
    }

    // --- CORRECTED REPOSITIONING LOGIC ---

    void CheckForRepositionStart()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, clickableLayer);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<TowerUpgrader>(out TowerUpgrader upgrader))
            {
                // Get the range from the TowerUpgrader, not from the old towerData variable.
                float currentRange = upgrader.GetCurrentLevelStats().range;
                StartRepositioning(hit.collider.gameObject, currentRange);
            }
        }
    }

    void StartRepositioning(GameObject tower, float range)
    {
        CancelActions(); // Cancel any other action, like upgrade selection

        towerToReposition = tower;
        previewSpriteRenderer = towerToReposition.GetComponentInChildren<SpriteRenderer>();
        CreateRangePreview(range, towerToReposition.transform);

        // Disable all tower components during repositioning
        if (tower.TryGetComponent<TowerUpgrader>(out TowerUpgrader upgrader)) upgrader.enabled = false;
        if (tower.TryGetComponent<Turret>(out Turret turret)) turret.enabled = false;
        if (tower.TryGetComponent<Mortar>(out Mortar mortar)) mortar.enabled = false;
        if (tower.TryGetComponent<Flamethrower>(out Flamethrower flamethrower)) flamethrower.enabled = false;
        RangeDetector detector = tower.GetComponentInChildren<RangeDetector>();
        if (detector != null) detector.enabled = false;
    }

    void HandleRepositioning()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
        towerToReposition.transform.position = mousePosition;
        bool canPlace = IsValidPlacement(mousePosition);
        previewSpriteRenderer.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
        {
            FinalizeRepositioning();
        }
    }

    void FinalizeRepositioning()
    {
        // Remove the temporary range indicator visual
        foreach (Transform child in towerToReposition.transform)
        if (towerToReposition.transform.childCount > 0)
        {
            if (child.name.Contains("RangeIndicator"))
            {
                Destroy(child.gameObject);
            }
        }

        // Re-enable all the tower's components
        if (towerToReposition.TryGetComponent<TowerUpgrader>(out TowerUpgrader upgrader)) upgrader.enabled = true;
                SoundManager.instance.PlayTowerPlaceSound();

        if (towerToReposition.TryGetComponent<Turret>(out Turret turret)) turret.enabled = true;
        if (towerToReposition.TryGetComponent<Mortar>(out Mortar mortar)) mortar.enabled = true;
        if (towerToReposition.TryGetComponent<Flamethrower>(out Flamethrower flamethrower)) flamethrower.enabled = true;
        RangeDetector detector = towerToReposition.GetComponentInChildren<RangeDetector>();
        if (detector != null) detector.enabled = true;
        previewSpriteRenderer.color = Color.white;
        towerToReposition = null;
        previewSpriteRenderer = null;
        IsPlacingOrRepositioning = false;
    }
    void HandleNewTowerPlacement()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
        towerPreviewInstance.transform.position = mousePosition;
        bool canPlace = IsValidPlacement(mousePosition);
        previewSpriteRenderer.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceTower(towerPreviewInstance.transform.position);
        }
    }

    
    void CreateRangePreview(float range, Transform parent)
    {
        if (rangePreviewPrefab != null)
        {
            GameObject rangeVisual = Instantiate(rangePreviewPrefab, parent);
            rangeVisual.transform.localPosition = Vector3.zero;
            rangeVisual.name = "RangeIndicator(Clone)";
            float diameter = range * 2f;
            rangeVisual.transform.localScale = new Vector3(diameter, diameter, 1f);
        }
    }

    bool IsValidPlacement(Vector3 position)
    {
        Collider2D overlap = Physics2D.OverlapCircle(position, 0.5f, invalidPlacementLayer);
        return overlap == null;
    }
}

