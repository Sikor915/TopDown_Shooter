using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    [SerializeField] SideEnum side;
    public SideEnum Side { get => side; }
    public enum SideEnum
    {
        North,
        East,
        South,
        West,
        None
    }

    [SerializeField] Vector3 positionFromRoot;
    public Vector3 PositionFromRoot { get => positionFromRoot; }
    [SerializeField] bool isTaken = false;
    public bool IsTaken { get => isTaken; set => isTaken = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        positionFromRoot = transform.localPosition;
        Debug.Log("Connection point at " + positionFromRoot + " on side " + side);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
