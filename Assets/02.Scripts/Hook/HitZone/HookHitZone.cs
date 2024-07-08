using Mirror;


public class HookHitZone : NetworkBehaviour
{
    protected AttackController AttackController;
    protected StatController StatController;
    protected PlayerController PlayerController;

    protected void Awake()
    {
        AttackController = transform.GetComponentInParent<AttackController>();
        StatController = transform.GetComponentInParent<StatController>();
        PlayerController = transform.GetComponentInParent<PlayerController>();
    }
}
