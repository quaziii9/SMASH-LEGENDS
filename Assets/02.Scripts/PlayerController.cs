using Sirenix.OdinInspector.Editor;
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

    private int comboCounter = 0;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();      
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

    }

    private void UpdateAnimator()
    {     
        _animator.SetBool("IsGround", _isGrounded);
        //_animator.SetBool("IsJumping", _rigidBody.velocity.y >= 1f);
        _animator.SetBool("IsFalling",_rigidBody.velocity.y <= -1f);
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _moveDirection = context.performed ? new Vector3(input.x, 0, input.y) : Vector3.zero;
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        _animator.SetTrigger("IsJump");
        if (context.performed && _isGrounded)
        {
            Jump();
        }
    }

    public void OnDefaultAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_canCombo)
            {
                _canCombo = false; // 콤보 상태를 비활성화
                comboCounter = (comboCounter + 1) % 3; // 콤보 카운터 증가, 최대 2까지 증가 후 0으로 돌아감
                _animator.SetInteger("ComboCounter", comboCounter);
                _animator.SetTrigger("IsAttack");
            }
            else if (!_isAttack)
            {
                _isAttack = true;
                comboCounter = 0; // 첫 번째 공격
                _animator.SetInteger("ComboCounter", comboCounter);
                _animator.SetTrigger("IsAttack");
            }
        }
    }

    private void Move()
    {
        if (_isAttack)
        {
            // 공격 중일 때 이동하지 않도록 속도를 0으로 설정
            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
            return;
        }
        _animator.SetBool("IsMoving", _moveDirection != Vector3.zero && _isGrounded);
        Vector3 velocity = new Vector3(_moveDirection.x * moveSpeed, _rigidBody.velocity.y, _moveDirection.z * moveSpeed);
        _rigidBody.velocity = velocity;

        LookAt();
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

    // 애니메이션 이벤트를 통해 공격이 끝났을 때 호출될 메서드
    public void OnComboAttackEnd()
    {
        _canCombo = false;
        _isAttack = false;
        comboCounter = 0; // 콤보 카운터 초기화
        _animator.SetInteger("ComboCounter", comboCounter);
        _animator.ResetTrigger("IsAttack");
    }

    // 애니메이션 이벤트를 통해 콤보 가능 상태로 만들기
    public void EnableCombo()
    {
        _canCombo = true;
    }
}
