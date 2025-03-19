using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private GridConfig config;
    private Node[,] grid;
    private Vector2 gridOrigin;

    Vector2[] directions = new Vector2[]
    {
        Vector2.right,
        Vector2.left,
        Vector2.up,
        Vector2.down,
        (Vector2.right + Vector2.up).normalized,
        (Vector2.left + Vector2.up).normalized,
        (Vector2.right + Vector2.down).normalized,
        (Vector2.left + Vector2.down).normalized
    };

    private void Awake()
    {
        Instance = this;
        gridOrigin = new Vector2(-config.gridSize.x / 2, -config.gridSize.y / 2);
        GenerateGrid();
    }

    void GenerateGrid()
    {
        int gridWidth = Mathf.RoundToInt(config.gridSize.x / config.nodeSize);
        int gridHeight = Mathf.RoundToInt(config.gridSize.y / config.nodeSize);
        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPos = new Vector2(x * config.nodeSize, y * config.nodeSize) + gridOrigin;

                bool walkable = !Physics2D.OverlapCircle(worldPos, config.nodeSize * 0.45f, config.platformLayer);

                grid[x, y] = new Node(walkable, worldPos, x, y);
            }
        }
    }

    public Node FindClosestWalkableNode(Node startNode)
    {
        int searchRadius = 3;
        for (int r = 1; r <= searchRadius; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    int nx = Mathf.Clamp(startNode.gridX + dx, 0, grid.GetLength(0) - 1);
                    int ny = Mathf.Clamp(startNode.gridY + dy, 0, grid.GetLength(1) - 1);
                    if (grid[nx, ny].walkable)
                        return grid[nx, ny];
                }
            }
        }

        return startNode;
    }

    public Node GetNearestNode(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - gridOrigin.x) / config.nodeSize);
        int y = Mathf.RoundToInt((worldPos.y - gridOrigin.y) / config.nodeSize);

        x = Mathf.Clamp(x, 0, grid.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, grid.GetLength(1) - 1);

        if (grid[x, y].walkable)
            return grid[x, y];

        Queue<(int, int)> queue = new Queue<(int, int)>();
        HashSet<(int, int)> visited = new HashSet<(int, int)>();

        queue.Enqueue((x, y));
        visited.Add((x, y));

        while (queue.Count > 0)
        {
            (int cx, int cy) = queue.Dequeue();

            foreach (var dir in directions)
            {
                int nx = Mathf.Clamp(cx + Mathf.RoundToInt(dir.x), 0, grid.GetLength(0) - 1);
                int ny = Mathf.Clamp(cy + Mathf.RoundToInt(dir.y), 0, grid.GetLength(1) - 1);

                if (!visited.Contains((nx, ny)))
                {
                    visited.Add((nx, ny));

                    if (grid[nx, ny].walkable)
                        return grid[nx, ny];

                    queue.Enqueue((nx, ny));
                }
            }
        }

        return grid[x, y];
    }


    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        foreach (var dir in directions)
        {
            int checkX = node.gridX + Mathf.RoundToInt(dir.x);
            int checkY = node.gridY + Mathf.RoundToInt(dir.y);

            if (checkX >= 0 && checkX < grid.GetLength(0) && checkY >= 0 && checkY < grid.GetLength(1))
            {
                Node neighbor = grid[checkX, checkY];

                bool isDiagonal = dir.x != 0 && dir.y != 0;
                if (isDiagonal)
                {
                    Node horizontalNeighbor = grid[node.gridX + Mathf.RoundToInt(dir.x), node.gridY];
                    Node verticalNeighbor = grid[node.gridX, node.gridY + Mathf.RoundToInt(dir.y)];

                    if (!horizontalNeighbor.walkable || !verticalNeighbor.walkable)
                        continue;
                }

                Vector2 belowNodePos = node.worldPosition + Vector2.down * config.nodeSize;
                Vector2 belowNeighborPos = neighbor.worldPosition + Vector2.down * config.nodeSize;

                bool hasGroundBelowCurrent = Physics2D.OverlapCircle(belowNodePos, config.nodeSize, config.platformLayer);
                bool hasGroundBelowNeighbor = Physics2D.OverlapCircle(belowNeighborPos, config.nodeSize, config.platformLayer);

                if (dir.y < 0 || (neighbor.walkable && (hasGroundBelowNeighbor || dir.y > 0)) ||
                    (dir.x != 0 && !hasGroundBelowCurrent && hasGroundBelowNeighbor))
                    neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Gizmos.color = grid[x, y].walkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(grid[x, y].worldPosition, Vector3.one * config.nodeSize * 0.8f);
            }
        }
    }
}