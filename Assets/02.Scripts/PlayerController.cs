using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
 

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.2f; // 지면 체크를 위한 거리

    private Rigidbody _rigidBody;
    private Animator _animator;
    private Vector3 _moveDirection;
    public bool _isGrounded;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        Move();
        CheckGroundStatus();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _moveDirection = context.performed ? new Vector3(input.x, 0, input.y) : Vector3.zero;
    }


    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && _isGrounded)
        {
            Jump();
        }
    }

    private void Move()
    {
        Vector3 velocity = new Vector3(_moveDirection.x * moveSpeed, _rigidBody.velocity.y, _moveDirection.z * moveSpeed);
        _rigidBody.velocity = velocity;

        //UpdateAnimator(_moveDirection.magnitude);
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



    private void CheckGroundStatus()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f; // 플레이어의 위치에서 약간 위쪽
        _isGrounded = Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance + 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        // 디버그를 위해 지면 체크 레이캐스트를 시각적으로 표시
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance + 0.1f));
    }
}
