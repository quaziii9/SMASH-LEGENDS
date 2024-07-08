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

        // null 검사를 추가합니다.
        if (attackController == null)
        {
            Debug.LogError("AttackController를 찾을 수 없습니다.");
        }
        if (statController == null)
        {
            Debug.LogError("StatController를 찾을 수 없습니다.");
        }
        if (collider == null)
        {
            Debug.LogError("Collider를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StatController otherPlayerStat = other.GetComponent<StatController>();
            StateController otherPlayerState = other.GetComponent<StateController>();
            PlayerController otherPlayerController = other.GetComponent<PlayerController>();

            // otherPlayerStat, otherPlayerState, otherPlayerController가 null인지 확인합니다.
            

            if (gameObject.name == "HeavyJumpAttackHitZone")
            {
                collider.enabled = false;
            }

            if (otherPlayerState.IsInvincible)
            {
                return;
            }

            bool isHost = otherPlayerController.IsHost;

            otherPlayerStat.Hitted(
                attackController.DamageAmount,
                attackController.KnockBackPower,
                attackController.transform.position,
                attackController.KnockBackDireciton,
                attackController.hitType,
                attackController.PlusAddForce,
                isHost);

            if (statController.currentSkillGauge < statController.maxSkillGuage)
            {
                statController.SkillGaugeAdd(statController.AddSkillGuage);
            }
            if (otherPlayerStat.currentSkillGauge < otherPlayerStat.maxSkillGuage)
            {
                otherPlayerStat.SkillGaugeAdd(statController.AddSkillGuage / 3 * 2);
            }


            if (gameObject.CompareTag("Bullet"))
            {
                gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
