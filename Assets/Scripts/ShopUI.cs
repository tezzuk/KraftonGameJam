using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public TowerData towerToBuild;

    void Start()
    {
        // This line automatically finds the Button component on the same GameObject
        // and tells it to call our OnButtonClick method when it's clicked.
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        // --- UPDATED LOGIC ---
        // First, check if the player can actually afford the tower.
        if (GameManager.instance.currency >= towerToBuild.buildCost)
        {
            // If they can, play the successful click sound.
            SoundManager.instance.PlayButtonClickSound();
            // Then, tell the BuildManager to select the tower.
            BuildManager.instance.SelectTowerToBuild(towerToBuild);
        }
        else
        {
            // If they can't afford it, do nothing (or you could play an "error" sound here).
            Debug.Log("Not enough currency! Buy sound not played.");
        }
    }
}

