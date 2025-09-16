using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIManager : Singleton<UpgradeUIManager>
{
    [SerializeField] GameObject upgradePanelPrefab;

    public Action onUpgradeChosen;

    void Start()
    {
        onUpgradeChosen += () => { foreach (Transform child in transform) { Destroy(child.gameObject); } };
    } 

    public void CreateUpgradeCards(List<StatUpgradeSO> upgradesRandomized)
    {
        foreach (StatUpgradeSO upgrade in upgradesRandomized)
        {
            UpgradeCard card = Instantiate(upgradePanelPrefab, transform).GetComponent<UpgradeCard>();
            StringBuilder sb = new StringBuilder();
            foreach (var stat in upgrade.affectedStats)
            {
                sb.AppendLine(stat.Key.ToString() + ": +" + stat.Value.ToString() + (upgrade.upgradeType == StatUpgradeSO.UpgradeType.Percent ? "%" : ""));
            }
            card.SetCard(upgrade.upgradeName, upgrade.upgradeDescription, sb.ToString(), upgrade);
        }
    }
}