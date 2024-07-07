using UnityEngine;

public class HookEffectController : EffectController
{
    private enum EffectName
    {
        SkillOn,
        SkillOff,
        LastHeavyAttackSmoke,
        JumpSmoke,
    }

    public void EnableSkillOnEffect()
    {
        _effects[(int)EffectName.SkillOn].SetActive(true);
    }

    public void DisableSkillOnEffect() => _effects[(int)EffectName.SkillOn].SetActive(false);
    public void EnableSkillOffEffect() => _effects[(int)EffectName.SkillOff].SetActive(true);


    public void EnableLastHeavyAttackSmoke() => _effects[(int)EffectName.LastHeavyAttackSmoke].SetActive(true);
    public void EnableJumpSmoke() => _effects[(int)EffectName.JumpSmoke].SetActive(true);
}
