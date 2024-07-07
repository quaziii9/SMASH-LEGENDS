using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
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

    public readonly int IsJumpComboAttack1 = Animator.StringToHash("IsJumpComboAttack1");
    public readonly int IsJumpComboAttack2 = Animator.StringToHash("IsJumpComboAttack2");
    public readonly int IsJumpComboAttack3 = Animator.StringToHash("IsJumpComboAttack3");


    private PlayerController Player;

    [SerializeField] private GameObject _attackHitZone;
    [SerializeField] private GameObject _secondAttackHitZone;
    [SerializeField] private GameObject _heavyAttackHitZone;
    [SerializeField] private GameObject _heavyJumpAttackHitZone;
    [SerializeField] private GameObject _jumpAttackHitZone;
    [SerializeField] private GameObject _skillAttackHitZone;

    [SerializeField] private Collider _heavyJumpAttackHitZoneCollider;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Player = GetComponent<PlayerController>();
        _attackController = GetComponentInChildren<AttackController>();
        _heavyJumpAttackHitZoneCollider = _heavyJumpAttackHitZone.GetComponent<Collider>();
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
        if (Player.legendType == PlayerController.LegendType.Hook) return;
        Player.CanLook = true;
    }

    public void HookJumpHeavyAttackAnimationEvent()
    {
        _attackController.attackMoveDistance = -2.5f;
        _attackController.attackMoveDuration = 0.3f;
        Player.HookJumpHeavyAttackMove = true; // 중력과 이동을 활성화
        Player.rigidbody.velocity = new Vector3(0, Player.StatController.jumpForce, 0); // Y축 속도 설정
        Player.AttackController.StartAttackMove();
    }


    private void EnableAttackHitZone() => _attackHitZone.SetActive(true);
    private void DisableAttackHitZone() => _attackHitZone.SetActive(false);

    private void EnableSecondAttackHitZone() => _secondAttackHitZone.SetActive(true);
    private void DisableSecondAttackHitZone() => _secondAttackHitZone.SetActive(false);

    private void EnableHeavyAttackHitZone() => _heavyAttackHitZone.SetActive(true);
    private void DisableHeavyAttackHitZone() => _heavyAttackHitZone.SetActive(false);


    private void EnableHeavyJumpAttackHitZone()
    {
        _heavyJumpAttackHitZone.SetActive(true);
        _heavyJumpAttackHitZoneCollider.enabled = true;

    }
    private void DisableHeavyJumpAttackHitZone()
    {
        _heavyJumpAttackHitZone.SetActive(false);
        _heavyJumpAttackHitZoneCollider.enabled = false;
    }
   

    private void EnableJumpAttackHitZone() => _jumpAttackHitZone.SetActive(true);
    private void DisableJumpAttackHitZone() => _jumpAttackHitZone.SetActive(false);
  
    private void EnableSkillAttackHitZone() => _skillAttackHitZone.SetActive(true);
    private void DisableSkillAttackHitZone() => _skillAttackHitZone.SetActive(false);


}
