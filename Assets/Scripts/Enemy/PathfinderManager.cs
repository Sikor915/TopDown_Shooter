using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfinderManager : Singleton<PathfinderManager>
{
    public Tilemap floorTilemap;
    Node[,] grid;
    int w, h;
    
    public void InitGrid()
    {
        floorTilemap.CompressBounds();
        BoundsInt bounds = floorTilemap.cellBounds;
        w = bounds.size.x;
        h = bounds.size.y;
        grid = new Node[w, h];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector3Int tilePosition = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                TileBase tile = floorTilemap.GetTile(tilePosition);
                bool walkable = tile != null;
                grid[x, y] = new Node(tilePosition, walkable);
            }
        }
    }
    public Node NodeFromWorld(Vector3 worldPos)
    {
        //Here it returns null
        Vector3Int cell = floorTilemap.WorldToCell(worldPos);
        int x = cell.x - floorTilemap.cellBounds.xMin;
        int y = cell.y - floorTilemap.cellBounds.yMin;
        if (x >= 0 && x < w && y >= 0 && y < h)
            return grid[x, y];
        return null;
    }

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        Node startNode = NodeFromWorld(startWorld);
        Node targetNode = NodeFromWorld(targetWorld);
        if (startNode == null || targetNode == null) return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet.OrderBy(n => n.FCost).First();
            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                int newCost = current.gCost + GetDistance(current, neighbor);
                if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = current;
                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
        return null;
    }

    int GetDistance(Node current, Node neighbor)
    {
        return Mathf.Abs(current.cell.x - neighbor.cell.x) + Mathf.Abs(current.cell.y - neighbor.cell.y);
    }

    IEnumerable<Node> GetNeighbors(Node current)
    {
        List<Node> neighbors = new List<Node>();
        Vector3Int[] directions = {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighborPos = current.cell + dir;
            Node neighbor = NodeFromWorld(floorTilemap.CellToWorld(neighborPos));
            if (neighbor != null)
                neighbors.Add(neighbor);
        }
        return neighbors;
    }

    List<Vector3> RetracePath(Node start, Node end)
    {
        List<Vector3> path = new List<Vector3>();
        Node curr = end;
        while (curr != start)
        {
            path.Add(floorTilemap.CellToWorld(curr.cell) + new Vector3(0.5f, 0.5f, 0));
            curr = curr.parent;
        }
        path.Reverse();
        return path;
    }
}

public class Node
{
    public Vector3Int cell;
    public bool walkable;
    public int gCost, hCost;
    public Node parent;
    public int FCost => gCost + hCost;

    public Node(Vector3Int _cell, bool _walkable)
    {
        cell = _cell;
        walkable = _walkable;
    }
}