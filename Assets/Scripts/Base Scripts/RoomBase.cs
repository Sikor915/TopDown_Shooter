using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*TODO: 
- Add check for open room connections
- Somehow work out how to do doors
*/
public class RoomBase : MonoBehaviour
{
    [SerializeField] int height, width;
    public int Height { get => height; }
    public int Width { get => width; }
    public Vector2Int Size { get => new(width, height); }

    [SerializeField] Tilemap[] tilemaps;
    [SerializeField] List<Vector3> doorPositions;
    [SerializeField] List<Vector3> obstaclePositions;
    [SerializeField] List<Vector3> spawnPositions;
    void Awake()
    {
        tilemaps = GetComponentsInChildren<Tilemap>();
        doorPositions = new List<Vector3>();
        obstaclePositions = new List<Vector3>();
        FindDoors();
        FindObstacles();
        FindSpawnPoints();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FindDoors()
    {
        foreach (var tilemap in tilemaps)
        {
            if (!tilemap.gameObject.CompareTag("Door"))
            {
                continue;
            }
            var bounds = tilemap.cellBounds;
            foreach (Vector3Int cellPos in bounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(cellPos))
                    continue;

                // Optionally get the center of the tile
                Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                doorPositions.Add(worldPos);
                Debug.Log("Found door at " + worldPos);
            }
            Debug.Log("Found " + doorPositions.Count + " doors in room " + gameObject.name);
        }
    }

    void FindObstacles()
    {
        foreach (var tilemap in tilemaps)
        {
            if (!tilemap.gameObject.CompareTag("Obstacle"))
            {
                continue;
            }
            var bounds = tilemap.cellBounds;
            foreach (Vector3Int cellPos in bounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(cellPos))
                    continue;

                // Optionally get the center of the tile
                Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                obstaclePositions.Add(worldPos);
                Debug.Log("Found obstacle at " + worldPos);
            }
            Debug.Log("Found " + obstaclePositions.Count + " obstacles in room " + gameObject.name);
        }
    }
    void FindSpawnPoints()
    {
        foreach (var tilemap in tilemaps)
        {
            if (!tilemap.gameObject.CompareTag("Spawnpoints"))
            {
                continue;
            }
            var bounds = tilemap.cellBounds;
            foreach (Vector3Int cellPos in bounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(cellPos))
                    continue;

                // Optionally get the center of the tile
                Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                spawnPositions.Add(worldPos);
                Debug.Log("Found spawn point at " + worldPos);
            }
            Debug.Log("Found " + spawnPositions.Count + " spawn points in room " + gameObject.name);
        }
    }
}
