using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

public class StatController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.4f;
    public float jumpMoveSpeed = 2.2f;

    [Header("Health")]
    public int maxHp = 9000;
    [SyncVar] public int currentHp;

    [Header("Jumping")]
    public float jumpForce = 14.28f;
    public float gravityScale = 36f;
    public float maxFallSpeed = 20f;

    [Header("Attacks")]
    public int defaultAttackDamage = 600;
    public int heavyAttackDamage = 900;
    public int skillAttackDamage = 1500;

    [Header("Knockback")]
    public float defaultKnockBackPower = 0.2f;
    public float heavyKnockBackPower = 0.38f;

    [Header("Cooldowns")]
    public float heavyAttackCoolTime = 4f;

    private PlayerController playerController;
    private EffectController effectController;

    public bool PlayerDie;


    private void Awake()
    {
        currentHp = maxHp;
        playerController = GetComponent<PlayerController>(); // PlayerController 인스턴스 설정
        effectController = GetComponent<EffectController>(); // PlayerController 인스턴스 설정


    }

    public void ApplyDamage(int damage, bool isHost)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            playerController.CanChange = false;
            effectController.SetDieSmokeEffect();

        }
        DuelManager.Instance.UpdateHealthBar(currentHp, maxHp, isHost);
    }
    [Command]
    public void CmdSmash(bool isHost)
    {
        Debug.Log($"CmdSmash called on server for player: {netId}, isHost: {isHost}");
        RpcSmash(isHost);
    }

    [ClientRpc]
    private void RpcSmash(bool isHost)
    {
        Debug.Log($"RpcSmash called on client for player: {netId}, isHost: {isHost}");
        Smash(isHost);
    }

    public void Smash(bool isHost)
    {
        Debug.Log($"Smash called for player: {netId}, isHost: {isHost}");
        gameObject.SetActive(false);
        playerController.ReviveLegend(isHost).Forget();
        DuelManager.Instance.UpdateScore(isHost);
        DuelManager.Instance.StartRespawnTimer(isHost);
    }

    [Command]
    public void CmdHitted(int damaged, float knockBackPower, Vector3 knockBackDirection, HitType hitType, bool plusAddForce, bool isHost)
    {
        RpcHitted(damaged, knockBackPower, knockBackDirection, hitType, plusAddForce, isHost);
    }

    [ClientRpc]
    public void RpcHitted(int damaged, float knockBackPower, Vector3 knockBackDirection, HitType hitType, bool plusAddForce, bool isHost)
    {
        ApplyDamage(damaged, isHost);
        playerController.AttackController.PlayerGetKnockBack(knockBackPower, knockBackDirection, hitType, plusAddForce);
    }

    public void Hitted(int damaged, float knockBackPower, Vector3 attackerPosition, Vector3 attackerDirection, HitType hitType, bool plusAddForce, bool isHost)
    {
        Vector3 direction = (transform.position - attackerPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(-direction);
        transform.rotation = lookRotation;

        Vector3 knockBackDirection = -transform.forward;
        knockBackDirection.y = attackerDirection.y;
        knockBackDirection.x = knockBackDirection.x >= 0 ? 1 : -1;

        CmdHitted(damaged, knockBackPower, knockBackDirection, hitType, plusAddForce, isHost);
    }
}