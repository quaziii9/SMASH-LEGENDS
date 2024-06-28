using Mirror;
using UnityEngine;

public class StatController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.4f;
    public float jumpMoveSpeed = 2.2f;

    [Header("Health")]
    public float maxHp = 10000f;
    [SyncVar(hook = nameof(OnHpChanged))] public float currentHp;

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

    private PlayerController playerController;

    private void Awake()
    {
        currentHp = maxHp;
        playerController = GetComponent<PlayerController>(); // PlayerController 인스턴스 설정
    }

    private void OnHpChanged(float oldHp, float newHp)
    {
        // 현재 체력이 변경될 때 처리할 로직이 있으면 여기에 추가
        Debug.Log($"HP changed from {oldHp} to {newHp}");
    }

    public void ApplyDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            // Handle player death
            Debug.Log("Player is dead");
        }
    }

    [Command]
    public void CmdHitted(float damaged, float knockBackPower, Vector3 knockBackDirection, HitType hitType)
    {
        ApplyDamage(damaged);
        RpcHitted(damaged, knockBackPower, knockBackDirection, hitType);
    }

    [ClientRpc]
    public void RpcHitted(float damaged, float knockBackPower, Vector3 knockBackDirection, HitType hitType)
    {
        if (!isServer) // 서버에서는 이미 데미지를 처리했으므로 클라이언트에서만 처리
        {
            ApplyDamage(damaged);
        }
        playerController.AttackController.PlayerGetKnockBack(knockBackPower, knockBackDirection, hitType);
    }

    public void Hitted(float damaged, float knockBackPower, Vector3 attackerPosition, Vector3 attackerDirection, HitType hitType)
    {
        Vector3 direction = (transform.position - attackerPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(-direction);
        transform.rotation = lookRotation;

        Vector3 knockBackDirection = -transform.forward;
        knockBackDirection.y = attackerDirection.y;
        knockBackDirection.x = knockBackDirection.x >= 0 ? 1 : -1;

        CmdHitted(damaged, knockBackPower, knockBackDirection, hitType);
    }
}
