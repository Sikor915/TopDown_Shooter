using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatUpgradeSO", menuName = "Scriptable Objects/Upgrades/StatUpgradeSO")]
public class StatUpgradeSO : Upgrade
{
    public enum UpgradeType
    {
        Flat,
        Percent
    }

    public UpgradeType upgradeType;
    public List<CreatureSO> affectedCreatures = new();

    public override void ApplyUpgrade()
    {
        foreach (CreatureSO creature in affectedCreatures)
        {
            creature.ApplyStatUpgrade(this);
        }
    }

}