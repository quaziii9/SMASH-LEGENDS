using System;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

public class PlayerController : NetworkBehaviour
{
    private float moveSpeed = 5.4f; // 피터의 이동속도
    private float jumpMoveSpeed = 2.2f; // 점프 중 이동 속도

    public Vector3 _moveDirection;

    [Header("GroundCheck")]
    public GameObject groundCheck;
    private float groundCheckDistance = 1f; // 지면 체크를 위한 거리
    public bool _isGrounded;
    public bool _Ground;

    public Rigidbody _rigidbody;
    public AnimationController _animationController;

    [SyncVar] public float _playerHp = 10000;

    [Header("State")]
    [SyncVar(hook = nameof(OnStateChanged))] public PlayerState _curState;
    public bool CanMove { get; set; }
    public bool CanLook { get; set; }
    public bool CanChange { get; set; }
    public bool IsMoveInputActive { get; private set; }
    public bool IsHitted;

    public bool PositionSet = false;

    private bool isJumping = false;
    private bool isIdleJump = false;

    [Header("Detection")]
    public LayerMask playerLayer; // 플레이어가 속한 레이어

    public AttackController _attackController;

    private IState _currentStateInstance; // 현재 상태 인스턴스

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animationController = GetComponent<AnimationController>();
        _attackController = GetComponent<AttackController>(); // AddComponent 대신 GetComponent 사용
        if (_attackController != null)
        {
            _attackController.Initialize(this);
        }
        else
        {
            Debug.LogError("AttackController is not attached to the PlayerController gameObject");
        }
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            ChangeState(PlayerState.Idle);
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Move();
            ApplyCustomGravity();
            CheckGroundStatus();
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            _attackController.UpdateCooldowns();
            _currentStateInstance?.ExecuteOnUpdate();
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (_currentStateInstance == null || _currentStateInstance.IsTransitioning)
        {
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            _currentStateInstance.OnInputCallback(new InputAction.CallbackContext());
        }

        Vector2 moveInput = Vector2.zero;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            moveInput.y = 1;
        }
        else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            moveInput.y = -1;
        }

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            moveInput.x = -1;
        }
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            moveInput.x = 1;
        }

        _moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        _currentStateInstance?.ExecuteOnUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        _currentStateInstance?.Exit(); // 현재 상태의 Exit 호출

        _curState = newState;
        CmdUpdateState(newState);

        _currentStateInstance = CreateStateInstance(newState); // 새로운 상태 인스턴스 생성
        _currentStateInstance?.Enter(); // 새로운 상태의 Enter 호출
    }

    private IState CreateStateInstance(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                return new IdleState(this, _attackController);
            case PlayerState.Run:
                return new RunState(this);
            case PlayerState.JumpUp:
                return new JumpUpState(this);
            case PlayerState.JumpDown:
                return new JumpDownState(this);
            case PlayerState.JumpLand:
                return new JumpLandState(this);
            case PlayerState.JumpAttack:
                return new JumpAttackState(this);
            case PlayerState.JumpHeavyAttack:
                return new JumpHeavyAttackState(this);
            case PlayerState.JumpHeavyAttackLanding:
                return new JumpHeavyAttackLandingState(this);
            case PlayerState.JumpAttackLanding:
                return new JumpAttackLandingState(this);
            case PlayerState.SkillAttack:
                return new SkillAttackState(this);
            case PlayerState.FirstAttack:
                return new FirstAttackState(this);
            case PlayerState.SecondAttack:
                return new SecondAttackState(this);
            case PlayerState.FinishAttack:
                return new FinishAttackState(this);
            case PlayerState.HeavyAttack:
                return new HeavyAttackState(this);
            case PlayerState.Hit:
                return new HitState(this);
            case PlayerState.HitUp:
                return new HitUpState(this);
            case PlayerState.HitDown:
                return new HitDownState(this);
            case PlayerState.HitLand:
                return new HitLandState(this);
            case PlayerState.DownIdle:
                return new DownIdleState(this);
            case PlayerState.RollUpFront:
                return new RollUpFrontState(this);
            case PlayerState.RollUpBack:
                return new RollUpBackState(this);
            case PlayerState.StandUp:
                return new StandUpState(this);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _Ground = true;
        }
    }

    [Command]
    void CmdUpdateState(PlayerState newState)
    {
        _curState = newState;
    }

    void OnStateChanged(PlayerState oldState, PlayerState newState)
    {
        _attackController.HandleAttack(newState); // 상태 변경 시 공격 값 설정
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
        if (!isLocalPlayer) return;

        Vector2 input = context.ReadValue<Vector2>();
        _moveDirection = context.performed ? new Vector3(input.x, 0, input.y) : Vector3.zero;
        IsMoveInputActive = context.performed;
    }

    public void OnDefaultAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            _currentStateInstance?.OnInputCallback(context);
        }
    }

    public void OnHeavyAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (_attackController.currentHeavyAttackCoolTime <= 0 && context.performed)
        {
            _currentStateInstance?.OnInputCallback(context);
        }
    }

    public void OnSkillAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            _currentStateInstance?.OnInputCallback(context);
        }
    }
    #endregion

    public void Move()
    {
        if (PositionSet == false) return;
        if (IsHitted) return;
        if (!isLocalPlayer) return;

        if (_currentStateInstance is FirstAttackState || _currentStateInstance is SecondAttackState || _currentStateInstance is FinishAttackState ||
            _currentStateInstance is JumpHeavyAttackState || _currentStateInstance is HeavyAttackState || _currentStateInstance is SkillAttackState ||
            _currentStateInstance is RollUpBackState || _currentStateInstance is RollUpFrontState)
        {
            _attackController.HandleAttackMove();
            return;
        }

        if (CanLook && _moveDirection != Vector3.zero)
        {
            LookAt();
        }

        if (CanMove)
        {
            float currentMoveSpeed = isJumping && isIdleJump ? jumpMoveSpeed : moveSpeed;
            Vector3 velocity = new Vector3(_moveDirection.x * currentMoveSpeed, _rigidbody.velocity.y, _moveDirection.z * currentMoveSpeed);
            _rigidbody.velocity = velocity;
        }
    }

    public void LookAt()
    {
        if (_moveDirection != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(_moveDirection);
            _rigidbody.rotation = targetAngle;
        }
    }

    public void Jump(bool idleJump = false)
    {
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _attackController.jumpForce, _rigidbody.velocity.z);
        isJumping = true;
        isIdleJump = idleJump;
    }

    public void Land()
    {
        isJumping = false;
        isIdleJump = false;
    }

    private void ApplyCustomGravity()
    {
        if (!_isGrounded)
        {
            _rigidbody.AddForce(Vector3.down * _attackController.gravityScale, ForceMode.Acceleration);

            if (_rigidbody.velocity.y < 0)
            {
                _rigidbody.velocity += Vector3.up * _attackController.gravityScale * 0.5f * Time.fixedDeltaTime;
            }
        }

        if (_rigidbody.velocity.y < -_attackController.maxFallSpeed)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -_attackController.maxFallSpeed, _rigidbody.velocity.z);
        }
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
        Vector3 origin = groundCheck.transform.position;
        _isGrounded = Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = groundCheck.transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackController.detectionRadius);
    }

    [Command]
    public void CmdHitted(float damaged, float knockBackPower, Vector3 knockBackDirection, HitType hitType)
    {
        RpcHitted(damaged, knockBackPower, knockBackDirection, hitType);
    }

    [ClientRpc]
    public void RpcHitted(float damaged, float knockBackPower, Vector3 knockBackDirection, HitType hitType)
    {
        PlayerGetDamaged(damaged);
        _attackController.PlayerGetKnockBack(knockBackPower, knockBackDirection, hitType);
    }

    private void PlayerGetDamaged(float damaged)
    {
        _playerHp -= damaged;
    }

    public void Hitted(float damaged, float knockBackPower, Vector3 attackerPosition, Vector3 attackerDirection, HitType hitType)
    {
        Vector3 direction = (transform.position - attackerPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(-direction);
        transform.rotation = lookRotation;

        Vector3 knockBackDirection = -transform.forward;
        knockBackDirection.y = attackerDirection.y;
        knockBackDirection.x = knockBackDirection.x >= 0 ? 1 : -1;

        CmdHitted(damaged, knockBackPower, knockBackDirection, hitType);
    }

    private void RotateTowardsAttacker(Vector3 attackerPosition)
    {
        Vector3 direction = (attackerPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }
    }
}
