public class LastHeavyBullet : HookBullet
{
    private void Awake()
    {
        BulletDeleteEffectPath = LastHeavyBulletDeleteEffectPath;
        bulletDeleteTime = HeavyBulletDeleteTime;
        currentBulletSpeed = HeavyBulletSpeed;
    }
}
