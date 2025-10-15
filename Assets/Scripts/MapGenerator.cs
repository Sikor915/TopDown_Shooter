using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

/*TODO:
- Map generation based on trying to connect rooms via doors
- Ensure no overlapping rooms
*/
public class MapGenerator : MonoBehaviour
{
    [SerializeField] List<RoomBase> roomPrefabs;
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
            if (room == existingRoom) continue;

            if (AreTilemapsOverlapping(room.Tilemaps[1].gameObject.GetComponent<CompositeCollider2D>(), newRoom.Tilemaps[1].gameObject.GetComponent<CompositeCollider2D>()))
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
    bool AreTilemapsOverlapping(CompositeCollider2D a, CompositeCollider2D b)
    {
        if (a == null || b == null) return false;

        Bounds boundsA = a.bounds;
        Bounds boundsB = b.bounds;

        Debug.Log(boundsA + ", " + boundsA.size);
        Debug.Log(boundsB + ", " + boundsB.size);

        if (!boundsA.Intersects(boundsB))
        {
            Debug.Log("AreTilemapsOverlapping: No overlap detected in bounding box check");
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
                Debug.LogWarning("AreTilemapsOverlapping: Overlap detected");
                return true;
            }
        }
        Debug.Log("AreTilemapsOverlapping: No overlap detected after detailed check");
        return false;
    }
}
