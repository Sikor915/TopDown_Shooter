using UnityEngine;
public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField] ScoreSO scoreSO;
    public ScoreSO ScoreSO => scoreSO;
    
    [SerializeField] GameObject moneyPrefab;
    public GameObject MoneyPrefab => moneyPrefab;

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
        EnemyController.OnEnemyKilled += AddScore;
    }

    void OnDisable()
    {
        EnemyController.OnEnemyKilled -= AddScore;
    }

    void Start()
    {
        ScoreSO.onMoneyChangedEvent.Invoke(ScoreSO.Money);
        ScoreSO.onScoreChangedEvent.Invoke(ScoreSO.Score);
    }

    public void AddMoney(int amount)
    {
        scoreSO.AddMoney(amount);
    }

    public int GetMoney()
    {
        return scoreSO.Money;
    }

    public void AddScore(int amount)
    {
        scoreSO.AddScore(amount);
    }

    public int GetScore()
    {
        return scoreSO.Score;
    }

}
