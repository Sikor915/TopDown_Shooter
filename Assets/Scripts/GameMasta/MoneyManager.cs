using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class MoneyManager : Singleton<MoneyManager>
{
    static readonly WaitForSeconds _waitForSeconds10 = new(10f);
    [SerializeField] ScoreSO scoreSO;
    public ScoreSO ScoreSO => scoreSO;
    
    [SerializeField] GameObject moneyPrefab;
    public GameObject MoneyPrefab => moneyPrefab;

    void Awake()
    {
        RemoveMoney(GetMoney());
        AddMoney(50);
        RemoveScore(GetScore());
        AddScore(100);
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        StartCoroutine(ScoreTimePenaltyCoroutine());
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
        AddScore(amount);
    }

    public void RemoveMoney(int amount)
    {
        scoreSO.AddMoney(-amount);
    }

    public int GetMoney()
    {
        return scoreSO.Money;
    }

    public void AddScore(int amount)
    {
        scoreSO.AddScore(amount);
    }

    public void RemoveScore(int amount)
    {
        scoreSO.AddScore(-amount);
    }

    public int GetScore()
    {
        return scoreSO.Score;
    }

    IEnumerator ScoreTimePenaltyCoroutine()
    {
        while (true)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "DemoScene")
            {
                yield break;
            }
            yield return _waitForSeconds10;
            if (ScoreSO.Score > 0)  RemoveScore(1);
        }
    }

}
