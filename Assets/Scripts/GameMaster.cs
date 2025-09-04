using UnityEngine;

public class GameMaster : MonoBehaviour {

    [SerializeField] PlayerSO playerSO;

    int score = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIController.Instance.SetScore(score);
        UIController.Instance.UpdateHealthText(playerSO.CurrentHealth, playerSO.MaxHealth);
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

    private void AddScoreToCounter(int value) {
        score += value;
        UIController.Instance.SetScore(score);
    }
}
