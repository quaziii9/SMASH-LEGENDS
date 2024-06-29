using System;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public StatController StatController { get; set; } 
    public AnimationController AimationController { get; set; }
    public AttackController AttackController { get; set; }
    public StateController StateController { get; set; }
    public EffectController EffectController { get; set; }

    public Vector3 moveDirection;
    public Rigidbody rigidbody;

    [Header("GroundCheck")]
    public GameObject groundCheck;
    private float _groundCheckDistance = 1f; // 지면 체크를 위한 거리
    public bool IsGrounded;
    public bool Ground;

    public bool CanMove { get; set; }
    public bool CanLook { get; set; }
    public bool CanChange { get; set; }

    public bool IsMoveInputActive { get; private set; }

    private bool isJumping = false;
    public bool isIdleJump = false;

    private Vector3 _rollUpMoveDirection;
    private float _rollUpMoveDistance = 2.5f;
    private float _rollUpMoveDuration = 0.3f;
    private float _rollUpMoveStartTime;
    private float _currentMoveDistance;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        AimationController = GetComponent<AnimationController>();
        EffectController = GetComponent<EffectController>();
        AttackController = GetComponent<AttackController>();
        StateController = GetComponent<StateController>();
        StatController = GetComponent<StatController>();

        if (AttackController != null)
        {
            AttackController.Initialize(this, StatController);
        }
        else
        {
            Debug.LogError("AttackController is not attached to the PlayerController gameObject");
        }

        if (StateController != null)
        {
            StateController.Initialize(this, AttackController);
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
            StateController.ChangeState(PlayerState.Idle);
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
            AttackController.UpdateCooldowns();
            StateController.ExecuteOnUpdate();
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (StateController.CurrentStateInstance == null || StateController.CurrentStateInstance.IsTransitioning)
        {
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            StateController.CurrentStateInstance.OnInputCallback(new InputAction.CallbackContext());
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

        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        StateController.ExecuteOnUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        StateController.ChangeState(newState);
    }

    private void OnStateChanged(PlayerState oldState, PlayerState newState)
    {
        AttackController.HandleAttack(newState); // 상태 변경 시 공격 값 설정
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Ground = true;
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
        moveDirection = context.performed ? new Vector3(input.x, 0, input.y) : Vector3.zero;
        IsMoveInputActive = context.performed;
    }

    public void OnDefaultAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            StateController.CurrentStateInstance?.OnInputCallback(context);
        }
    }

    public void OnHeavyAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (AttackController.currentHeavyAttackCoolTime <= 0 && context.performed)
        {
            StateController.CurrentStateInstance?.OnInputCallback(context);
        }
    }

    public void OnSkillAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            StateController.CurrentStateInstance?.OnInputCallback(context);
        }
    }
    #endregion

    public void Move()
    {
        if (StateController.PositionSet == false || StateController.IsHitted==true || !isLocalPlayer) return;

        if (StateController.CurrentStateInstance is RollUpBackState || StateController.CurrentStateInstance is RollUpFrontState)
        {
            HandleRollUpMOVE();
        }

        if (StateController.CurrentStateInstance is FirstAttackState || StateController.CurrentStateInstance is SecondAttackState || StateController.CurrentStateInstance is FinishAttackState ||
            StateController.CurrentStateInstance is JumpHeavyAttackState || StateController.CurrentStateInstance is HeavyAttackState || StateController.CurrentStateInstance is SkillAttackState)

        {
            AttackController.HandleAttackMove();
            return;
        }

        if (CanLook && moveDirection != Vector3.zero)
        {
            LookAt();
        }

        if (CanMove)
        {
            float currentMoveSpeed = isJumping && isIdleJump ? StatController.jumpMoveSpeed : StatController.moveSpeed;
            Vector3 velocity = new Vector3(moveDirection.x * currentMoveSpeed, rigidbody.velocity.y, moveDirection.z * currentMoveSpeed);
            rigidbody.velocity = velocity;
        }
    }

    public void LookAt()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(moveDirection);
            rigidbody.rotation = targetAngle;
        }
    }

    public void Jump()
    {
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, StatController.jumpForce, rigidbody.velocity.z);
        isJumping = true;
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
        rigidbody.MovePosition(rigidbody.position + forwardMovement);

        _currentMoveDistance = distanceToMove;

        if (CanLook && moveDirection != Vector3.zero)
        {
            LookAt();
        }
    }

    public void StartRollUpMove()
    {
        if (!isLocalPlayer) return;
        _rollUpMoveStartTime = Time.time;
        _currentMoveDistance = 0;
        if (StateController._curState == PlayerState.RollUpBack) _rollUpMoveDirection = -transform.forward;
        else
            _rollUpMoveDirection = transform.forward;
    }

    private void ApplyCustomGravity()
    {
        if (!IsGrounded)
        {
            rigidbody.AddForce(Vector3.down * StatController.gravityScale, ForceMode.Acceleration);

            if (rigidbody.velocity.y < 0)
            {
                rigidbody.velocity += Vector3.up * StatController.gravityScale * 0.5f * Time.fixedDeltaTime;
            }
        }

        if (rigidbody.velocity.y < -StatController.maxFallSpeed)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, -StatController.maxFallSpeed, rigidbody.velocity.z);
        }
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
        Vector3 origin = groundCheck.transform.position;
        IsGrounded = Physics.Raycast(origin, Vector3.down, out hit, _groundCheckDistance);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null || AttackController == null)
        {
            return; // groundCheck나 _attackController가 null이면 아무 것도 하지 않음
        }

        Gizmos.color = Color.red;
        Vector3 origin = groundCheck.transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.down * (_groundCheckDistance));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AttackController.detectionRadius);
    }
}
