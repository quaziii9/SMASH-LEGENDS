public class SkillHeavyBullet : HookBullet
{
    private void Awake()
    {
        BulletDeleteEffectPath = SkillBulletDeleteEffectPath;
        bulletDeleteTime = SkillHeavyBulletDeleteTime;
        currentBulletSpeed = HeavyBulletSpeed;
    }
}
