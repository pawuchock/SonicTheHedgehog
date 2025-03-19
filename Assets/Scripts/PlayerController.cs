using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private GridConfig gridConfig;
    private int currentPathIndex;
    private List<Node> path;

    private void RotateVisual(Vector2 target)
    {
        transform.localScale = transform.position.x < target.x
            ? new Vector3(-0.5f, transform.localScale.y, transform.localScale.z)
            : new Vector3(0.5f, transform.localScale.y, transform.localScale.z);
    }

    public void MoveTo(Vector2 target)
    {
        RotateVisual(target);
        path = Pathfinding.Instance.FindPath(transform.position, target);

        if (path == null)
        {
            Debug.Log("No path found");
            return;
        }

        currentPathIndex = 0;
        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        while (currentPathIndex < path.Count)
        {
            Vector2 targetPos = path[currentPathIndex].worldPosition;
            float diff = targetPos.x - transform.position.x;
            
            if (targetPos.y > transform.position.y && Mathf.Abs(diff) <= gridConfig.nodeSize)
            {
                var cachedCurrentX = transform.position.x;
                
                while (Math.Abs(targetPos.x - cachedCurrentX) < gridConfig.nodeSize)
                {
                    currentPathIndex++;
                    targetPos = path[currentPathIndex].worldPosition;
                }

                yield return StartCoroutine(JumpToTarget(transform.position, targetPos, playerConfig.jumpDuration));
            }
            else
            {
                float fallMultiplier = targetPos.y < transform.position.y ? playerConfig.fallSpeedMultiplier : 1.0f;

                while (Vector2.Distance(transform.position, targetPos) > playerConfig.stoppingDistance)
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPos, playerConfig.speed * fallMultiplier * Time.fixedDeltaTime);
                    yield return new WaitForFixedUpdate();
                }
            }

            currentPathIndex++;
        }
    }

    IEnumerator JumpToTarget(Vector2 startPos, Vector2 targetPos, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            Vector2 horizontalPos = Vector2.Lerp(startPos, targetPos, t);

            float height = Mathf.Sin(t * Mathf.PI);

            transform.position = new Vector2(horizontalPos.x, horizontalPos.y + height);

            yield return null;
        }

        transform.position = targetPos;
    }

    private void OnDrawGizmos()
    {
        if (path == null) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
        }
    }
}