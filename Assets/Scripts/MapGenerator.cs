using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*TODO:
- Map generation based on trying to connect rooms via doors
- Ensure no overlapping rooms
*/
public class MapGenerator : MonoBehaviour
{
    [SerializeField] List<RoomBase> roomPrefabs;
    RoomBase startingRoom;
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
    }

    // Update is called once per frame
    void Update()
    {

    }

}
