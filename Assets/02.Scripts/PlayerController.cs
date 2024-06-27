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
    public IState _curState;
    [SyncVar(hook = nameof(OnStateChanged))] public string _curStateString;
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
            ChangeState(new IdleState(this, _attackController));
            _curStateString = _curState.ToString();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _rigidbody.AddForce(Vector3.one * 200f);
        }
        if (isLocalPlayer)
        {
            _attackController.UpdateCooldowns();
            _curState?.ExecuteOnUpdate();
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (_curState == null || _curState.IsTransitioning)
        {
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            _curState.OnInputCallback(new InputAction.CallbackContext());
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

        _curState?.ExecuteOnUpdate();
    }

    public void ChangeState(IState newState)
    {
        if (_curState != null)
        {
            _curState.Exit();
        }

        _curState = newState;

        if (_curState != null)
        {
            _curState.Enter();
            _curStateString = _curState.ToString();
            CmdUpdateState(_curStateString);
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
    void CmdUpdateState(string newState)
    {
        _curStateString = newState;
    }

    void OnStateChanged(string oldState, string newState)
    {
        if (this != null)
        {
            _attackController.HandleAttack(newState);
        }
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
            _curState?.OnInputCallback(context);
        }
    }

    public void OnHeavyAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (_attackController.currentHeavyAttackCoolTime <= 0 && context.performed)
        {
            _curState?.OnInputCallback(context);
        }
    }

    public void OnSkillAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            _curState?.OnInputCallback(context);
        }
    }
    #endregion

    public void Move()
    {
        if (PositionSet == false) return;
        if (IsHitted) return;
        if (!isLocalPlayer) return;

        if (_curState is FirstAttackState || _curState is SecondAttackState || _curState is FinishAttackState ||
            _curState is JumpHeavyAttackState || _curState is HeavyAttackState || _curState is SkillAttackState ||
            _curState is RollUpBackState || _curState is RollUpFrontState)
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

    public void StartAttackMove()
    {
        if (!isLocalPlayer) return;
        _attackController.StartAttackMove();
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
        PlayerGetKnockBack(knockBackPower, knockBackDirection, hitType);
    }

    private void PlayerGetDamaged(float damaged)
    {
        _playerHp -= damaged;
    }

    private void PlayerGetKnockBack(float knockBackPower, Vector3 knockBackDirection, HitType hitType)
    {
        _attackController.PlayerGetKnockBack(knockBackPower, knockBackDirection, hitType);
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
