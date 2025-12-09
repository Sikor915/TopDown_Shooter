using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] List<Vector2Int> spawnPoints;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] List<GameObject> weaponPrefabs;

    readonly List<GameObject> enemies = new();
    public List<GameObject> Enemies { get { return enemies; } }

    const string gunPrefabsPath = "Prefabs/Weapons/Guns/";

    public void AddSpawnPoint(Vector2Int point)
    {
        spawnPoints.Add(point);
    }

    public void ClearSpawnPoints()
    {
        spawnPoints.Clear();
    }

    public void SpawnEnemies()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            GameObject enemy = ObjectPooling.Instance.GetPooledEnemy();
            if (enemy != null)
            {
                enemy.GetComponent<EnemyController>().EnemyWeapon = RandomizeSpawnWeapon().GetComponent<Weapon>();
                enemy.transform.position = new Vector3(spawnPoint.x, spawnPoint.y, 0);
                enemy.GetComponent<EnemyController>().ActivateEnemy();
                enemies.Add(enemy);
                enemy.GetComponent<BasicEnemyAI>().ActivateEnemy();
            }
        }
        Debug.Log("Spawned Enemies: " + enemies.Count);
    }

    public void ClearEnemies()
    {
        foreach (var enemy in enemies)
        {
            ObjectPooling.Instance.ReturnEnemyToPool(enemy.GetComponent<EnemyController>());
        }
        enemies.Clear();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        ObjectPooling.Instance.ReturnEnemyToPool(enemy.GetComponent<EnemyController>());
        enemies.Remove(enemy);
    }

    GameObject RandomizeSpawnWeapon()
    {
        int randomIndex = Random.Range(0, weaponPrefabs.Count);
        GameObject gunPrefab = weaponPrefabs[randomIndex];
        return gunPrefab;
    }
}
