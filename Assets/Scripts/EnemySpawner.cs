using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : Singleton<EnemySpawner>
{
    // In seconds
    [SerializeField] float interval = 2f;
    [SerializeField] SpriteRenderer background;
    [SerializeField] List<Vector2Int> spawnPoints;
    [SerializeField] GameObject enemyPrefab;

    List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> Enemies { get { return enemies; } }

    float timer = 0f;
    readonly float camSizeBuffer = 2.0f;


    public void AddSpawnPoint(Vector2Int point)
    {
        spawnPoints.Add(point);
    }

    public void SpawnEnemies()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            GameObject enemy = Instantiate(enemyPrefab, new Vector2(spawnPoint.x + 0.5f, spawnPoint.y + 0.5f), Quaternion.identity);
            enemies.Add(enemy);
        }
        Debug.Log("Spawned Enemies: " + enemies.Count);
    }

    public void ClearEnemies()
    {
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    // Update is called once per frame
    /*void Update() {
        timer += Time.deltaTime;

        if (timer >= interval) {
            timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy() {

        Bounds spriteBounds = background.bounds;

        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;

        Vector3 camPos = Camera.main.transform.position;

        float camLeft = camPos.x - camWidth / 2f;
        float camRight = camPos.x + camWidth / 2f;
        float camBottom = camPos.y - camHeight / 2f;
        float camTop = camPos.y + camHeight / 2f;
        float spawnX = 0.0f;
        float spawnY = 0.0f;

        int side = Random.Range(0, 4);

        switch (side) {
            case 0: // Top
                spawnY = Random.Range(camTop + camSizeBuffer, spriteBounds.max.y);
                spawnX = Random.Range(
                    Mathf.Max(spriteBounds.min.x, camLeft - camSizeBuffer),
                    Mathf.Min(spriteBounds.max.x, camRight + camSizeBuffer)
                );

                break;
            case 1: // Bottom
                spawnY = Random.Range(spriteBounds.min.y, camBottom - camSizeBuffer);
                spawnX = Random.Range(
                    Mathf.Max(spriteBounds.min.x, camLeft - camSizeBuffer),
                    Mathf.Min(spriteBounds.max.x, camRight + camSizeBuffer)
                );

                break;
            case 2: // Left
                spawnX = Random.Range(spriteBounds.min.x, camLeft - camSizeBuffer);
                spawnY = Random.Range(
                    Mathf.Max(spriteBounds.min.y, camBottom - camSizeBuffer),
                    Mathf.Min(spriteBounds.max.y, camTop + camSizeBuffer)
                );

                break;
            case 3: // Right
                spawnX = Random.Range(camRight + camSizeBuffer, spriteBounds.max.x);
                spawnY = Random.Range(
                    Mathf.Max(spriteBounds.min.y, camBottom - camSizeBuffer),
                    Mathf.Min(spriteBounds.max.y, camTop + camSizeBuffer)
                );

                break;
        }
        if (spawnX > spriteBounds.max.x) spawnX = spriteBounds.max.x - 0.1f;
        if (spawnX < spriteBounds.min.x) spawnX = spriteBounds.min.x + 0.1f;
        if (spawnY > spriteBounds.max.y) spawnY = spriteBounds.max.y - 0.1f;
        if (spawnY < spriteBounds.min.y) spawnY = spriteBounds.min.y + 0.1f;
        Vector2 spawnPosition = new(spawnX, spawnY);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

    }*/
}
