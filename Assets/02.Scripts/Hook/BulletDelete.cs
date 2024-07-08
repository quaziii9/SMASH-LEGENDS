using UnityEngine;
using UnityEngine.Pool;

public class BulletDelete : MonoBehaviour
{
    public ObjectPool<BulletDelete> Pool { get; set; }

    private void OnDisable()
    {
        if (Pool != null)
        {
            Pool.Release(this);
        }
    }
}
