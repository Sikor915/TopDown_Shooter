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
    public Tilemap[] Tilemaps { get => tilemaps; }
    [SerializeField] List<Vector3> obstaclePositions;
    [SerializeField] List<Vector3> spawnPositions;
    [SerializeField] List<Door> doorPositions;
    public List<Door> DoorPositions { get => doorPositions; }
    public List<Vector3> ObstaclePositions { get => obstaclePositions; }
    public List<Vector3> SpawnPositions { get => spawnPositions; }


    void Awake()
    {
        doorPositions = new List<Door>();
        obstaclePositions = new List<Vector3>();
        FindDoors();
        FindObstacles();
        FindSpawnPoints();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (doorPositions == null)
            return;
        foreach (var door in doorPositions)
        {
            Gizmos.DrawSphere(door.MiddlePosition, 0.1f);
        }
        foreach (var spawn in spawnPositions)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(spawn, Vector3.one * 0.2f);
        }
    }

    public List<Door> GetDoorsWithSize(int size)
    {
        List<Door> matchingDoors = new();
        foreach (var door in doorPositions)
        {
            if (door.Size == size && !door.IsConnected)
            {
                matchingDoors.Add(door);
            }
        }
        return matchingDoors;
    }

    void FindDoors()
    {
        Tilemap doorTilemap = null;
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.gameObject.CompareTag("Door"))
            {
                doorTilemap = tilemap;
                break;
            }
        }
        if (doorTilemap == null)
            return;

        List<Vector3Int> doorCells = new();

        doorTilemap.CompressBounds();

        var bounds = doorTilemap.cellBounds;
        foreach (Vector3Int cellPos in bounds.allPositionsWithin)
        {
            if (!doorTilemap.HasTile(cellPos))
                continue;

            doorCells.Add(cellPos);
        }

        if (doorCells.Count <= 0)
            return;

        int i = 0;
        ConnectionPoint[] connectionPoints = GetComponentsInChildren<ConnectionPoint>();

        Debug.Log("Found " + connectionPoints.Length + " connection points in room " + gameObject.name);
        while (doorCells.Count > 0)
        {
            List<Vector3> currentDoorTiles = new()
            {
                doorTilemap.GetCellCenterWorld(doorCells[i])
            };
            Vector3Int checkPos = doorCells[i];
            doorCells.RemoveAt(i);

            for (int j = 0; j < doorCells.Count; j++)
            {
                if (Vector3Int.Distance(checkPos, doorCells[j]) <= 1.1f)
                {
                    currentDoorTiles.Add(doorTilemap.GetCellCenterWorld(doorCells[j]));
                    checkPos = doorCells[j];
                    doorCells.RemoveAt(j);
                    j--;
                }
            }
            Vector3 middlePos = Vector3.zero;
            foreach (var pos in currentDoorTiles)
            {
                middlePos += pos;
            }

            middlePos /= currentDoorTiles.Count;
            Vector3 localMiddlePos = transform.InverseTransformPoint(middlePos);
            Debug.Log("Local middle pos: " + localMiddlePos);
            Debug.Log("World middle pos: " + middlePos);

            ConnectionPoint.SideEnum side = HelperFunctions.DetermineSide(localMiddlePos);

            Debug.Log("Determined side: " + side);

            ConnectionPoint connectionPoint = Array.Find(connectionPoints, cp => cp.Side == side && !cp.IsTaken);

            doorPositions.Add(new Door(currentDoorTiles, middlePos, currentDoorTiles.Count, connectionPoint));
            Debug.Log("Found door at " + middlePos + " with size " + currentDoorTiles.Count + " in room " + gameObject.name);
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

[Serializable]
public class Door
{
    readonly List<Vector3> doorTiles;
    readonly int size;
    readonly Vector3 middlePosition;
    ConnectionPoint connectionPoint;
    bool isOpen;
    bool isConnected;

    public List<Vector3> DoorTiles { get => doorTiles; }
    public Vector3 MiddlePosition { get => middlePosition; }
    public int Size { get => size; }
    public bool IsOpen { get => isOpen; set => isOpen = value; }
    public bool IsConnected
    {
        get => isConnected;
        set
        {
            isConnected = value;
            if (connectionPoint != null)
                connectionPoint.IsTaken = value;
        }

    }
    public ConnectionPoint ConnectionPoint { get => connectionPoint; set => connectionPoint = value; }

    public Door(List<Vector3> doorTiles, Vector3 middlePosition, int size, ConnectionPoint connectionPoint = null)
    {
        this.doorTiles = doorTiles;
        this.middlePosition = middlePosition;
        this.size = size;
        isOpen = false;
        this.connectionPoint = connectionPoint;
        isConnected = false;

    }

    public bool AllOverlapOther(List<Vector3> listB, float tolerance = 0.01f)
    {
        if (doorTiles == null || listB == null)
            return false;

        foreach (var posA in doorTiles)
        {
            bool anyMatch = false;
            foreach (var posB in listB)
            {
                if (Vector3.Distance(posA, posB) <= tolerance)
                {
                    anyMatch = true;
                    break;
                }
            }
            if (!anyMatch)
                return false;
        }
        return true;
    }

    public void ConnectDoors()
    {
        isConnected = true;
        ConnectionPoint.IsTaken = true;
    }
}
