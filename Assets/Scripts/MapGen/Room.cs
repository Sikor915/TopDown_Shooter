using System.Collections.Generic;
using UnityEngine;

public class Room
{
    RectInt rect;
    public RectInt Rect { get { return rect; } }
    List<Vector2Int> patrolPoints;
    public List<Vector2Int> PatrolPoints { get { return patrolPoints; } }

    public Room(RectInt rect)
    {
        this.rect = rect;
        patrolPoints = new List<Vector2Int>();
    }

    public void AddPatrolPoint(Vector2Int point)
    {
        patrolPoints.Add(point);
    }

    public void RemovePatrolPoint(Vector2Int point)
    {
        patrolPoints.Remove(point);
    }
}