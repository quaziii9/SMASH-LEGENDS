using UnityEngine;
using Mirror;

public enum HitType
{
    Hit,
    HitUp,
}

public class AttackController : NetworkBehaviour
{
    private PlayerController player;

    private float heavyAttackCoolTime = 4f;
    public float currentHeavyAttackCoolTime = 0f; // 현재 쿨타임
    [SerializeField] private float _defaultAttackDamage = 600;
    [SerializeField] private float _heavyAttackDamage = 900;
    [SerializeField] private float _skillAttackDamage = 1500;

    [Header("Knockback")]
    private float _defaultAttackKnockBackPower = 0.2f;
    private float _heavyAttackKnockBackPower = 0.38f;

    [SyncVar] public float DamageAmount;
    [SyncVar] public float KnockBackPower = 1;
    [SyncVar] public Vector3 KnockBackDireciton;
    [SyncVar] public HitType hitType;

    public void Initialize(PlayerController playerController)
    {
        player = playerController;
    }

    public void HandleAttack(string attackType)
    {
        switch (attackType)
        {
            case nameof(FirstAttackState):
                SetAttackValues(_defaultAttackDamage / 3, _defaultAttackKnockBackPower, player.transform.up * 0.5f, HitType.Hit);
                break;
            case nameof(SecondAttackState):
                SetAttackValues(_defaultAttackDamage / 6, _defaultAttackKnockBackPower, player.transform.up * 0.5f, HitType.Hit);
                break;
            case nameof(FinishAttackState):
                SetAttackValues(_defaultAttackDamage / 3, _heavyAttackKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case nameof(JumpAttackState):
                SetAttackValues(_defaultAttackDamage * 0.6f, _heavyAttackKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case nameof(HeavyAttackState):
                SetAttackValues(_heavyAttackDamage, _heavyAttackKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case nameof(JumpHeavyAttackLandingState):
            case nameof(JumpHeavyAttackState):
                SetAttackValues(_heavyAttackDamage / 3 * 2, _heavyAttackKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case nameof(SkillAttackState):
                SetAttackValues((_skillAttackDamage - 500) / 5, _defaultAttackKnockBackPower, player.transform.up, HitType.Hit);
                break;
        }
    }


    private void SetAttackValues(float damage, float knockBackPower, Vector3 knockBackDirection, HitType hitType)
    {
        DamageAmount = damage;
        KnockBackPower = knockBackPower;
        KnockBackDireciton = knockBackDirection;
        this.hitType = hitType;
    }

    public void SkillLastAttackDamage()
    {
        SetAttackValues(_skillAttackDamage / 5 + 500, _heavyAttackKnockBackPower, player.transform.forward + player.transform.up * 1.2f, HitType.HitUp);
    }

    public void UpdateCooldowns()
    {
        if (currentHeavyAttackCoolTime > 0)
        {
            currentHeavyAttackCoolTime -= Time.deltaTime;
        }
    }

    public void StartHeavyAttackCooldown()
    {
        currentHeavyAttackCoolTime = heavyAttackCoolTime; // 스킬 사용 후 쿨타임 설정
    }
}
