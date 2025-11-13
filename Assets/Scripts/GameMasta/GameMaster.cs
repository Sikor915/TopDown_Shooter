using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMaster : Singleton<GameMaster>
{

    [SerializeField] PlayerSO playerSO;
    [SerializeField] PlayerController playerController;

    public PlayerController PlayerController => playerController;

    const string statsPath = "Assets/ScriptableObjects/Player";

    void Start()
    {
        playerController.PlayerSO.creatureSO.onHealthChangedEvent?.Invoke(playerController.CurrentHealth, playerController.PlayerSO.creatureSO.MaxHealth);
    }

    void Awake()
    {
        playerController = PlayerController.Instance;
        playerController.GetComponent<PlayerInput>().enabled = true;
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
}
