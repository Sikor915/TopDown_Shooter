using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase wallTile;
    [SerializeField] string mapSeed = "Default";

    [SerializeField] int minRoomSize = 10;
    [SerializeField] int maxRoomSize = 20;
    [SerializeField] RectInt mapSpace = new RectInt(-50, -50, 100, 100);
    BSPNode bspRoot;
    List<RectInt> rooms;

    void Start()
    {
        CommenceGeneration();
    }

    public void CommenceGeneration()
    {
        Random.InitState(mapSeed.GetHashCode());
        bspRoot = null;
        BSPGenerator bspGenerator = new BSPGenerator();
        bspRoot = bspGenerator.GenerateBSP(mapSpace, minRoomSize);
        rooms = GenerateRooms(bspRoot);
        DrawDungeon(rooms);
    }

    List<RectInt> GenerateRooms(BSPNode node)
    {
        List<RectInt> roomsNew = new List<RectInt>();
        TraverseAndCreateRooms(node, roomsNew);
        return roomsNew;
    }

    // TODO: Implement corridor generation
    // TODO: Add random spawnpoints in the rooms
    void TraverseAndCreateRooms(BSPNode node, List<RectInt> rooms)
    {
        if (node == null) return;

        if (node.left == null && node.right == null) // Leaf node
        {
            int roomWidth = Random.Range(minRoomSize / 2, Mathf.Min(node.rect.width - 1, maxRoomSize));
            int roomHeight = Random.Range(minRoomSize / 2, Mathf.Min(node.rect.height - 1, maxRoomSize));
            int roomX = Random.Range(node.rect.x, node.rect.x + node.rect.width - roomWidth);
            int roomY = Random.Range(node.rect.y, node.rect.y + node.rect.height - roomHeight);

            RectInt room = new RectInt(roomX, roomY, roomWidth, roomHeight);
            rooms.Add(room);
        }
        else
        {
            TraverseAndCreateRooms(node.left, rooms);
            TraverseAndCreateRooms(node.right, rooms);
        }
    }

    // TODO: Add start and end rooms somewhere so that they connect to the rest of the dungeon
    void DrawDungeon(List<RectInt> rooms, List<Vector2Int> corridors = null)
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        foreach (var room in rooms)
        {
            for (int x = room.xMin; x < room.xMax; x++)
            {
                for (int y = room.yMin; y < room.yMax; y++)
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
                    Debug.Log("DRAW_DUNGEON: Placing wall tile at " + (x + 1) + ", " + y);
                    wallTilemap.SetTile(new Vector3Int(x + 1, y, 0), wallTile);
                }
                if (floorTilemap.GetTile(new Vector3Int(x - 1, y, 0)) == null)
                {
                    Debug.Log("DRAW_DUNGEON: Placing wall tile at " + (x - 1) + ", " + y);
                    wallTilemap.SetTile(new Vector3Int(x - 1, y, 0), wallTile);
                }
                if (floorTilemap.GetTile(new Vector3Int(x, y + 1, 0)) == null)
                {
                    Debug.Log("DRAW_DUNGEON: Placing wall tile at " + x + ", " + (y + 1));
                    wallTilemap.SetTile(new Vector3Int(x, y + 1, 0), wallTile);
                }
                if (floorTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == null)
                {
                    Debug.Log("DRAW_DUNGEON: Placing wall tile at " + x + ", " + (y - 1));
                    wallTilemap.SetTile(new Vector3Int(x, y - 1, 0), wallTile);
                }
            }
        }
    }

    // MAP GENERATION CODE VERSION 0.1 BELOW

    /*[SerializeField] List<RoomBase> roomPrefabs;
    [SerializeField] Vector3Int startPosition;
    [SerializeField] float rotationOfStartingRoom = 0f;
    [SerializeField] int MapWidth, MapHeight;
    [Header("Generation DEBUG Settings")]
    [SerializeField] int roomsToSpawn = 10;
    RoomBase startingRoom;
    List<RoomBase> spawnedRooms;
    readonly Vector3 roomSpawnPosition = new(-50, 20, 0);
    void Awake()
    {
        roomPrefabs = Resources.LoadAll<RoomBase>("Rooms").ToList();
        if (roomPrefabs == null || roomPrefabs.Count == 0)
        {
            Debug.LogError("No room prefabs found in Resources/Rooms!");
        }
        startingRoom = roomPrefabs.Find(room => room.gameObject.name == "Corridor");
        spawnedRooms = new List<RoomBase>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CommenceGeneration();
    }

    public void CommenceGeneration()
    {
        ResetMap();
        RoomBase instantiatedRoom = Instantiate(startingRoom, startPosition, Quaternion.Euler(0, 0, rotationOfStartingRoom));
        spawnedRooms.Add(instantiatedRoom);
        int spawnTries = 0;
        while (spawnedRooms.Count < roomsToSpawn && spawnTries < roomsToSpawn * 2)
        {
            GenerateRoom();
            spawnTries++;
        }
    }

    void ResetMap()
    {
        foreach (var room in spawnedRooms)
        {
            Destroy(room.gameObject);
        }
        spawnedRooms.Clear();
    }

    void GenerateRoom()
    {
        RoomBase randomExistingRoom;
        List<Door> nextRoomValidDoors;
        RoomBase nextRoom;
        Door startingDoor;
        int counter = 0;
        do
        {
            int randomIndex;
            nextRoom = null;
            nextRoomValidDoors = null;

            randomExistingRoom = spawnedRooms[Random.Range(0, spawnedRooms.Count)];
            List<Door> existingRoomDoors = randomExistingRoom.DoorPositions.Where(door => !door.IsConnected).ToList();
            startingDoor = existingRoomDoors[Random.Range(0, existingRoomDoors.Count)];

            while (true)
            {
                randomIndex = Random.Range(0, roomPrefabs.Count);
                nextRoom = Instantiate(roomPrefabs[randomIndex], roomSpawnPosition, Quaternion.identity, randomExistingRoom.transform);
                nextRoomValidDoors = nextRoom.GetDoorsWithSize(startingDoor.Size).Where(door => !door.IsConnected).ToList();
                if (nextRoomValidDoors.Count == 0)
                {
                    Destroy(nextRoom.gameObject);
                    continue;
                }
                break;
            }
        } while (!AttachRoom(randomExistingRoom, startingDoor, nextRoom, nextRoomValidDoors) && counter++ < 10);
    }

    bool AttachRoom(RoomBase existingRoom, Door existingRoomDoor, RoomBase newRoom, List<Door> newRoomValidDoors)
    {
        Door matchingDoor = null;
        foreach (var door in newRoomValidDoors)
        {
            if (AreOpposite(existingRoomDoor.ConnectionPoint.Side, door.ConnectionPoint.Side))
            {
                matchingDoor = door;
                break;
            }
        }
        if (matchingDoor == null)
        {
            Debug.LogWarning("ATTACH_ROOM: No matching door");
            Destroy(newRoom.gameObject);
            return false;
        }
        Debug.Log("ATTACH_ROOM: Existing room door at " + existingRoomDoor.MiddlePosition + " on side " + existingRoomDoor.ConnectionPoint.Side);
        Debug.Log("ATTACH_ROOM: Matching door found at " + matchingDoor.MiddlePosition + " on side " + matchingDoor.ConnectionPoint.Side);

        Vector3 newRoomPosition = newRoom.transform.position;

        Vector3 offset = matchingDoor.ConnectionPoint.transform.position - newRoomPosition;

        newRoom.transform.position = existingRoomDoor.ConnectionPoint.transform.position - offset;

        // Check for overlap with existing rooms
        foreach (var room in spawnedRooms)
        {
            if (AreCollidersOverlapping(room.GetComponent<BoxCollider2D>(), newRoom.GetComponent<BoxCollider2D>()))
            {
                Debug.LogWarning("ATTACH_ROOM: Overlap detected with room " + room.name);
                Destroy(newRoom.gameObject);
                return false;
            }
        }

        existingRoomDoor.IsConnected = true;
        matchingDoor.IsConnected = true;
        spawnedRooms.Add(newRoom);
        return true;
    }

    bool AreOpposite(ConnectionPoint.SideEnum a, ConnectionPoint.SideEnum b)
    {
        return (a == ConnectionPoint.SideEnum.North && b == ConnectionPoint.SideEnum.South)
            || (a == ConnectionPoint.SideEnum.South && b == ConnectionPoint.SideEnum.North)
            || (a == ConnectionPoint.SideEnum.East && b == ConnectionPoint.SideEnum.West)
            || (a == ConnectionPoint.SideEnum.West && b == ConnectionPoint.SideEnum.East);
    }
    //TODO: Somehow fix this bullshit with intersecting bounds. 
    //Currently it doesn't detect overlaps even tho it should per the documentation
    bool AreCollidersOverlapping(BoxCollider2D a, BoxCollider2D b)
    {
        if (a == null || b == null) return false;

        Bounds boundsA = a.bounds;
        Bounds boundsB = b.bounds;

        Debug.Log(boundsA + ", " + boundsA.size);
        Debug.Log(boundsB + ", " + boundsB.size);

        if (!boundsA.Intersects(boundsB))
        {
            Debug.Log("AreCollidersOverlapping: No overlap detected in bounding box check");
            return false;
        }

        // Detailed physics overlap test
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Collider2D[] results = new Collider2D[1];
        int count = a.Overlap(filter, results);

        foreach (var hit in results)
        {
            if (hit == b)
            {
                Debug.LogWarning("AreCollidersOverlapping: Overlap detected");
                return true;
            }
        }
        Debug.Log("AreCollidersOverlapping: No overlap detected after detailed check");
        return false;
    }*/
}
