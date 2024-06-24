using System;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using EventLibrary;
using EnumTypes;

public class PlayerController : NetworkBehaviour
{
    private float moveSpeed = 5.4f; // 피터의 이동속도
    public float jumpForce = 14.28f; // 피터의 점프가속도
    private float gravityScale = 36f; // 피터의 중력가속도
    private float maxFallSpeed = 20f; // 피터의 최대 낙하속도

    private Vector3 _attackMoveDirection; // 공격 중 이동 방향
    public float _attackMoveDistance; // 공격 중 이동할 거리
    public float _attackMoveDuration; // 공격 중 이동하는 데 걸리는 시간
    public float _attackMoveStartTime; // 공격 중 이동 시작 시간
    public float _currentMoveDistance; // 현재까지 이동한 거리

    public Vector3 _moveDirection;

    [Header("GroundCheck")]
    public GameObject groundCheck;
    private float groundCheckDistance = 1f; // 지면 체크를 위한 거리
    private float groundDistance = .4f; // 지면 체크를 위한 거리
    public bool _isGrounded;
    public bool _Ground;

    public Rigidbody _rigidBody;
    public AnimationController _animationController;

    [Header("Attack")]
    private float heavyAttackCoolTime = 4f;
    private float currentHeavyAttackCoolTime = 0f; // 현재 쿨타임
    public float DamageAmount { get; set; }
    public float KnockBackPower { get; set; }

    [Header("State")]
    public IState _curState;
    public bool CanMove { get; set; }
    public bool CanLook { get; set; }
    public bool CanChange { get; set; }
    public bool IsMoveInputActive { get; private set; }

    private bool isJumping = false;
    private bool isIdleJump = false;
    private float jumpMoveSpeed = 2.2f; // 점프 중 이동 속도

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animationController = GetComponent<AnimationController>();
        EventManager<IngameEvents>.StartListening(IngameEvents.Hitted, Hitted);
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            ChangeState(new IdleState(this));
            currentHeavyAttackCoolTime = 0f;
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
            if (currentHeavyAttackCoolTime > 0)
            {
                currentHeavyAttackCoolTime -= Time.deltaTime;
            }
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

        _curState?.OnInputCallback(context);
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

        if (currentHeavyAttackCoolTime <= 0 && context.performed)
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
        if (_curState is FirstAttackState || _curState is SecondAttackState || _curState is FinishAttackState ||
            _curState is JumpHeavyAttackState || _curState is HeavyAttackState || _curState is SkillAttackState)
        {
            float elapsedTime = Time.time - _attackMoveStartTime;
            float fraction = elapsedTime / _attackMoveDuration;
            float distanceToMove = Mathf.Lerp(0, _attackMoveDistance, fraction);

            Vector3 forwardMovement = _attackMoveDirection * (distanceToMove - _currentMoveDistance);
            _rigidBody.MovePosition(_rigidBody.position + forwardMovement);

            _currentMoveDistance = distanceToMove;

            if (CanLook && _moveDirection != Vector3.zero)
            {
                LookAt();
            }

            return;
        }

        if (CanLook && _moveDirection != Vector3.zero)
        {
            LookAt();
        }

        if (CanMove)
        {
            float currentMoveSpeed = isJumping && isIdleJump ? jumpMoveSpeed : moveSpeed;
            Vector3 velocity = new Vector3(_moveDirection.x * currentMoveSpeed, _rigidBody.velocity.y, _moveDirection.z * currentMoveSpeed);
            _rigidBody.velocity = velocity;
        }
    }

    public void LookAt()
    {
        if (_moveDirection != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(_moveDirection);
            _rigidBody.rotation = targetAngle;
        }
    }

    public void Jump(bool idleJump = false)
    {
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, jumpForce, _rigidBody.velocity.z);
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
        _Ground = Physics.Raycast(origin, Vector3.down, out hit, groundDistance);
    }

    private void OnDrawGizmosSelected()
    {
        // 디버그를 위해 지면 체크 레이캐스트를 시각적으로 표시
        Gizmos.color = Color.red;
        Vector3 origin = groundCheck.transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundDistance));
    }

    public void StartAttackMove()
    {    
        if(_curState.ToString() == "SkillAttackState")
        {
            _attackMoveDistance = 8f;
            _attackMoveDuration = 1.5f;
        }
        _attackMoveStartTime = Time.time;
        _currentMoveDistance = 0;
        _attackMoveDirection = transform.forward; // 현재 보고 있는 방향으로 이동    
    }

    public void StartHeavyAttackCooldown()
    {
        currentHeavyAttackCoolTime = heavyAttackCoolTime; // 스킬 사용 후 쿨타임 설정
    }


    public void Hitted()
    {
        PlayerGetDamaged();
        PlayerGetKnockBack();
    }

    public void PlayerGetDamaged()
    {
        Debug.Log(DamageAmount);
    }

    public void PlayerGetKnockBack()
    {
        Debug.Log(KnockBackPower);
    }

}
