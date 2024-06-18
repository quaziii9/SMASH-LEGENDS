using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5.4f; // ������ �̵��ӵ�
    private float jumpForce = 14.28f; // ������ �������ӵ�
    private float gravityScale = 36f; // ������ �߷°��ӵ�
    private float maxFallSpeed = 20f; // ������ �ִ� ���ϼӵ�
    [SerializeField] public GameObject groundCheck;

    private float groundCheckDistance = 1.5f; // ���� üũ�� ���� �Ÿ�

    private Rigidbody _rigidBody;
    private Animator _animator;
    private Vector3 _moveDirection;
    private bool _isGrounded;

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

    private void UpdateAnimator()
    {     
        _animator.SetBool("IsGround", _isGrounded);
        _animator.SetBool("IsFalling",_rigidBody.velocity.y <= -1f);
        _animator.SetBool("IsMoving", _moveDirection != Vector3.zero && _isGrounded);
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _moveDirection = context.performed ? new Vector3(input.x, 0, input.y) : Vector3.zero;
    }


    public void OnJumpInput(InputAction.CallbackContext context)
    {
        _animator.SetTrigger("Jump");
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

    private void ApplyCustomGravity()
    {
        if (!_isGrounded)
        {
            _rigidBody.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);

            if (_rigidBody.velocity.y < 0)
            {
                _rigidBody.velocity += Vector3.up * gravityScale * 0.5f * Time.fixedDeltaTime; // ���� ���� ����
            }
        }

        // �ִ� ���ϼӵ��� �ʰ����� �ʵ��� ����
        if (_rigidBody.velocity.y < -maxFallSpeed)
        {
            _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, -maxFallSpeed, _rigidBody.velocity.z);
        }
    }


    private void CheckGroundStatus()
    {
        RaycastHit hit;
        Vector3 origin = groundCheck.transform.position; // �÷��̾��� ��ġ���� �ణ ����
        _isGrounded = Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance);
    }

    private void OnDrawGizmosSelected()
    {
        // ����׸� ���� ���� üũ ����ĳ��Ʈ�� �ð������� ǥ��
        Gizmos.color = Color.red;
        Vector3 origin = groundCheck.transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance));
    }
}
