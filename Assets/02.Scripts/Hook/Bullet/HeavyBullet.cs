public class HeavyBullet : HookBullet
{
    private void Awake()
    {
        BulletDeleteEffectPath = HeavyBulletDeleteEffectPath;
        bulletDeleteTime = HeavyBulletDeleteTime;
        currentBulletSpeed = HeavyBulletSpeed;
    }
}
