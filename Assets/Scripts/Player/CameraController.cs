using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject player;
    Vector3 cameraOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraOffset = new Vector3(0, 0, -10);
        gameObject.transform.position = new Vector3(player.transform.position.x + cameraOffset.x, player.transform.position.y + cameraOffset.y, cameraOffset.z);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(player.transform.position.x + cameraOffset.x, player.transform.position.y + cameraOffset.y, cameraOffset.z);

    }
}
