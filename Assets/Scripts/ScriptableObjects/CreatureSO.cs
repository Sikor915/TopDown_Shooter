using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CreatureSO", menuName = "Scriptable Objects/CreatureSO")]
public class CreatureSO : ScriptableObject
{
    [Header("Events")]
    public UnityEvent<float, float> onHealthChangedEvent;
    public UnityEvent onStatsChangedEvent;

    [Header("Stats/HP")]
    [SerializeField] float maxHealth = 100;
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; onHealthChangedEvent?.Invoke(currentHealth, maxHealth); }
    }
    [SerializeField] float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; onHealthChangedEvent?.Invoke(currentHealth, maxHealth); }
    }

    public List<StatInfo> instanceStats = new();
    public List<StatInfo> statInfo = new();
    public List<StatUpgradeSO> appliedUpgrades = new();
    void OnEnable()
    {
        onHealthChangedEvent ??= new UnityEvent<float, float>();
        currentHealth = maxHealth;
    }

    public void Reset()
    {
        currentHealth = maxHealth;
        ResetUpgrades();
    }

    public void DeductHealth(int damage)
    {
        CurrentHealth -= damage;
        onHealthChangedEvent?.Invoke(currentHealth, maxHealth);
    }

    public void ApplyStatUpgrade(StatUpgradeSO upgrades)
    {
        appliedUpgrades.Add(upgrades);
        onStatsChangedEvent?.Invoke();
    }

    public float GetStat(StatInfo.Stat stat)
    {
        if (instanceStats.Find(s => s.statType == stat) is StatInfo instanceStat)
        {
            return GetUpgradedStat(stat, instanceStat.statValue);
        }
        else if (statInfo.Find(s => s.statType == stat) is StatInfo baseStat)
        {
            return GetUpgradedStat(stat, baseStat.statValue);
        }
        return -Mathf.Infinity;
    }

    void ResetUpgrades()
    {
        appliedUpgrades.Clear();
        onStatsChangedEvent?.Invoke();
    }

    float GetUpgradedStat(StatInfo.Stat stat, float value)
    {
        foreach (StatUpgradeSO upgrades in appliedUpgrades)
        {
            if (!upgrades.affectedStats.ContainsKey(stat)) continue;
            if (upgrades.upgradeType == StatUpgradeSO.UpgradeType.Flat)
            {
                value += upgrades.affectedStats[stat];
            }
            else if (upgrades.upgradeType == StatUpgradeSO.UpgradeType.Percent)
            {
                value *= (1 + upgrades.affectedStats[stat]);
            }
        }
        return value;
    }
}
