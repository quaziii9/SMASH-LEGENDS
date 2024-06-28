using System;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Controller")]
    public StatController _statController; // StatController 추가
    public AnimationController _animationController;
    public AttackController _attackController;
    public StateController _stateController;

    public Vector3 _moveDirection;
    public Rigidbody _rigidbody;

    [Header("GroundCheck")]
    public GameObject groundCheck;
    private float groundCheckDistance = 1f; // 지면 체크를 위한 거리
    public bool _isGrounded;
    public bool _Ground;

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

    private Vector3 _rollUpMoveDirection;
    private float _rollUpMoveDistance = 2.5f;
    private float _rollUpMoveDuration = 0.3f;
    private float _rollUpMoveStartTime;
    private float _currentMoveDistance;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animationController = GetComponent<AnimationController>();
        _attackController = GetComponent<AttackController>();
        _stateController = GetComponent<StateController>();
        _statController = GetComponent<StatController>();

        if (_attackController != null)
        {
            _attackController.Initialize(this, _statController);
        }
        else
        {
            Debug.LogError("AttackController is not attached to the PlayerController gameObject");
        }

        if (_stateController != null)
        {
            _stateController.Initialize(this, _attackController);
        }
        else
        {
            Debug.LogError("StateController is not attached to the PlayerController gameObject");
        }
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            _stateController.ChangeState(PlayerState.Idle);
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
            _stateController.ExecuteOnUpdate();
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (_stateController.CurrentStateInstance == null || _stateController.CurrentStateInstance.IsTransitioning)
        {
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            _stateController.CurrentStateInstance.OnInputCallback(new InputAction.CallbackContext());
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

        _stateController.ExecuteOnUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        _stateController.ChangeState(newState);
    }

    private void OnStateChanged(PlayerState oldState, PlayerState newState)
    {
        _attackController.HandleAttack(newState); // 상태 변경 시 공격 값 설정
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _Ground = true;
        }
    }

    [Command]
    public void CmdUpdateState(PlayerState newState)
    {
        _curState = newState;
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
            _stateController.CurrentStateInstance?.OnInputCallback(context);
        }
    }

    public void OnHeavyAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (_attackController.currentHeavyAttackCoolTime <= 0 && context.performed)
        {
            _stateController.CurrentStateInstance?.OnInputCallback(context);
        }
    }

    public void OnSkillAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            _stateController.CurrentStateInstance?.OnInputCallback(context);
        }
    }
    #endregion

    public void Move()
    {
        if (PositionSet == false) return;
        if (IsHitted) return;
        if (!isLocalPlayer) return;

        if (_stateController.CurrentStateInstance is RollUpBackState || _stateController.CurrentStateInstance is RollUpFrontState)
        {
            HandleRollUpMOVE();
        }

        if (_stateController.CurrentStateInstance is FirstAttackState || _stateController.CurrentStateInstance is SecondAttackState || _stateController.CurrentStateInstance is FinishAttackState ||
            _stateController.CurrentStateInstance is JumpHeavyAttackState || _stateController.CurrentStateInstance is HeavyAttackState || _stateController.CurrentStateInstance is SkillAttackState)

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
            float currentMoveSpeed = isJumping && isIdleJump ? _statController.jumpMoveSpeed : _statController.moveSpeed;
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
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _statController.jumpForce, _rigidbody.velocity.z);
        isJumping = true;
        isIdleJump = idleJump;
    }

    public void Land()
    {
        isJumping = false;
        isIdleJump = false;
    }

    public void HandleRollUpMOVE()
    {
        float elapsedTime = Time.time - _rollUpMoveStartTime;
        float fraction = elapsedTime / _rollUpMoveDuration;
        float distanceToMove = Mathf.Lerp(0, _rollUpMoveDistance, fraction);

        Vector3 forwardMovement = _rollUpMoveDirection * (distanceToMove - _currentMoveDistance);
        _rigidbody.MovePosition(_rigidbody.position + forwardMovement);

        _currentMoveDistance = distanceToMove;

        if (CanLook && _moveDirection != Vector3.zero)
        {
            LookAt();
        }
    }

    public void StartRollUpMove()
    {
        if (!isLocalPlayer) return;
        _rollUpMoveStartTime = Time.time;
        _currentMoveDistance = 0;
        if (_curState == PlayerState.RollUpBack) _rollUpMoveDirection = -transform.forward;
        else
            _rollUpMoveDirection = transform.forward;
    }

    private void ApplyCustomGravity()
    {
        if (!_isGrounded)
        {
            _rigidbody.AddForce(Vector3.down * _statController.gravityScale, ForceMode.Acceleration);

            if (_rigidbody.velocity.y < 0)
            {
                _rigidbody.velocity += Vector3.up * _statController.gravityScale * 0.5f * Time.fixedDeltaTime;
            }
        }

        if (_rigidbody.velocity.y < -_statController.maxFallSpeed)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -_statController.maxFallSpeed, _rigidbody.velocity.z);
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
}
