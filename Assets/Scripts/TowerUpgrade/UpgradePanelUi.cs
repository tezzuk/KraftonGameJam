using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use TextMeshPro for better text rendering

public class UpgradePanelUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject uiPanel;
    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI statsText; // To show current and next level stats
    public Button upgradeButton;

    private TowerUpgrader targetTower;

    void Start()
    {
        uiPanel.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgradePressed);
    }

    public void Show(TowerUpgrader tower)
    {
        targetTower = tower;
        uiPanel.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        targetTower = null;
        uiPanel.SetActive(false);
    }

    private void UpdateUI()
    {
        if (targetTower == null) return;

        towerNameText.text = targetTower.towerData.towerName;

        TowerUpgradeLevel currentStats = targetTower.GetCurrentLevelStats();
        TowerUpgradeLevel nextStats = targetTower.GetNextLevelStats();

        // Display current stats
        string currentStatsInfo = $"Level {targetTower.currentUpgradeLevel + 1}\n" +
                                  $"Range: {currentStats.range}\n" +
                                  $"Fire Rate: {currentStats.fireRate}\n" +
                                  $"Damage: {currentStats.projectileDamage}";

        if (nextStats != null)
        {
            upgradeCostText.text = $"Upgrade Cost: {nextStats.upgradeCost}";
            upgradeButton.interactable = true;

            // Display comparison with next level stats
            string nextStatsInfo = $"\n\nNext Level\n" +
                                   $"Range: {nextStats.range} (+{nextStats.range - currentStats.range:F1})\n" +
                                   $"Fire Rate: {nextStats.fireRate} (+{nextStats.fireRate - currentStats.fireRate:F2})\n" +
                                   $"Damage: {nextStats.projectileDamage} (+{nextStats.projectileDamage - currentStats.projectileDamage})";

            statsText.text = currentStatsInfo + nextStatsInfo;
        }
        else
        {
            // Tower is at max level
            upgradeCostText.text = "Max Level";
            statsText.text = currentStatsInfo;
            upgradeButton.interactable = false;
        }
    }

    private void OnUpgradePressed()
    {
        if (targetTower != null)
        {
            targetTower.AttemptUpgrade();
            // After attempting an upgrade, refresh the UI to show new stats or max level
            UpdateUI();
        }
    }
}
