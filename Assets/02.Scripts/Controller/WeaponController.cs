using UnityEngine;
using Mirror;

public class WeaponController : NetworkBehaviour
{
    AttackController attackController;
    StatController statController;
    Collider collider;

    private void Start()
    {
        attackController = GetComponentInParent<AttackController>();
        statController = GetComponentInParent<StatController>();
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StatController otherPlayerStat = other.GetComponent<StatController>();
            StateController otherPlayerState = other.GetComponent<StateController>();
            PlayerController otherPlayerController = other.GetComponent<PlayerController>();
            

            if (gameObject.name == "HeavyJumpAttackHitZone")
            {
                collider.enabled = false;
            }

            if (otherPlayerState.IsInvincible)
            {
                return;
            }

          

            bool isHost = otherPlayerController.IsHost;


            if (gameObject.CompareTag("SkillBullet"))
            {
                otherPlayerStat.Hitted(
                statController.skillAttackDamage,
                isHost);
            }
            else
            {
                otherPlayerStat.Hitted(
                attackController.DamageAmount,
                attackController.KnockBackPower,
                attackController.transform.position,
                attackController.KnockBackDireciton,
                attackController.hitType,
                attackController.PlusAddForce,
                isHost);
            }

            if (statController.currentSkillGauge < statController.maxSkillGuage)
            {
                statController.SkillGaugeAdd(statController.AddSkillGuage);
            }
            if (otherPlayerStat.currentSkillGauge < otherPlayerStat.maxSkillGuage)
            {
                otherPlayerStat.SkillGaugeAdd(statController.AddSkillGuage / 3 * 2);
            }


            if (gameObject.CompareTag("Bullet") || gameObject.CompareTag("SkillBullet"))
            {
                gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
