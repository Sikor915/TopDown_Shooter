using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScoreSO", menuName = "Scriptable Objects/ScoreSO", order = 1)]
public class ScoreSO : ScriptableObject
{
    [Header("Score and money values")]
    [SerializeField]
    int score;
    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    [SerializeField]
    int money;
    public int Money
    {
        get { return money; }
        set { money = value; }
    }

    [Header("Events")]
    public UnityEvent<int> onScoreChangedEvent = new UnityEvent<int>();
    public UnityEvent<int> onMoneyChangedEvent = new UnityEvent<int>();

    public void AddScore(int value)
    {
        Score += value;
        onScoreChangedEvent?.Invoke(Score);
    }

    public void AddMoney(int value)
    {
        Money += value;
        onMoneyChangedEvent?.Invoke(Money);
    }
}