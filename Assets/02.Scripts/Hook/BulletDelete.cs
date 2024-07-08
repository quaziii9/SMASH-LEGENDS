using UnityEngine;
using UnityEngine.Pool;

public class BulletDelete : MonoBehaviour
{
    public ObjectPool<BulletDelete> Pool { private get; set; }

    private void OnDisable()
    {
        Pool.Release(this);
    }
}
