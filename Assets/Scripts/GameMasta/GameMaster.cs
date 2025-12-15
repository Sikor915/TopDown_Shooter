using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMaster : Singleton<GameMaster>
{

    [SerializeField] PlayerSO playerSO;
    [SerializeField] PlayerController playerController;

    public PlayerController PlayerController => playerController;

#if UNITY_EDITOR
    const string statsPath = "Assets/Resources/Player";
#else
    const string statsPath = "Player";
#endif

    void Start()
    {
        playerController.PlayerSO.creatureSO.onHealthChangedEvent?.Invoke(playerController.CurrentHealth, playerController.PlayerSO.creatureSO.MaxHealth);
    }

    void Awake()
    {
        playerController = PlayerController.Instance;
        playerController.GetComponent<PlayerInput>().enabled = true;
    }

    void OnEnable()
    {
        playerController.onPlayerDeath.AddListener(PlayerDied);
    }

    void OnDisable()
    {
        playerController.onPlayerDeath.RemoveListener(PlayerDied);
    }

    void OnApplicationQuit()
    {
        List<CreatureSO> creatures = HelperFunctions.GetScriptableObjectsOfType<CreatureSO>(statsPath);

        foreach (CreatureSO creature in creatures)
        {
            creature.Reset();
        }
    }

    public void PreparePlayerSpawn(Room startingRoom)
    {
        Vector2Int spawnPosition = new Vector2Int(
            startingRoom.Rect.x + startingRoom.Rect.width / 2,
            startingRoom.Rect.y + startingRoom.Rect.height / 2
        );
        playerController.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
    }

    public void ReturnToMainMenu()
    {
        Settings.Instance.ReturnToMainMenu();
    }

    public void Restart()
    {
        
    }

    void PlayerDied()
    {
        MusicManager.Instance.StopMusic();
        DeactivateAllEnemiesInScene();
        int score = MoneyManager.Instance.GetScore();
        int money = MoneyManager.Instance.GetMoney();
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScore = score;
        }
        GameOverScreen.Instance.SetupGameOverScreen(score, money, highScore);
    }

    void DeactivateAllEnemiesInScene()
    {
        EnemyController[] enemiesInScene = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (EnemyController enemy in enemiesInScene)
        {
            ObjectPooling.Instance.ReturnEnemyToPool(enemy);
        }
    }
}
