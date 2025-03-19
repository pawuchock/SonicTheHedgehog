using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private RingsPool ringsPool;
    [SerializeField] private RingFactory factory;
    [SerializeField] private int ringsToSpawn = 5;
    [SerializeField] private GridConfig config;
    public GameObject targetMarkerPrefab;
    private GameObject targetMarkerInstance;

    private void Start()
    {
        ringsPool.OnAllRingsReturned += () =>
        {
            factory.ResetSpawnPositions();
            SpawnInitialRings();
        };


        SpawnInitialRings();
    }

    private void SpawnInitialRings()
    {
        for (int i = 0; i < ringsToSpawn; i++)
        {
            Transform spawnPosition = factory.GetSpawnPosition();
            ringsPool.GetOneRing(spawnPosition);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MarkTargetPosition(mousePos);
            player.MoveTo(FindClosestIntersection(mousePos));
        }
    }

    public void MarkTargetPosition(Vector2 targetPosition)
    {
        if (targetMarkerPrefab != null && targetMarkerInstance == null)
            targetMarkerInstance = Instantiate(targetMarkerPrefab);

        Node targetNode = GridManager.Instance.GetNearestNode(FindClosestIntersection(targetPosition));
        targetMarkerInstance.transform.position = targetNode.worldPosition;
        targetMarkerInstance.SetActive(true);
    }
    
    private Vector2 FindClosestIntersection(Vector2 targetPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(targetPosition, Vector2.down, Mathf.Infinity, config.platformLayer);
    
        if (hit.collider != null)
            return hit.point + Vector2.up * config.nodeSize;
    
        return targetPosition;
    }

}