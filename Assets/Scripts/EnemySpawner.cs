using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour {
    // In seconds
    [SerializeField] float interval = 2f;
    [SerializeField] SpriteRenderer background;

    public GameObject enemyPrefab;

    float timer = 0f;
    readonly float camSizeBuffer = 2.0f;

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;

        if (timer >= interval) {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy() {

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

        Vector2 spawnPosition = new(spawnX, spawnY);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

    }
}
