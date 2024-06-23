using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;

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

    private PlayerController _playerController;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
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
        _playerController.StartAttackMove(8f, 0.5f);
    }

    public void CanMoveAnimationEvent()
    {
        _playerController.CanMove = true;
        _playerController.CanLook = true;
    }

    public void CanChangeAnimationEvent()
    {
        _playerController.CanChange = true;
        _playerController.CanLook = true;
    }
}
