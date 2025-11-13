using System.Collections.Generic;
using UnityEngine;

public class PerkShop : MonoBehaviour, IShop
{
    
    const string upgradesPath = "Assets/ScriptableObjects/Upgrades";

    public void OpenShop()
    {
        RandomizeUpgradesToShow();
    }

    public void CloseShop()
    {
        Debug.Log("Perk Shop closed");
    }
    
    void RandomizeUpgradesToShow()
    {
        List<StatUpgradeSO> allUpgrades = HelperFunctions.GetScriptableObjectsOfType<StatUpgradeSO>(upgradesPath);
        List<StatUpgradeSO> upgradesRandomized = new();
        int upgradesToShow = 3;
        while (upgradesRandomized.Count < upgradesToShow)
        {
            int randomIndex = Random.Range(0, allUpgrades.Count);
            StatUpgradeSO randomUpgrade = allUpgrades[randomIndex];
            if (!upgradesRandomized.Contains(randomUpgrade))
            {
                upgradesRandomized.Add(randomUpgrade);
            }
        }
        UpgradeUIManager.Instance.CreateUpgradeCards(upgradesRandomized);
    }
}
