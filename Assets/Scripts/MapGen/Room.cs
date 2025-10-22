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

    public List<Vector2Int> GetRandomPatrolPoints(int amount = 4)
    {
        List<Vector2Int> selectedPoints = new List<Vector2Int>();
        List<Vector2Int> availablePoints = new List<Vector2Int>(patrolPoints);

        Debug.Log("Available Patrol Points: " + availablePoints.Count);

        for (int i = 0; i < amount && availablePoints.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            selectedPoints.Add(availablePoints[randomIndex]);
            availablePoints.RemoveAt(randomIndex);
        }

        return selectedPoints;
    }
}