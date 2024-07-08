using UnityEngine;
using UnityEngine.Pool;

public class HookBullet : MonoBehaviour
{
    protected BulletDelete bulletDeleteEffect;
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

    public ObjectPool<HookBullet> Pool { get; set; }
    internal GameObject constructor;

    private ObjectPool<BulletDelete> _bulletDeleteEffectPool;
    private float _elapsedTime;

    private void Start()
    {
        bulletDeleteEffect = Resources.Load<BulletDelete>(BulletDeleteEffectPath);
        _bulletDeleteEffectPool = new ObjectPool<BulletDelete>(CreateBulletDeleteEffectOnPool,
            GetPoolBulletDeleteEffect, ReturnBulletDeleteEffect, (effect) => Destroy(effect), true, 10, 500);
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
        if (gameObject.activeSelf == true)
        {
            Pool.Release(this);
        }
        BulletDelete effect = _bulletDeleteEffectPool.Get();
        effect.transform.position = position;
        _elapsedTime = 0;
    }
    private BulletDelete CreateBulletDeleteEffectOnPool()
    {
        BulletDelete effect = Instantiate(bulletDeleteEffect);
        effect.Pool = _bulletDeleteEffectPool;
        return effect;
    }
    private void GetPoolBulletDeleteEffect(BulletDelete effect) => effect.gameObject.SetActive(true);
    private void ReturnBulletDeleteEffect(BulletDelete effect) => effect.gameObject.SetActive(false);
}
