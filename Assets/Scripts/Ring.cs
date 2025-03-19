using UnityEngine;

public class Ring : MonoBehaviour
{
    private RingsPool ringsPool;

    public void Initialize(RingsPool pool)
    {
        ringsPool = pool;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            ringsPool.ReturnRing(this);
    }
}