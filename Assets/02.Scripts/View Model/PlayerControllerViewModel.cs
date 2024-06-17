using ViewModel;

public class PlayerControllerViewModel : ViewModelBase
{
    private float _moveX;
    private float _moveY;
    private float _moveZ;

    public float MoveX
    {
        get { return _moveX; }
        set
        {
            if (_moveX == value)
                return;

            _moveX = value;
        }
    }

    public float MoveY 
    {
        get { return _moveY; }
        set
        {
            if (_moveY == value)
                return;

            _moveY = value;
        }
    }
    public float MoveZ
    {
        get { return _moveZ; }
        set
        {
            if (_moveZ == value)
                return;

            _moveZ = value;
            OnPropertyChanged(nameof(MoveZ));
        }
    }

    public float Speed { get; set; }
    public float JumpForce { get; set; }
    public bool IsGrounded { get; set; }

    public PlayerControllerViewModel(float speed, float jumpForce)
    {
        Speed = speed;
        JumpForce = jumpForce;
        IsGrounded = true;
    }
}



