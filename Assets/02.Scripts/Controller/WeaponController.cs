using UnityEngine;
using Mirror;

public class WeaponController : NetworkBehaviour
{
    Collider weaponCollider;
    AttackController attackController;
    StatController statController;

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        attackController = GetComponentInParent<AttackController>();
        statController = GetComponentInParent<StatController>();
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

            statController.SkillGaugeAdd(statController.AddSkillGuage);

            otherplayer.SkillGaugeAdd(statController.AddSkillGuage / 3 * 2);


            weaponCollider.enabled = false;
        }
    }
}
