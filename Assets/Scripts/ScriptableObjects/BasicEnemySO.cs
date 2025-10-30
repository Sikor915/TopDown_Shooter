using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "BasicEnemySO", menuName = "Scriptable Objects/BasicEnemySO")]
public class BasicEnemySO : ScriptableObject
{
    [Header("Events")]
    public UnityEvent onHealthChangedEvent;
    public UnityEvent onStatsChangedEvent;

    [Header("Stats")]
    [SerializeField] float maxHealth = 100;
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; onHealthChangedEvent?.Invoke(); }
    }
    [SerializeField] float damage;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    [SerializeField] int enemyScore;
    public int EnemyScore
    {
        get { return enemyScore; }
        set { enemyScore = value; }
    }
    [SerializeField] float moveSpeed;
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    [SerializeField] float patrolSpeed;
    public float PatrolSpeed
    {
        get { return patrolSpeed; }
        set { patrolSpeed = value; }
    }

    public List<StatInfo> statInfo = new();
    public List<StatUpgradeSO> appliedUpgrades = new();

    public void Reset()
    {
        ResetUpgrades();
    }

    public void DeductHealth(int damage)
    {
        onHealthChangedEvent?.Invoke();
    }

    public void ApplyStatUpgrade(StatUpgradeSO upgrades)
    {
        appliedUpgrades.Add(upgrades);
        onStatsChangedEvent?.Invoke();
    }

    public float GetStat(StatInfo.Stat stat)
    {
        if (statInfo.Find(s => s.statType == stat) is StatInfo baseStat)
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
