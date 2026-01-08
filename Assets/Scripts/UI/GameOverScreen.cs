using TMPro;
using UnityEngine;

public class GameOverScreen : Singleton<GameOverScreen>
{
    [SerializeField] TMP_Text finalScoreText;
    [SerializeField] TMP_Text finalMoneyText;
    [SerializeField] TMP_Text highScoreText;

    int score, money, highScore;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetupGameOverScreen(int finalScore, int finalMoney, int bestScore)
    {
        score = finalScore;
        money = finalMoney;
        highScore = bestScore;

        finalScoreText.text = "Score: " + score.ToString();
        finalMoneyText.text = "Money: " + money.ToString();
        highScoreText.text = "High Score: " + highScore.ToString();

        Debug.Log("Game Over Screen Setup Complete");
    }

    public void ReturnToMenu()
    {
        GameMaster.Instance.ReturnToMainMenu();
    }

    public void Restart()
    {
        GameMaster.Instance.Restart();
    }
}
