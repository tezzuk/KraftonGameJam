using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public TowerData towerToBuild;

    void Start()
    {
        // Automatically add a listener to the button this script is on.
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        // Tell the BuildManager to select our tower.
        BuildManager.instance.SelectTowerToBuild(towerToBuild);
    }
}
