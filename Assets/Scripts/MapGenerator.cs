using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    RoomBase startingRoom;
    readonly Vector3 roomSpawnPosition = new(-50, 20, 0);

    void Awake()
    {
        // Load all prefabs in “Rooms” folder inside Resources
        roomPrefabs = Resources.LoadAll<RoomBase>("Rooms").ToList();
        // Check count
        if (roomPrefabs == null || roomPrefabs.Count == 0)
        {
            Debug.LogError("No room prefabs found in Resources/Rooms!");
        }
        startingRoom = roomPrefabs.Find(room => room.gameObject.name == "Corridor");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RoomBase instantiatedRoom = Instantiate(startingRoom, startPosition, Quaternion.Euler(0, 0, rotationOfStartingRoom));
        List<Door> startingDoors = instantiatedRoom.DoorPositions;
        int randomIndex = Random.Range(0, startingDoors.Count);
        Door startingDoor = startingDoors[randomIndex];
        Debug.Log("Starting door at " + startingDoor.MiddlePosition + " with size " + startingDoor.Size);
        RoomBase nextRoom = null;
        List<Door> nextRoomValidDoors = null;
        while (true)
        {
            randomIndex = Random.Range(0, roomPrefabs.Count);
            nextRoom = Instantiate(roomPrefabs[randomIndex], roomSpawnPosition, Quaternion.identity, instantiatedRoom.transform);
            nextRoomValidDoors = nextRoom.GetDoorsWithSize(startingDoor.Size);
            if (nextRoomValidDoors.Count == 0)
            {
                Destroy(nextRoom.gameObject);
                continue;
            }
            break;
        }
        AttachRoom(instantiatedRoom, startingDoor, nextRoom, nextRoomValidDoors);


        /*randomIndex = Random.Range(0, nextRoomValidDoors.Count);
        Door nextRoomDoor = nextRoomValidDoors[randomIndex];
        Vector3 roomStart = nextRoom.transform.position;


        Vector3 offset = nextRoomDoor.MiddlePosition - roomStart;
        Vector3 newPosition = startingDoor.MiddlePosition - offset;
        nextRoom.transform.position = newPosition;
        Debug.Log("Next room door at " + nextRoomDoor.MiddlePosition + " with size " + nextRoomDoor.Size);

        if (nextRoomDoor.AllOverlapOther(startingDoor.DoorTiles))
        {
            Debug.Log("Door tiles do not match!");
            nextRoom.transform.RotateAround(nextRoomDoor.MiddlePosition, Vector3.forward, 90);
        }
        // TODO: Work out the rotation properly
        Tilemap newRoomFloor = nextRoom.Tilemaps.FirstOrDefault(t => t.gameObject.CompareTag("Floor"));
        Tilemap startingRoomFloor = instantiatedRoom.Tilemaps.FirstOrDefault(t => t.gameObject.CompareTag("Floor"));

        List<Vector3Int> overlappingCells = GetOverlappingCells(newRoomFloor, startingRoomFloor);
        if (overlappingCells.Count > 0)
        {
            Debug.LogError("Rooms are overlapping at " + overlappingCells.Count + " cells!");
        }
        else
        {
            Debug.Log("Rooms placed successfully without overlap.");
        }*/

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static List<Vector3Int> GetOverlappingCells(Tilemap a, Tilemap b)
    {
        List<Vector3Int> overlapping = new();

        a.CompressBounds();
        b.CompressBounds();

        BoundsInt boundsA = a.cellBounds;

        foreach (Vector3Int cell in boundsA.allPositionsWithin)
        {
            if (!a.HasTile(cell))
                continue;

            if (b.HasTile(cell))
            {
                overlapping.Add(cell);
            }
        }

        return overlapping;
    }

    void AttachRoom(RoomBase existingRoom, Door existingRoomDoor, RoomBase newRoom, List<Door> newRoomValidDoors)
    {
        Door matchingDoor = null;
        foreach (var door in newRoomValidDoors)
        {
            if(AreOpposite(existingRoomDoor.ConnectionPoint.Side, door.ConnectionPoint.Side))
            {
                matchingDoor = door;
                break;
            }
        }
        if (matchingDoor == null)
        {
            Debug.LogWarning("No matching door");
            return;
        }

        Debug.Log("Matching door found at " + matchingDoor.MiddlePosition + " on side " + matchingDoor.ConnectionPoint.Side);

        Vector3 existingRoomPosition = existingRoom.transform.position;
        Vector3 newRoomPosition = newRoom.transform.position;


        Vector3 offset = matchingDoor.ConnectionPoint.transform.position - newRoomPosition;

        Debug.Log("Offset: " + offset);
        Debug.Log("Offset from door to connect: " + existingRoomDoor.ConnectionPoint.transform.position);
        Debug.Log("Old position: " + newRoomPosition);
        newRoom.transform.position = existingRoomDoor.ConnectionPoint.transform.position - offset;
        Debug.Log("New position: " + newRoom.transform.position);

        existingRoomDoor.IsConnected = true;
        matchingDoor.IsConnected = true;
    }

    bool AreOpposite(ConnectionPoint.SideEnum a, ConnectionPoint.SideEnum b)
    {
        return (a == ConnectionPoint.SideEnum.North && b == ConnectionPoint.SideEnum.South)
            || (a == ConnectionPoint.SideEnum.South && b == ConnectionPoint.SideEnum.North)
            || (a == ConnectionPoint.SideEnum.East && b == ConnectionPoint.SideEnum.West)
            || (a == ConnectionPoint.SideEnum.West && b == ConnectionPoint.SideEnum.East);
    }
    RoomBase GetRandomRoomWithDoorSize(int size)
    {
        List<RoomBase> matchingRooms = new();
        foreach (var room in roomPrefabs)
        {
            if (room.GetDoorsWithSize(size).Count > 0)
            {
                matchingRooms.Add(room);
            }
        }
        if (matchingRooms.Count == 0)
        {
            Debug.LogError("No rooms found with door size " + size);
            return null;
        }
        RoomBase selectedRoom = matchingRooms[Random.Range(0, matchingRooms.Count)];
        Debug.Log("Selected room: " + selectedRoom.gameObject.name);
        return selectedRoom;
    }
}
