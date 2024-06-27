using System;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

public enum HitType
{
    Hit,
    HitUp,
}

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

    public Rigidbody _rigidbody;
    public AnimationController _animationController;

    [Header("Attack")]
    private float heavyAttackCoolTime = 4f;
    private float currentHeavyAttackCoolTime = 0f; // 현재 쿨타임
    [SerializeField] private float _defaultAttackDamage = 600;
    [SerializeField] private float _heavyAttackDamage = 900;
    [SerializeField] private float _skillAttackDamage = 1500;

    [Header("Kncokback")]
    private float _defaultAttackKnockBackPower = 0.2f;
    private float _heavyAttackKnockBackPower = 0.38f;

    [SyncVar] public float _playerHp = 10000;

    [SyncVar] public float DamageAmount;
    [SyncVar] public float KnockBackPower = 1;
    [SyncVar] public Vector3 KnockBackDireciton;
    [SyncVar] public HitType hitType;

    [Header("State")]
    public IState _curState;
    [SyncVar(hook = nameof(OnStateChanged))] public string _curStateString;
    public bool CanMove { get; set; }
    public bool CanLook { get; set; }
    public bool CanChange { get; set; }
    public bool IsMoveInputActive { get; private set; }
    public bool IsHitted;

    private bool isJumping = false;
    private bool isIdleJump = false;
    private float jumpMoveSpeed = 2.2f; // 점프 중 이동 속도

    [Header("Detection")]
    public float detectionRadius = 10f; // 탐지 반경
    public LayerMask playerLayer; // 플레이어가 속한 레이어

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animationController = GetComponent<AnimationController>();
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            ChangeState(new IdleState(this));
            _curStateString = _curState.ToString();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _rigidbody.AddForce(Vector3.one * 200f);
        }
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
        switch (_curStateString)
        {
            case nameof(FirstAttackState):
                DamageAmount = _defaultAttackDamage / 3;
                KnockBackPower = _defaultAttackKnockBackPower;
                KnockBackDireciton = transform.forward + transform.up * 0.5f;
                hitType = HitType.Hit;
                break;
            case nameof(SecondAttackState):
                DamageAmount = _defaultAttackDamage / 6;
                KnockBackPower = _defaultAttackKnockBackPower;
                KnockBackDireciton = transform.forward + transform.up * 0.5f;
                hitType = HitType.Hit;
                break;
            case nameof(FinishAttackState):
                DamageAmount = _defaultAttackDamage / 3;
                KnockBackPower = _heavyAttackKnockBackPower;
                KnockBackDireciton = transform.forward + transform.up * 1.2f;
                hitType = HitType.HitUp;
                break;
            case nameof(JumpAttackState):
                DamageAmount = _defaultAttackDamage * 0.6f;
                KnockBackPower = _heavyAttackKnockBackPower;
                KnockBackDireciton = transform.forward + transform.up * 1.2f;
                hitType = HitType.HitUp;
                break;
            case nameof(HeavyAttackState):
                DamageAmount = _heavyAttackDamage;
                KnockBackPower = _heavyAttackKnockBackPower;
                KnockBackDireciton = transform.forward + transform.up * 1.2f;
                hitType = HitType.HitUp;
                break;
            case nameof(JumpHeavyAttackState):
                DamageAmount = _heavyAttackDamage / 3 * 2;
                KnockBackPower = _heavyAttackKnockBackPower;
                KnockBackDireciton = transform.forward + transform.up * 1.2f;
                hitType = HitType.HitUp;
                break;
            case nameof(SkillAttackState):
                DamageAmount = (_skillAttackDamage - 500) / 5;
                KnockBackPower = _defaultAttackKnockBackPower;
                KnockBackDireciton = transform.forward + transform.up;
                hitType = HitType.Hit;
                break;
        }
    }

    public void SkillLastAttackDamage()
    {
        DamageAmount = _skillAttackDamage / 5 + 500;
        KnockBackPower = _heavyAttackKnockBackPower;
        KnockBackDireciton = transform.forward + transform.up * 1.2f;
        hitType = HitType.HitUp;
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

        // _curState?.OnInputCallback(context);
    }

    public void OnDefaultAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            Debug.Log("Default Attack Input Performed");
            RotateTowardsNearestPlayer();
            _curState?.OnInputCallback(context);
        }
    }

    public void OnHeavyAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (currentHeavyAttackCoolTime <= 0 && context.performed)
        {
            Debug.Log("Heavy Attack Input Performed");
            RotateTowardsNearestPlayer();
            _curState?.OnInputCallback(context);
        }
    }

    public void OnSkillAttackInput(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (context.performed)
        {
            Debug.Log("Skill Attack Input Performed");
            RotateTowardsNearestPlayer();
            _curState?.OnInputCallback(context);
        }
    }
    #endregion

    public void Move()
    {
        if (IsHitted) return;
        if (!isLocalPlayer) return;

        if (_curState is FirstAttackState || _curState is SecondAttackState || _curState is FinishAttackState ||
            _curState is JumpHeavyAttackState || _curState is HeavyAttackState || _curState is SkillAttackState ||
            _curState is RollUpBackState || _curState is RollUpFrontState)
        {
            float elapsedTime = Time.time - _attackMoveStartTime;
            float fraction = elapsedTime / _attackMoveDuration;
            float distanceToMove = Mathf.Lerp(0, _attackMoveDistance, fraction);

            Vector3 forwardMovement = _attackMoveDirection * (distanceToMove - _currentMoveDistance);
            _rigidbody.MovePosition(_rigidbody.position + forwardMovement);

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
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, jumpForce, _rigidbody.velocity.z);
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
            _rigidbody.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);

            if (_rigidbody.velocity.y < 0)
            {
                _rigidbody.velocity += Vector3.up * gravityScale * 0.5f * Time.fixedDeltaTime; // 감속 비율 적용
            }
        }

        // 최대 낙하속도를 초과하지 않도록 제한
        if (_rigidbody.velocity.y < -maxFallSpeed)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -maxFallSpeed, _rigidbody.velocity.z);
        }
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
        Vector3 origin = groundCheck.transform.position; // 플레이어의 위치에서 약간 위쪽
        _isGrounded = Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance);
        //_Ground = Physics.Raycast(origin, Vector3.down, out hit, groundDistance);
    }

    private void OnDrawGizmosSelected()
    {
        // 디버그를 위해 지면 체크 레이캐스트를 시각적으로 표시
        Gizmos.color = Color.red;
        Vector3 origin = groundCheck.transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void StartAttackMove()
    {
        if (!isLocalPlayer) return;
        if (_curState.ToString() == nameof(SkillAttackState))
        {
            _attackMoveDistance = 8f;
            _attackMoveDuration = 1.2f;
        }
        _attackMoveStartTime = Time.time;
        _currentMoveDistance = 0;
        if (_curState.ToString() == nameof(RollUpBackState)) _attackMoveDirection = -transform.forward;
        else
            _attackMoveDirection = transform.forward; // 현재 보고 있는 방향으로 이동    
    }

    public void StartHeavyAttackCooldown()
    {
        currentHeavyAttackCoolTime = heavyAttackCoolTime; // 스킬 사용 후 쿨타임 설정
    }


    [Command]
    public void CmdHitted(float damaged, float knockBackPower, Vector3 knockBackDireciton, HitType hitType)
    {
        RpcHitted(damaged, knockBackPower, knockBackDireciton, hitType);
    }

    [ClientRpc]
    public void RpcHitted(float damaged, float knockBackPower, Vector3 knockBackDireciton, HitType hitType)
    {
        PlayerGetDamaged(damaged);
        PlayerGetKnockBack(knockBackPower, knockBackDireciton, hitType);
    }

    private void PlayerGetDamaged(float damaged)
    {
        _playerHp -= damaged;
    }

    public void PlayerGetKnockBack(float knockBackPower, Vector3 knockBackDireciton, HitType hitType)
    {
        switch (hitType)
        {
            case HitType.Hit:
                ChangeState(new HitState(this));
                break;
            case HitType.HitUp:
                ChangeState(new HitUpState(this));
                break;
        }
        _rigidbody.velocity = Vector3.zero;
        Debug.Log(knockBackPower);
        Debug.Log(knockBackDireciton);
        _rigidbody.AddForce(knockBackDireciton * knockBackPower, ForceMode.Impulse);

    }

    public void Hitted(float damaged, float knockBackPower, Vector3 knockBackDireciton, HitType hitType)
    {
        CmdHitted(damaged, knockBackPower, knockBackDireciton, hitType);
    }

    private void RotateTowardsNearestPlayer()
    {
        Debug.Log("Rotating towards nearest player...");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hitColliders.Length > 0)
        {
            Transform nearestPlayer = null;
            float minDistance = Mathf.Infinity;

            foreach (Collider collider in hitColliders)
            {
                // 자신을 무시
                if (collider.gameObject == this.gameObject)
                    continue;

                float distance = Vector3.Distance(transform.position, collider.transform.position);
                Debug.Log($"Found player at distance: {distance}");
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPlayer = collider.transform;
                }
            }

            if (nearestPlayer != null)
            {
                Vector3 direction = (nearestPlayer.position - transform.position).normalized;
                if (direction != Vector3.zero) // 벡터가 (0,0,0)이 아닌지 확인
                {
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = lookRotation; // 즉시 회전
                    Debug.Log($"Rotating towards player at position: {nearestPlayer.position}");
                }
            }
        }
    }
}
