using UnityEngine;
using UnityEngine.Pool;

public class FireDelete : MonoBehaviour
{
    public ObjectPool<FireDelete> pool { private get; set; }

    private void OnDisable()
    {
        pool.Release(this);
    }
}
