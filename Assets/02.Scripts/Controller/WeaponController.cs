using UnityEngine;
using Mirror;

public class WeaponController : NetworkBehaviour
{
    Collider weaponCollider;
    AttackController attackController;

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        attackController = GetComponentInParent<AttackController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StatController otherplayer = other.GetComponent<StatController>();
            StateController otherplayerState = other.GetComponent<StateController>();
            PlayerController otherPlayerController = other.GetComponent<PlayerController>();

            if (otherplayerState.IsInvincible == true) return;

            bool isHost = otherPlayerController.IsHost;

            otherplayer.Hitted(
                attackController.DamageAmount, 
                attackController.KnockBackPower, 
                attackController.transform.position, 
                attackController.KnockBackDireciton, 
                attackController.hitType, 
                attackController.PlusAddForce,
                isHost);

            weaponCollider.enabled = false;
        }
    }
}
