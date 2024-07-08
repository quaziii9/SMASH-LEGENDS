using UnityEngine;
using UnityEngine.Pool;

public class HookBullet : MonoBehaviour
{
    protected GameObject bulletDeleteEffectPrefab;
    protected const float DefaultBulletSpeed = 20f;
    protected const float HeavyBulletSpeed = 28f;
    protected const float DefaultBulletDeleteTime = 0.23f;
    protected const float HeavyBulletDeleteTime = 0.28f;
    protected const float SkillBulletDeleteTime = 0.28f;
    protected const float SkillHeavyBulletDeleteTime = 0.3f;

    protected readonly string HeavyBulletDeleteEffectPath = "Prefab/Hook/Hook_Ingame/Hook_Heavy_Bullet_Delete_Effect";
    protected readonly string LastHeavyBulletDeleteEffectPath = "Prefab/Hook/Hook_Ingame/Hook_Last_Heavy_Bullet_Delete_Effect";
    protected readonly string SkillBulletDeleteEffectPath = "Prefab/Hook/Hook_Ingame/Hook_SKill_Bullet_Delete_Effect";
    protected string BulletDeleteEffectPath = "Prefab/Hook/Hook_Ingame/Hook_Default_Bullet_Delete_Effect";

    protected float bulletDeleteTime = 0.23f;
    protected float currentBulletSpeed = 20;
    protected float skillBulletSpeed = 30;

    public ObjectPool<HookBullet> Pool { get; set; }
    internal GameObject constructor;

    private ObjectPool<BulletDelete> _bulletDeleteEffectPool;
    private float _elapsedTime;

    private void Start()
    {
        bulletDeleteEffectPrefab = Resources.Load<GameObject>(BulletDeleteEffectPath);
        _bulletDeleteEffectPool = new ObjectPool<BulletDelete>(CreateBulletDeleteEffectOnPool, GetPoolBulletDeleteEffect, ReturnBulletDeleteEffect, (effect) => Destroy(effect.gameObject), true, 10, 500);
    }

    private void Update()
    {
        BulletDirection();
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= bulletDeleteTime)
        {
            BulletPostProcessing(transform.position);
        }
    }

    private void BulletDirection()
    {
        transform.Translate(Vector3.forward * (currentBulletSpeed * Time.deltaTime));
    }

    public void BulletPostProcessing(Vector3 position)
    {
        if (gameObject.activeSelf)
        {
            Pool.Release(this);
        }
        BulletDelete effect = _bulletDeleteEffectPool.Get();
        effect.transform.SetParent(constructor.transform, false); // 부모 설정
        effect.transform.position = position;
        effect.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        _elapsedTime = 0;
    }

    private BulletDelete CreateBulletDeleteEffectOnPool()
    {
        GameObject effectObject = Instantiate(bulletDeleteEffectPrefab, constructor.transform); // 부모 설정
        BulletDelete effect = effectObject.AddComponent<BulletDelete>();
        effect.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        effect.Pool = _bulletDeleteEffectPool;
        return effect;
    }

    private void GetPoolBulletDeleteEffect(BulletDelete effect)
    {
        effect.gameObject.SetActive(true);
        effect.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
    }

    private void ReturnBulletDeleteEffect(BulletDelete effect) => effect.gameObject.SetActive(false);
}
