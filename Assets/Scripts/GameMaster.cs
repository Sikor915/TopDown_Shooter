using System.Collections.Generic;
using UnityEngine;

public class GameMaster : Singleton<GameMaster> {

    [SerializeField] PlayerSO playerSO;

    const string statsPath = "Assets/ScriptableObjects/Stats";

    int score = 0;
    int Score
    {
        get { return score; }
        set { score = value; UIController.Instance.SetScore(score); }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIController.Instance.SetScore(score);
        UIController.Instance.UpdateHealthText(playerSO.creatureSO.CurrentHealth, playerSO.creatureSO.MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() {
        EnemyController.OnEnemyKilled += AddScoreToCounter;
    }

    void OnDisable() {
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
        score += value;
        UIController.Instance.SetScore(score);
    }
}
