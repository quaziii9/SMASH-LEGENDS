using System;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Mathematics;

public class PlayerController : NetworkBehaviour
{
    public enum LegendType { Peter, Hook }

    [SyncVar] public LegendType legendType;

    public StatController StatController { get; set; }
    public AnimationController AimationController { get; set; }
    public AttackController AttackController { get; set; }
    public StateController StateController { get; set; }
    public EffectController EffectController { get; set; }

    public Vector3 moveDirection;
    public Rigidbody rigidbody;
    public Collider _collider;

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

    private CancellationTokenSource _taskCancel;

    private Vector3 startPosition1 = new Vector3(-20, 1.5f, 0); // 원하는 위치로 설정
    private Vector3 startPosition2 = new Vector3(20, 1.5f, 0); // 원하는 위치로 설정
    private Quaternion startRotation1 = Quaternion.Euler(new Vector3(0, 90, 0));
    private Quaternion startRotation2 = Quaternion.Euler(new Vector3(0, -90, 0));

    public int CanDefaultFlash = 0;

    [SyncVar] public bool IsHost;


    public bool HookJumpHeavyAttackMove;

    private void OnEnable()
    {
        if (EffectController != null)
        {
            EffectController.DisableDieSmokeEffect();
        }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
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
        if (StatController == null)
        {
            Debug.LogError("StatController component is missing on this GameObject.");
        }


        if (gameObject.name.Contains("Peter"))
        {
            legendType = LegendType.Peter;
        }
        else if (gameObject.name.Contains("Hook"))
        {
            legendType = LegendType.Hook;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkConnectionToClient conn = connectionToClient;
        IsHost = conn != null && conn == NetworkServer.localConnection;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkConnectionToClient conn = connectionToClient;
        IsHost = conn != null && conn == NetworkServer.localConnection;
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            StateController.ChangeState(PlayerState.Idle);
        }
        CanChange = true;

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

        //if (keyboard.spaceKey.wasPressedThisFrame)
        //{
        //    StateController.CurrentStateInstance.OnInputCallback(new InputAction.CallbackContext());
        //}

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

        moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

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

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("HangZone"))
        {
            if (StateController.CurState == PlayerState.HangFall) return;
            rigidbody.velocity = Vector3.zero;
            transform.forward = GetHangForward(other.transform.position);
            transform.position = GetHangPosition(other.transform.position);
            ChangeState(PlayerState.Hang);
        }
    }

    private Vector3 GetHangForward(Vector3 other)
    {
        Vector3 otherPosition = other.normalized;
        otherPosition.x = Mathf.Round(other.x);
        otherPosition.y = 0;
        otherPosition.z = Mathf.Round(other.z);

        return otherPosition * -1;
    }

    private Vector3 GetHangPosition(Vector3 hangPosition)
    {
        float hangPositionY = 1f;

        if (hangPosition.x != 0)
        {
            hangPosition = new Vector3(GetHangCorrectionPosition(hangPosition.x), hangPositionY, transform.position.z);
        }

        else if (hangPosition.z != 0)
        {
            hangPosition = new Vector3(transform.position.x, hangPositionY, GetHangCorrectionPosition(hangPosition.z));
        }

        return hangPosition;

        float GetHangCorrectionPosition(float value)
        {
            if (value > 0)
            {
                value -= 0.5f;
            }

            if (value < 0)
            {
                value += 0.5f;
            }

            return value;
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

        if (CanChange && CanDefaultFlash == 0)
        {
            if (StateController.CurState == PlayerState.Idle) return;
            DefualtAttackIconEnableFlash().Forget();
        }
    }

    public void OnHeavyAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (StatController.currentHeavyAttackCoolTime <= 0 && context.performed)
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
        if (StateController.PositionSet == false || StateController.IsHitted == true ||
            !isLocalPlayer || GameManager.Instance.MatchOver == true) return;



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

        if(StateController.CurrentStateInstance is HookSecondAttackState || StateController.CurrentStateInstance is HookHeavyAttackState || 
            StateController.CurrentStateInstance is HookFirstJumpAttackState || StateController.CurrentStateInstance is HookSecondJumpAttackState || 
            StateController.CurrentStateInstance is HookFinishJumpAttackState || StateController.CurrentStateInstance is HookJumpHeavyAttackState)
        {
            AttackController.HandleHookAttackMove();
            return;
        }

        if (CanLook && moveDirection != Vector3.zero)
        {
            LookAt();
        }

        if (CanMove)
        {
            float currentMoveSpeed = isJumping && isIdleJump ? StatController.jumpMoveSpeed : StatController.moveSpeed;
            Vector3 normalizedMoveDirection = moveDirection.normalized; // 정규화된 이동 방향
            Vector3 velocity = new Vector3(normalizedMoveDirection.x * currentMoveSpeed, rigidbody.velocity.y, normalizedMoveDirection.z * currentMoveSpeed);
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
        if (StateController.CurState == PlayerState.RollUpBack) _rollUpMoveDirection = -transform.forward;
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


    public async UniTaskVoid FallAsync()
    {
        float _fallingWaitTime = 3f;
        _taskCancel = new();

        await UniTask.Delay(TimeSpan.FromSeconds(_fallingWaitTime), cancellationToken: _taskCancel.Token);
        _collider.enabled = false;
        ChangeState(PlayerState.HangFall);

       
    }

    public async UniTaskVoid HangFall()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        // 콜라이더 다시 활성화
        _collider.enabled = true;
    }

    public void EscapeInHang()
    {
        _taskCancel.Cancel();
    }

    public async UniTask ReviveLegend(bool isHost)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(5));

        rigidbody.velocity = Vector3.zero;
        SetPosition(isHost);
        gameObject.SetActive(true);
        ChangeState(PlayerState.Idle);
        StatController.currentHp = StatController.maxHp;
        DuelUIController.Instance.UpdateHealthBar(StatController.currentHp, StatController.maxHp, isHost);
        StatController.CmdUpdateHPUI(StatController.currentHp, StatController.maxHp);

        CanChange = true;
        StateController.IsHitted = false;
        EffectController.StartInvincibleFlashEffect(5);
    }


    public void SetPosition(bool isHost)
    {
        if (isHost)
        {
            gameObject.transform.position = startPosition1;
            gameObject.transform.rotation = startRotation1;
        }
        else
        {
            gameObject.transform.position = startPosition2;
            gameObject.transform.rotation = startRotation2;
        }
    }


    public async UniTask DefualtAttackIconEnableFlash()
    {
        if(isLocalPlayer)
        {
            CanDefaultFlash++;
            DuelUIController.Instance.DefualtAttackIconEnable();
            await UniTask.Delay(100);
            DuelUIController.Instance.DefualtAttackIconDisable();
            await UniTask.Delay(100);
            if(StateController.CurState !=PlayerState.FinishAttack)
                DuelUIController.Instance.DefualtAttackIconEnable();
        }
    }

    public void DefualtAttackIconEnable()
    {
        if(isLocalPlayer)
            DuelUIController.Instance.DefualtAttackIconEnable();
    }
}
