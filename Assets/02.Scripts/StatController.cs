using Mirror;
using UnityEngine;

public class StatController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.4f;
    public float jumpMoveSpeed = 2.2f;

    [Header("Health")]
    public float maxHp = 10000f;
    [SyncVar] public float currentHp;

    [Header("Jumping")]
    public float jumpForce = 14.28f;
    public float gravityScale = 36f;
    public float maxFallSpeed = 20f;

    [Header("Attacks")]
    public float defaultAttackDamage = 600f;
    public float heavyAttackDamage = 900f;
    public float skillAttackDamage = 1500f;

    [Header("Knockback")]
    public float defaultKnockBackPower = 0.2f;
    public float heavyKnockBackPower = 0.38f;

    [Header("Cooldowns")]
    public float heavyAttackCoolTime = 4f;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public float CurrentHp
    {
        get => currentHp;
        set => currentHp = Mathf.Clamp(value, 0, maxHp);
    }

    public void ApplyDamage(float damage)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            // Handle player death
            Debug.Log("Player is dead");
        }
    }


}
