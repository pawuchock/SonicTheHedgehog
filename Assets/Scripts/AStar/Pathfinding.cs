using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;
    [SerializeField] private GridConfig config;
    private void Awake()
    {
        Instance = this;
    }

    public List<Node> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = GridManager.Instance.GetNearestNode(startPos);
        Node targetNode = GridManager.Instance.GetNearestNode(targetPos);

        if (!startNode.walkable)
            startNode = GridManager.Instance.FindClosestWalkableNode(startNode);

        if (!targetNode.walkable)
            targetNode = GridManager.Instance.FindClosestWalkableNode(targetNode);


        if (!startNode.walkable || !targetNode.walkable) return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GridManager.Instance.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.gridX - b.gridX);
        int distY = Mathf.Abs(a.gridY - b.gridY);

        int horizontalCost = 10;
        int upwardCost = 30;
        int downwardCost = 5;
        int floatingPenalty = 25;
        int directionChangePenalty = 15;

        bool isHorizontalMove = (distX > 0 && distY == 0);
        bool isVerticalMove = (distY > 0 && distX == 0);

        int cost = isHorizontalMove
            ? (distX * horizontalCost)
            : (distX * horizontalCost) + (distY * (b.gridY > a.gridY ? upwardCost : downwardCost));

        bool isFloating = !Physics2D.OverlapCircle(b.worldPosition + Vector2.down * config.nodeSize, config.nodeSize * 0.45f, config.platformLayer);

        if (isFloating)
            cost += floatingPenalty;

        if (!isHorizontalMove && !isVerticalMove)
            cost += directionChangePenalty;

        return cost;
    }
}