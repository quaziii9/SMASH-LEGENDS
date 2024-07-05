using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    public BoxCollider _weaponCollider;
    public AttackController _attackController;

    public readonly int IsIdle = Animator.StringToHash("IsIdle");
    public readonly int IsRunning = Animator.StringToHash("IsRunning");
    public readonly int IsJumpingUp = Animator.StringToHash("IsJumpingUp");
    public readonly int IsJumpingDown = Animator.StringToHash("IsJumpingDown");
    public readonly int IsLanding = Animator.StringToHash("IsLanding");
    public readonly int IsJumpAttacking = Animator.StringToHash("IsJumpAttacking");
    public readonly int IsLightLanding = Animator.StringToHash("IsLightLanding");
    public readonly int IsHeavyAttacking = Animator.StringToHash("IsHeavyAttacking");
    public readonly int IsJumpHeavyAttacking = Animator.StringToHash("IsJumpHeavyAttacking");
    public readonly int IsHeavyLanding = Animator.StringToHash("IsHeavyLanding");
    public readonly int IsComboAttack1 = Animator.StringToHash("IsComboAttack1");
    public readonly int IsComboAttack2 = Animator.StringToHash("IsComboAttack2");
    public readonly int IsComboAttack3 = Animator.StringToHash("IsComboAttack3");
    public readonly int IsSkillAttack = Animator.StringToHash("IsSkillAttack");

    public readonly int IsHit = Animator.StringToHash("IsHit");
    public readonly int IsHitUp = Animator.StringToHash("IsHitUp");
    public readonly int IsHitDown = Animator.StringToHash("IsHitDown");
    public readonly int IsHitLand = Animator.StringToHash("IsHitLand");

    public readonly int IsDownIdle = Animator.StringToHash("IsDownIdle");
    public readonly int IsRollUpFront = Animator.StringToHash("IsRollUpFront");
    public readonly int IsRollUpBack = Animator.StringToHash("IsRollUpBack");
    public readonly int IsStandUp = Animator.StringToHash("IsStandUp");

    public readonly int IsHang = Animator.StringToHash("IsHang");
    public readonly int IsHangFalling = Animator.StringToHash("IsHangFalling");


    private PlayerController Player;

    [SerializeField] private GameObject _skillAttackHitZone;
    [SerializeField] private GameObject _attackHitZone;
    [SerializeField] private GameObject _heavyAttackHitZone;
    [SerializeField] private GameObject _jumpAttackHitZone;
    [SerializeField] private GameObject _finishDefaultAttackHitZone;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Player = GetComponent<PlayerController>();
        _weaponCollider = GetComponentInChildren<BoxCollider>();
        _attackController = GetComponentInChildren<AttackController>();
        if (_weaponCollider == null)
        {
            Debug.LogError("No BoxCollider found in children");
        }
    }

    public void SetBool(int parameter, bool value)
    {
        if (_animator != null)
        {
            _animator.SetBool(parameter, value);
        }
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
        _attackController.StartAttackMove();
    }

    public void CanMoveAnimationEvent()
    {
        Player.CanMove = true;
        Player.CanLook = true;
    }

    public void CanChangeAnimationEvent()
    {
        if(Player.StateController.CurState != PlayerState.FinishAttack)
            Player.CanDefaultFlash = 0;
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

    private void EnableAttackHitZone() => _attackHitZone.SetActive(true);
    private void DisableAttackHitZone() => _attackHitZone.SetActive(false);
    private void EnableFinishAttackHitZone() => _finishDefaultAttackHitZone.SetActive(true);
    private void DisableFinishAttackHitZone() => _finishDefaultAttackHitZone.SetActive(false);
    private void EnableJumpAttackHitZone() => _jumpAttackHitZone.SetActive(true);
    private void DisableJumpAttackHitZone() => _jumpAttackHitZone.SetActive(false);
    private void EnableHeavyAttackHitZone() => _heavyAttackHitZone.SetActive(true);
    private void DisableHeavyAttackHitZone() => _heavyAttackHitZone.SetActive(false);
    private void EnableSkillAttackHitZone() => _skillAttackHitZone.SetActive(true);
    private void DisableSkillAttackHitZone() => _skillAttackHitZone.SetActive(false);


}
