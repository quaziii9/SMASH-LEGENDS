using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using ViewModel.Extensions;


public class PlayerControllerView : MonoBehaviour
{
    private PlayerControllerViewModel _vm;
    private Rigidbody rb;

    public float speedMultiplier = 5.0f;
    public float jumpForceMultiplier = 5.0f;

    private void Awake()
    {
        _vm = new PlayerControllerViewModel(speedMultiplier, jumpForceMultiplier);
        _vm.PropertyChanged += OnPropChanged_Player;
        rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        _vm.PropertyChanged -= OnPropChanged_Player;
    }


    private void OnPropChanged_Player(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.MoveZ):
                if (_vm.MoveZ > 0)
                {
                    rb.AddForce(Vector3.up * _vm.MoveZ, ForceMode.Impulse);
                    _vm.MoveZ = 0; // 힘을 적용한 후 초기화
                }
                break;
        }
    }

    private void Update()
    {
        Vector3 movement = new Vector3(_vm.MoveX, 0, _vm.MoveY) * _vm.Speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    public void OnMove(InputValue value)
    {
        _vm.RequestMove(value.Get<Vector2>().x, value.Get<Vector2>().y);
    }

    public void OnJump()
    {
        _vm.RequestJump();       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            _vm.IsGrounded = true;
        }
    }
}
