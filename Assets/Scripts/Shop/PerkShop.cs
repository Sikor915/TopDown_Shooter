using System.Collections.Generic;
using UnityEngine;

public class PerkShop : Singleton<PerkShop>, IShop
{
    [SerializeField] int perksToShow = 3;
    const string upgradesPath = "Assets/ScriptableObjects/Upgrades";

    public bool isOpen = false;

    public void OpenShop()
    {
        if(MoneyManager.Instance.GetMoney() < 5)
        {
            Debug.Log("Not enough money to open Perk Shop");
            return;
        }
        MoneyManager.Instance.AddMoney(-5);
        RandomizeUpgradesToShow();
        isOpen = true;
    }

    public void CloseShop()
    {
        Debug.Log("Perk Shop closed");
        isOpen = false;
    }
    
    void RandomizeUpgradesToShow()
    {
        List<StatUpgradeSO> allUpgrades = HelperFunctions.GetScriptableObjectsOfType<StatUpgradeSO>(upgradesPath);
        List<StatUpgradeSO> upgradesRandomized = new();

        while (upgradesRandomized.Count < perksToShow)
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
