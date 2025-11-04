using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] List<Vector2Int> spawnPoints;
    [SerializeField] GameObject enemyPrefab;

    readonly List<GameObject> enemies = new();
    public List<GameObject> Enemies { get { return enemies; } }


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
}
