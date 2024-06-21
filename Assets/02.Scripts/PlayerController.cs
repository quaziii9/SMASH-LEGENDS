using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5.4f; // 피터의 이동속도
    private float jumpForce = 14.28f; // 피터의 점프가속도
    private float gravityScale = 36f; // 피터의 중력가속도
    private float maxFallSpeed = 20f; // 피터의 최대 낙하속도
    public GameObject groundCheck;

    private float groundCheckDistance = 1f; // 지면 체크를 위한 거리

    private Rigidbody _rigidBody;
    private Animator _animator;
    private Vector3 _moveDirection;
    private bool _isGrounded;
    private bool _isAttack;
    private bool _canCombo;
    private bool _canChangeAnimation;
    private bool _isJumping;
    private bool _hasAirAttacked;
    private bool _isLanding;
    private bool _isCanMove;

    private float _attackMoveDistance; // 공격 중 이동할 거리
    private Vector3 _attackMoveDirection; // 공격 중 이동 방향
    private float _attackMoveDuration = 0.3f; // 공격 중 이동하는 데 걸리는 시간
    private float _attackMoveStartTime; // 공격 중 이동 시작 시간
    private float _currentMoveDistance; // 현재까지 이동한 거리

    [Header("Attack")]
    private int comboCounter = 0;
    private float skillCoolTime = 4f;

    private IState _curState;

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
        Move();
        ApplyCustomGravity();
        CheckGroundStatus();
        UpdateAnimator();
    }

    private void Update()
    {
        if (skillCoolTime > 0)
        {
            skillCoolTime -= Time.deltaTime;
        }
    }

    public void ChangeState(IState newState)
    {
        _curState?.Exit();
        _curState = newState;
        _curState.Enter();
    }

    private void UpdateAnimator()
    {
        _animator.SetBool("AirAttacking", _hasAirAttacked);
        _animator.SetBool("IsGround", _isGrounded);
        _animator.SetBool("IsJumping", _isJumping);
        _animator.SetBool("IsFalling", _rigidBody.velocity.y < -1f);
        _animator.SetBool("CanChangeAnimation", _canChangeAnimation);
    }

    #region InputSystem
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _moveDirection = context.performed ? new Vector3(input.x, 0, input.y) : Vector3.zero;
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && _isGrounded && _isAttack == false && _isLanding == false)
        {
            _animator.SetTrigger("IsJump");
            Jump();
            _isJumping = true;
        }
    }

    public void OnDefaultAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isGrounded)
            {
                _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
                if (_canCombo == true)
                {
                    if (comboCounter > 2)
                    {
                        return;
                    }
                    _animator.SetInteger("ComboCounter", comboCounter);
                    _animator.SetTrigger("IsAttack");
                    StartAttackMove(1.5f);
                    comboCounter++;
                    _canCombo = false; // 콤보 상태를 비활성화
                }
                else
                {              
                    comboCounter = 0; // 첫 번째 공격                   
                    _animator.SetInteger("ComboCounter", comboCounter);
                    _animator.SetTrigger("IsAttack");
                    StartAttackMove(1.5f);
                    comboCounter++;
                }
            }
            else if (_isJumping == true && _hasAirAttacked == false) // 점프 중 공격
            {
                _hasAirAttacked = true;
                _animator.SetTrigger("AirAttack");
            }
        }
    }

    public void OnHeavyAttackInput(InputAction.CallbackContext context)
    {
        if(context.performed && skillCoolTime <0 && _isGrounded == true)
        {
            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
            _isAttack = true;
            _animator.SetTrigger("HeavyAttack");
            StartAttackMove(1.5f);
            skillCoolTime = 4f;
        }
        if (context.performed && skillCoolTime < 0 && _isJumping == true)
        {
            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
            _isAttack = true;
            _animator.SetTrigger("AirHeavyAttack");
            StartAttackMove(2.5f);
            _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, jumpForce, _rigidBody.velocity.z);
            skillCoolTime = 4f;
        }
    }

    public void OnSkillAttackInput(InputAction.CallbackContext context)
    {
        _isAttack = true;
        _animator.SetTrigger("SkillAttack");
    }
    #endregion

    private void Move()
    {
        if (_isLanding)
        {
            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
            return;
        }
        if( _isAttack)
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
        LookAt();
        _animator.SetBool("IsMoving", _moveDirection != Vector3.zero && _isGrounded);
        Vector3 velocity = new Vector3(_moveDirection.x * moveSpeed, _rigidBody.velocity.y, _moveDirection.z * moveSpeed);
        _rigidBody.velocity = velocity;
        
    }

    private void Jump()
    {
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, jumpForce, _rigidBody.velocity.z);
    }

    protected void LookAt()
    {
        if (_moveDirection != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(_moveDirection);
            _rigidBody.rotation = targetAngle;
        }
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


    public void IdleAnimationEvent()
    {
        _isLanding = false;
        _canCombo = false;
        _animator.ResetTrigger("IsAttack");
        _isAttack = false;
        _isJumping = false;
        _hasAirAttacked = false;
        _animator.ResetTrigger("AirAttack");
        _animator.ResetTrigger("HeavyAttack");
        _animator.ResetTrigger("SkillAttack");
        _animator.ResetTrigger("AirHeavyAttack");
    }

    public void CanChangeAnimationEvent()
    {
        _canChangeAnimation = true; 
    }

    // 애니메이션 이벤트를 통해 콤보 가능 상태로 만들기
    public void EnableComboAnimationEvent()
    {
        _canChangeAnimation = false;
        _canCombo = true;
        //_isAttack = true;
    }

    // 점프 애니메이션 이벤트
    public void JumpLandAnimationEvent()
    {
        _hasAirAttacked = false;
    }

    public void LookFalseAnimationEvent()
    {
        _isLanding = true;
    }

    private void StartAttackMove(float distance)
    {
        _isAttack = true;
        _attackMoveDistance = distance;
        _attackMoveStartTime = Time.time;
        _currentMoveDistance = 0;
        _attackMoveDirection = transform.forward; // 현재 보고 있는 방향으로 이동
    }
}
