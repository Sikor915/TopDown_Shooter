using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Preperations for Dungeon Generation")]
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase wallTile;
    [SerializeField] string mapSeed = "Default";
    
    [Header("BSP Generation Settings")]
    [SerializeField] int minRoomSize = 10;
    [SerializeField] int maxRoomSize = 20;
    [SerializeField] int maxBSPDepth = 4;
    [SerializeField] RectInt mapSpace = new RectInt(-50, -50, 100, 100);
    
    
    BSPNode bspRoot;
    List<Room> rooms;
    List<Vector2Int> corridors;

    void Start()
    {
        CommenceGeneration();
    }

    public void CommenceGeneration()
    {
        Random.InitState(mapSeed.GetHashCode());
        bspRoot = null;
        EnemySpawner.Instance.ClearEnemies();
        BSPGenerator bspGenerator = new BSPGenerator();
        bspRoot = bspGenerator.GenerateBSP(mapSpace, minRoomSize, maxBSPDepth);
        (rooms, corridors) = GenerateRooms(bspRoot);
        DrawDungeon();
        PrepareSpawnpoints();
        PreparePatrolPoints();
    }

    (List<Room>, List<Vector2Int>) GenerateRooms(BSPNode node)
    {
        List<Room> roomsNew = new List<Room>();
        TraverseAndCreateRooms(node, roomsNew);
        List<Vector2Int> corridors = GenerateCorridors(roomsNew);
        return (roomsNew, corridors);
    }

    List<Vector2Int> GenerateCorridors(List<Room> rooms)
    {
        List<Vector2Int> corridors = new List<Vector2Int>();
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Vector2Int roomACenter = new Vector2Int(rooms[i].Rect.x + rooms[i].Rect.width / 2, rooms[i].Rect.y + rooms[i].Rect.height / 2);
            Vector2Int roomBCenter = new Vector2Int(rooms[i + 1].Rect.x + rooms[i + 1].Rect.width / 2, rooms[i + 1].Rect.y + rooms[i + 1].Rect.height / 2);

            for (int x = Mathf.Min(roomACenter.x, roomBCenter.x); x <= Mathf.Max(roomACenter.x, roomBCenter.x); x++)
            {
                corridors.Add(new Vector2Int(x, roomACenter.y));
                corridors.Add(new Vector2Int(x, roomACenter.y + 1));
                corridors.Add(new Vector2Int(x, roomACenter.y + 2));
                corridors.Add(new Vector2Int(x, roomACenter.y - 1));
                corridors.Add(new Vector2Int(x, roomACenter.y - 2));
            }
            for (int y = Mathf.Min(roomACenter.y, roomBCenter.y); y <= Mathf.Max(roomACenter.y, roomBCenter.y); y++)
            {
                corridors.Add(new Vector2Int(roomBCenter.x, y));
                corridors.Add(new Vector2Int(roomBCenter.x + 1, y));
                corridors.Add(new Vector2Int(roomBCenter.x + 2, y));
                corridors.Add(new Vector2Int(roomBCenter.x - 1, y));
                corridors.Add(new Vector2Int(roomBCenter.x - 2, y));
            }
        }        
        return corridors;
    }

    // TODO: Refactor corridors to maybe check if you can reach every room from any other room and place corridors accordingly
    // TODO: Add random spawnpoints in the rooms
    void TraverseAndCreateRooms(BSPNode node, List<Room> rooms)
    {
        if (node == null) return;

        if (node.left == null && node.right == null) // Leaf node
        {
            int roomWidth = Random.Range(minRoomSize / 2, Mathf.Min(node.rect.width - 1, maxRoomSize));
            int roomHeight = Random.Range(minRoomSize / 2, Mathf.Min(node.rect.height - 1, maxRoomSize));
            int roomX = Random.Range(node.rect.x, node.rect.x + node.rect.width - roomWidth);
            int roomY = Random.Range(node.rect.y, node.rect.y + node.rect.height - roomHeight);

            Room room = new Room(new RectInt(roomX, roomY, roomWidth, roomHeight));
            rooms.Add(room);
        }
        else
        {
            TraverseAndCreateRooms(node.left, rooms);
            TraverseAndCreateRooms(node.right, rooms);
        }
    }

    // TODO: Add start and end rooms somewhere so that they connect to the rest of the dungeon
    // TODO: Remove unnecessary walls if there is floor tiles on each side.
    void DrawDungeon()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        foreach (var room in rooms)
        {
            for (int x = room.Rect.xMin; x < room.Rect.xMax; x++)
            {
                for (int y = room.Rect.yMin; y < room.Rect.yMax; y++)
                {
                    floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }
        }

        if (corridors != null)
        {
            foreach (var corridorPosition in corridors)
            {
                floorTilemap.SetTile(new Vector3Int(corridorPosition.x, corridorPosition.y, 0), floorTile);
            }
        }

        BoundsInt bounds = floorTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (floorTilemap.GetTile(new Vector3Int(x, y, 0)) != floorTile)
                {
                    continue;
                }
                if (floorTilemap.GetTile(new Vector3Int(x + 1, y, 0)) == null)
                {
                    wallTilemap.SetTile(new Vector3Int(x + 1, y, 0), wallTile);
                }
                if (floorTilemap.GetTile(new Vector3Int(x - 1, y, 0)) == null)
                {
                    wallTilemap.SetTile(new Vector3Int(x - 1, y, 0), wallTile);
                }
                if (floorTilemap.GetTile(new Vector3Int(x, y + 1, 0)) == null)
                {
                    wallTilemap.SetTile(new Vector3Int(x, y + 1, 0), wallTile);
                }
                if (floorTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == null)
                {
                    wallTilemap.SetTile(new Vector3Int(x, y - 1, 0), wallTile);
                }
            }
        }
    }

    void PrepareSpawnpoints()
    {
        EnemySpawner.Instance.ClearSpawnPoints();
        foreach (var room in rooms)
        {
            int randomAmountOfSpawns = Random.Range(1, 4);
            for (int i = 0; i < randomAmountOfSpawns; i++)
            {
                Vector2Int spawnPoint = new(Random.Range(room.Rect.xMin, room.Rect.xMax), Random.Range(room.Rect.yMin, room.Rect.yMax));
                EnemySpawner.Instance.AddSpawnPoint(spawnPoint);
            }
        }
        EnemySpawner.Instance.SpawnEnemies();
    }

    void PreparePatrolPoints()
    {
        foreach (var room in rooms)
        {
            int randomAmountOfPatrols = Random.Range(2, 5);
            for (int i = 0; i < randomAmountOfPatrols; i++)
            {
                Vector2Int patrolPoint = new(Random.Range(room.Rect.xMin, room.Rect.xMax), Random.Range(room.Rect.yMin, room.Rect.yMax));
                room.AddPatrolPoint(patrolPoint);
            }
        }
    }

}