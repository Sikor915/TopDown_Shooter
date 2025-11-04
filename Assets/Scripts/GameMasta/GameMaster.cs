using System.Collections.Generic;
using UnityEngine;

public class GameMaster : Singleton<GameMaster>
{

    [SerializeField] PlayerSO playerSO;
    [SerializeField] PlayerController playerController;

    public PlayerController PlayerController => playerController;

    const string statsPath = "Assets/ScriptableObjects/Player";
    const string upgradesPath = "Assets/ScriptableObjects/Upgrades";

    int score = 0;
    public int Score
    {
        get { return score; }
        set { score = value; UIController.Instance.SetScore(score); }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIController.Instance.SetScore(score);
        UIController.Instance.UpdateHealthText(playerController.CurrentHealth, playerSO.creatureSO.MaxHealth);
    }

    void OnEnable()
    {
        EnemyController.OnEnemyKilled += AddScoreToCounter;
    }

    void OnDisable()
    {
        EnemyController.OnEnemyKilled -= AddScoreToCounter;
    }

    void OnApplicationQuit()
    {
        List<CreatureSO> creatures = HelperFunctions.GetScriptableObjectsOfType<CreatureSO>(statsPath);

        foreach (CreatureSO creature in creatures)
        {
            creature.Reset();
        }
    }

    void AddScoreToCounter(int value)
    {
        Score += value;
        if (Score % 5 == 0 && Score != 0)
        {
            RandomizeUpgradesToShow();
        }
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
