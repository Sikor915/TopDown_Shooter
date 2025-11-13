using UnityEngine;
public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField]
    ScoreSO scoreSO;
    public ScoreSO ScoreSO => scoreSO;
    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        EnemyController.OnEnemyKilled += scoreSO.AddScore;
    }

    void OnDisable()
    {
        EnemyController.OnEnemyKilled -= scoreSO.AddScore;
    }

    public void AddMoney(int amount)
    {
        scoreSO.AddMoney(amount);
    }

    public int GetMoney()
    {
        return scoreSO.Money;
    }

}
