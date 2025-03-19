using System.Collections.Generic;
using UnityEngine;

public class RingsPool : MonoBehaviour
{
    [SerializeField] private RingFactory ringFactory;

    private Queue<Ring> ringsPool = new Queue<Ring>();
    private int activeRings = 0;

    public System.Action OnAllRingsReturned;

   public Ring GetOneRing(Transform transform)
    {
        if (ringsPool.Count == 0)
            PopulatePool();

        Ring ring = ringsPool.Dequeue();
        ring.gameObject.SetActive(true);
        ring.transform.position = transform.position;
        ring.transform.SetParent(transform);
        activeRings++;
        return ring;
    }

    public void ReturnRing(Ring ring)
    {
        ring.gameObject.SetActive(false);
        ringsPool.Enqueue(ring);
        activeRings--;

        if (activeRings == 0)
            OnAllRingsReturned?.Invoke();
    }

    private void PopulatePool()
    {
        Ring newRing = ringFactory.CreateRing();
        newRing.Initialize(this);
        newRing.gameObject.SetActive(false);
        ringsPool.Enqueue(newRing);
    }
}