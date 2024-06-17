using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
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
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _capsuleCollider.bounds.extents.y);
    }

}
