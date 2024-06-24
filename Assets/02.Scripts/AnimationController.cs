using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    public BoxCollider _weaponCollider;

    public readonly int IsIdle = Animator.StringToHash("IsIdle");
    public readonly int IsJumpingUp = Animator.StringToHash("IsJumpingUp");
    public readonly int IsJumpingDown = Animator.StringToHash("IsJumpingDown");
    public readonly int IsLanding = Animator.StringToHash("IsLanding");
    public readonly int IsJumpAttacking = Animator.StringToHash("IsJumpAttacking");
    public readonly int IsLightLanding = Animator.StringToHash("IsLightLanding");
    public readonly int IsHeavyAttacking = Animator.StringToHash("IsHeavyAttacking");
    public readonly int IsJumpHeavyAttacking = Animator.StringToHash("IsJumpHeavyAttacking");
    public readonly int IsHeavyLanding = Animator.StringToHash("IsHeavyLanding");
    public readonly int IsRunning = Animator.StringToHash("IsRunning");
    public readonly int IsComboAttack1 = Animator.StringToHash("IsComboAttack1");
    public readonly int IsComboAttack2 = Animator.StringToHash("IsComboAttack2");
    public readonly int IsComboAttack3 = Animator.StringToHash("IsComboAttack3");
    public readonly int IsSkillAttack = Animator.StringToHash("IsSkillAttack");

    private PlayerController Player;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Player = GetComponent<PlayerController>();
        _weaponCollider = GetComponentInChildren<BoxCollider>();
        if (_weaponCollider == null)
        {
            Debug.LogError("No BoxCollider found in children");
        }
    }

    public void SetBool(int parameter, bool value)
    {
        _animator.SetBool(parameter, value);
    }

    public bool GetBool(int parameter)
    {
        return _animator.GetBool(parameter);
    }

    public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
    {
        return _animator.GetCurrentAnimatorStateInfo(layerIndex);
    }

    public void StartAttackMovingAnimationEvent()
    {
        Player.StartAttackMove();
    }

    public void CanMoveAnimationEvent()
    {
        Player.CanMove = true;
        Player.CanLook = true;
    }

    public void CanChangeAnimationEvent()
    {
        Player.CanChange = true;
        Player.CanLook = true;
    }

    public void WeaponColliderEnable()
    {
        _weaponCollider.enabled = true;
    }

    public void WeaponColliderDisable()
    {
        _weaponCollider.enabled = false;
    }

    public void SkillLastAttackDamage()
    {
        Player.DamageAmount = (Player._skillAttackDamage) / 5 + 500;

    }
}
