public class DefaultBullet : HookBullet
{

    private void Awake()
    {
        bulletDeleteTime = DefaultBulletDeleteTime;
        currentBulletSpeed = DefaultBulletSpeed;
    }
}
