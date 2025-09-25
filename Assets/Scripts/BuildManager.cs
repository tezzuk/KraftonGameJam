using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    [Header("Setup")]
    public LayerMask invalidPlacementLayer;
    public LayerMask clickableLayer;
    public GameObject rangePreviewPrefab;

    public bool IsPlacingOrRepositioning { get; private set; } = false;

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
        if (Mouse.current == null) return;
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, clickableLayer);
            if (hit.collider != null)
            {
                TowerData data = null;
                if (hit.collider.TryGetComponent<Turret>(out Turret turret)) data = turret.towerData;
                if (hit.collider.TryGetComponent<Mortar>(out Mortar mortar)) data = mortar.towerData;
                if (hit.collider.TryGetComponent<Flamethrower>(out Flamethrower flamethrower)) data = flamethrower.towerData;
                if (data != null)
                {
                    StartRepositioning(hit.collider.gameObject, data.range);
                }
            }
        }
    }

    void StartRepositioning(GameObject tower, float range)
    {
        IsPlacingOrRepositioning = true;
        towerToReposition = tower;
        previewSpriteRenderer = towerToReposition.GetComponentInChildren<SpriteRenderer>();
        CreateRangePreview(range, towerToReposition.transform);
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
        // --- ADD THIS LINE ---
        // Play the placement sound when the tower is placed back down.
        SoundManager.instance.PlayTowerPlaceSound();
        // --- END ADDITION ---

        if (towerToReposition.transform.childCount > 0)
        {
           foreach (Transform child in towerToReposition.transform) {
               if(child.name.Contains("RangeIndicator")){
                   Destroy(child.gameObject);
               }
           }
        }
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

    public void SelectTowerToBuild(TowerData tower)
    {
        if (GameManager.instance.currency < tower.buildCost) return;
        CancelActions();
        IsPlacingOrRepositioning = true;
        selectedTower = tower;
        towerPreviewInstance = Instantiate(selectedTower.towerPrefab);
        previewSpriteRenderer = towerPreviewInstance.GetComponentInChildren<SpriteRenderer>();
        CreateRangePreview(tower.range, towerPreviewInstance.transform);
        if (towerPreviewInstance.TryGetComponent<Turret>(out Turret turret)) turret.enabled = false;
        if (towerPreviewInstance.TryGetComponent<Mortar>(out Mortar mortar)) mortar.enabled = false;
        if (towerPreviewInstance.TryGetComponent<Flamethrower>(out Flamethrower flamethrower)) flamethrower.enabled = false;
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
        if (canPlace && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceTower(towerPreviewInstance.transform.position);
        }
    }

    void PlaceTower(Vector3 position)
    {
        SoundManager.instance.PlayTowerPlaceSound();
        GameManager.instance.SpendCurrency(selectedTower.buildCost);
        Destroy(towerPreviewInstance);
        GameObject newTower = Instantiate(selectedTower.towerPrefab, position, Quaternion.identity);
        if (newTower.TryGetComponent<Turret>(out Turret turret)) turret.towerData = selectedTower;
        if (newTower.TryGetComponent<Mortar>(out Mortar mortar)) mortar.towerData = selectedTower;
        if (newTower.TryGetComponent<Flamethrower>(out Flamethrower flamethrower)) flamethrower.towerData = selectedTower;
        CancelActions();
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
        IsPlacingOrRepositioning = false;
    }
}

