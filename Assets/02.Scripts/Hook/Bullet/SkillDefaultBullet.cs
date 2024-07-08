public class SkillDefaultBullet : HookBullet
{
    private void Awake()
    {
        BulletDeleteEffectPath = SkillBulletDeleteEffectPath;
        bulletDeleteTime = SkillBulletDeleteTime;
        currentBulletSpeed = DefaultBulletSpeed;
    }
}
