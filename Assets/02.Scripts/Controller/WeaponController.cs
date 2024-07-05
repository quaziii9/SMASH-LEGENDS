using UnityEngine;
using Mirror;

public class WeaponController : NetworkBehaviour
{
    AttackController attackController;
    StatController statController;
    Collider Collider;

    private void Start()
    {
        attackController = GetComponentInParent<AttackController>();
        statController = GetComponentInParent<StatController>();
        Collider = GetComponent<Collider>();
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

            if(statController.currentSkillGauge < statController.maxSkillGuage)
                statController.SkillGaugeAdd(statController.AddSkillGuage);
            if (otherplayer.currentSkillGauge < otherplayer.maxSkillGuage)
                otherplayer.SkillGaugeAdd(statController.AddSkillGuage / 3 * 2);

            if (gameObject.name == "HeavyJumpAttackHitZone")
            {
                Collider.enabled = false;
            }
        }
    }
}
