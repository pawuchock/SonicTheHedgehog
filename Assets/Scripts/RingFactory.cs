using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RingFactory : MonoBehaviour
{
    [SerializeField] private Ring ringPrefab;
    [SerializeField] private RingsPool ringsPool;
    [SerializeField] private List<SpawnData> spawnDatas;

    public Ring CreateRing()
    {
        if (spawnDatas.Count == 0)
            return null;

        Ring newRing = Instantiate(ringPrefab);
        newRing.Initialize(ringsPool);
        return newRing;
    }

    public void ResetSpawnPositions()
    {
        spawnDatas.ForEach(data => data.isOcupied = false);
    }

    public Transform GetSpawnPosition()
    {
        List<SpawnData> freepoints = spawnDatas.Where(e => !e.isOcupied).ToList();
        SpawnData data = freepoints[Random.Range(0, freepoints.Count)];
        data.isOcupied = true;

        return data.spawnPoint;
    }

    [ContextMenu("Populate Spawn Points")] // Just to make life easier:)
    public void FindAllSpawnPoints()
    {
        spawnDatas.Clear();
        SpawnPoint[] allSpawnPoints = FindObjectsOfType<SpawnPoint>();

        foreach (SpawnPoint spawn in allSpawnPoints)
            spawnDatas.Add(new SpawnData { spawnPoint = spawn.transform, isOcupied = false });
    }
}

[System.Serializable]
class SpawnData
{
    public Transform spawnPoint;
    public bool isOcupied;
}