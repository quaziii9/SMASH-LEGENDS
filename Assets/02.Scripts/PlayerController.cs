using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5.4f; // 피터의 이동속도
    public float jumpForce = 14.28f; // 피터의 점프가속도
    private float gravityScale = 36f; // 피터의 중력가속도
    private float maxFallSpeed = 20f; // 피터의 최대 낙하속도
    public GameObject groundCheck;

    private float groundCheckDistance = 1f; // 지면 체크를 위한 거리

    public Rigidbody _rigidBody;
    public Animator _animator;
    public Vector3 _moveDirection;
    public bool _isGrounded;

    private float _attackMoveDistance; // 공격 중 이동할 거리
    private Vector3 _attackMoveDirection; // 공격 중 이동 방향
    private float _attackMoveDuration = 0.3f; // 공격 중 이동하는 데 걸리는 시간
    private float _attackMoveStartTime; // 공격 중 이동 시작 시간
    private float _currentMoveDistance; // 현재까지 이동한 거리

    [Header("Attack")]
    private float skillCoolTime = 4f;

    public IState _curState;
    public bool CanMove { get; set; }
    public bool CanLook { get; set; }
    public bool CanChange { get; set; }

    public readonly int IsIdle = Animator.StringToHash("IsIdle");
    public readonly int IsJumpingUp = Animator.StringToHash("IsJumpingUp");
    public readonly int IsJumpingDown = Animator.StringToHash("IsJumpingDown");
    public readonly int IsLanding = Animator.StringToHash("IsLanding");
    public readonly int IsJumpAttacking = Animator.StringToHash("IsJumpAttacking");
    public readonly int IsLightLanding = Animator.StringToHash("IsLightLanding");
    public readonly int IsHeavyAttacking = Animator.StringToHash("IsHeavyAttacking");
    public readonly int IsJumpHeavyAttacking = Animator.StringToHash("IsJumpHeavyAttacking");
    public readonly int IsRunning = Animator.StringToHash("IsRunning");
    public readonly int IsComboAttack1 = Animator.StringToHash("IsComboAttack1");
    public readonly int IsComboAttack2 = Animator.StringToHash("IsComboAttack2");
    public readonly int IsComboAttack3 = Animator.StringToHash("IsComboAttack3");
    public readonly int IsSkillAttack = Animator.StringToHash("IsSkillAttack");


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ChangeState(new IdleState(this));
    }

    private void FixedUpdate()
    {
        if (CanMove)
        {
            Move();
        }
        ApplyCustomGravity();
        CheckGroundStatus();
    }

    private void Update()
    {
        if (skillCoolTime > 0)
        {
            skillCoolTime -= Time.deltaTime;
        }
        _curState?.ExecuteOnUpdate();
    }

    public void ChangeState(IState newState)
    {
        _curState?.Exit();
        _curState = newState;
        _curState.Enter();
    }

    public void BindInputCallback(bool isBind, Action<InputAction.CallbackContext> callback)
    {
        if (callback == null)
        {
            Debug.LogError("Callback is null!");
            return;
        }

        var inputActions = GetComponent<PlayerInput>().actions;
        foreach (var action in inputActions)
        {
            if (isBind)
            {
                action.performed += callback;
                action.canceled += callback;
            }
            else
            {
                action.performed -= callback;
                action.canceled -= callback;
            }
        }
    }


    #region InputSystem
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _moveDirection = context.performed ? new Vector3(input.x, 0, input.y) : Vector3.zero;

        _curState?.OnInputCallback(context);
    }


    public void OnDefaultAttackInput(InputAction.CallbackContext context)
    {
        _curState?.OnInputCallback(context);
    }

    public void OnHeavyAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isGrounded)
            {
                _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
                ChangeState(new HeavyAttackState(this));
            }
            else
            {
                ChangeState(new JumpHeavyAttackState(this));
            }
        }
    }


    public void OnSkillAttackInput(InputAction.CallbackContext context)
    {
        _curState?.OnInputCallback(context);
    }

    #endregion

    public void Move()
    {
        if (_curState is JumpLightLandingState)
        {
            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
            return;
        }

        if (_curState is ComboAttack1State || _curState is ComboAttack2State || _curState is ComboAttack3State || _curState is JumpHeavyAttackState || _curState is HeavyAttackState || _curState is SkillAttackState)
        {
            // 공격 중 이동 처리
            float elapsedTime = Time.time - _attackMoveStartTime;
            float fraction = elapsedTime / _attackMoveDuration;
            float distanceToMove = Mathf.Lerp(0, _attackMoveDistance, fraction);

            Vector3 forwardMovement = _attackMoveDirection * (distanceToMove - _currentMoveDistance);
            _rigidBody.MovePosition(_rigidBody.position + forwardMovement);

            _currentMoveDistance = distanceToMove;

            return;
        }

        if (CanLook && _moveDirection != Vector3.zero)
        {
            LookAt();
        }

        Vector3 velocity = new Vector3(_moveDirection.x * moveSpeed, _rigidBody.velocity.y, _moveDirection.z * moveSpeed);
        _rigidBody.velocity = velocity;
    }


    public void LookAt()
    {
        if (_moveDirection != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(_moveDirection);
            _rigidBody.rotation = targetAngle;
        }
    }


    public void Jump()
    {
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, jumpForce, _rigidBody.velocity.z);
    }


    private void ApplyCustomGravity()
    {
        if (!_isGrounded)
        {
            _rigidBody.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);

            if (_rigidBody.velocity.y < 0)
            {
                _rigidBody.velocity += Vector3.up * gravityScale * 0.5f * Time.fixedDeltaTime; // 감속 비율 적용
            }
        }

        // 최대 낙하속도를 초과하지 않도록 제한
        if (_rigidBody.velocity.y < -maxFallSpeed)
        {
            _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, -maxFallSpeed, _rigidBody.velocity.z);
        }
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
        Vector3 origin = groundCheck.transform.position; // 플레이어의 위치에서 약간 위쪽
        _isGrounded = Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance);
    }

    private void OnDrawGizmosSelected()
    {
        // 디버그를 위해 지면 체크 레이캐스트를 시각적으로 표시
        Gizmos.color = Color.red;
        Vector3 origin = groundCheck.transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance));
    }


    //public void IdleAnimationEvent()
    //{
    //    _isLanding = false;
    //    _canCombo = false;
    //    //_animator.ResetTrigger("IsAttack");
    //    _isAttack = false;
    //    _isJumping = false;
    //    _hasAirAttacked = false;
    //    //_animator.ResetTrigger("AirAttack");
    //    _animator.ResetTrigger("HeavyAttack");
    //    _animator.ResetTrigger("SkillAttack");
    //    _animator.ResetTrigger("AirHeavyAttack");
    //}

    public void CanMoveAnimationEvent()
    {
        CanMove = true;
    }
    public void LookTrueAnimationEvent()
    {
        CanLook = true;
    }

    // 애니메이션 이벤트를 통해 콤보 가능 상태로 만들기
    public void EnableComboAnimationEvent()
    {
        //_canChangeAnimation = false;
        //_canCombo = true;
        //_isAttack = true;
    }

    // 점프 애니메이션 이벤트
    public void JumpLandAnimationEvent()
    {
        //_hasAirAttacked = false;
    }

    public void StartAttackMove(float distance)
    {
        //_isAttack = true;
        _attackMoveDistance = distance;
        _attackMoveStartTime = Time.time;
        _currentMoveDistance = 0;
        _attackMoveDirection = transform.forward; // 현재 보고 있는 방향으로 이동
    }
}
