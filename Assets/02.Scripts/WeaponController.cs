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
            PlayerController otherplayer = other.GetComponent<PlayerController>();
            otherplayer.Hitted(attackController.DamageAmount, attackController.KnockBackPower, attackController.transform.position, attackController.KnockBackDireciton, attackController.hitType);

            weaponCollider.enabled = false;
        }
    }
}
