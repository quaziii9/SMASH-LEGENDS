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
    [SyncVar(hook = nameof(OnHpChanged))] public int currentHp;

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
    [SyncVar(hook = nameof(OnHeavyAttackCoolTimeChanged))] public float currentHeavyAttackCoolTime = 0f;

    private PlayerController playerController;
    private EffectController effectController;
    public LegendUI legendUI;

    public bool PlayerDie;

    private void Awake()
    {
        currentHp = maxHp;
        playerController = GetComponent<PlayerController>();
        effectController = GetComponent<EffectController>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        legendUI.SetHpBar(maxHp);
        legendUI.UpdateHPUI(currentHp, maxHp);

        //OnHpChanged(currentHp, currentHp);
        OnHeavyAttackCoolTimeChanged(currentHeavyAttackCoolTime, currentHeavyAttackCoolTime);
    }

    public void ApplyDamage(int damage, bool isHost)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            playerController.CanChange = false;
            effectController.SetDieSmokeEffect();
        }
        DuelUIController.Instance.UpdateHealthBar(currentHp, maxHp, isHost);
        CmdUpdateHPUI(currentHp, maxHp);
    }

    [Command]
    public void CmdUpdateHPUI(int currentHp, int maxHp)
    {
        RpcUpdateHPUI(currentHp, maxHp);
    }

    [ClientRpc]
    private void RpcUpdateHPUI(int currentHp, int maxHp)
    {
        legendUI.UpdateHPUI(currentHp, maxHp);
    }

    public async UniTaskVoid StartCooldownTimer()
    {
        while (currentHeavyAttackCoolTime > 0)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            currentHeavyAttackCoolTime -= Time.deltaTime;
            currentHeavyAttackCoolTime = Mathf.Clamp(currentHeavyAttackCoolTime, 0, heavyAttackCoolTime);
            CmdUpdateHeavyAttackCoolTimeUI(currentHeavyAttackCoolTime, heavyAttackCoolTime);
            if (isLocalPlayer)
            {
                DuelUIController.Instance.UpdateHeavyAttackIconeCoolTime(currentHeavyAttackCoolTime, heavyAttackCoolTime);
            }
        }
    }

    [Command]
    private void CmdUpdateHeavyAttackCoolTimeUI(float currentCoolTime, float maxCoolTime)
    {
        RpcUpdateHeavyAttackCoolTimeUI(currentCoolTime, maxCoolTime);
    }

    [ClientRpc]
    private void RpcUpdateHeavyAttackCoolTimeUI(float currentCoolTime, float maxCoolTime)
    {
        legendUI.UpdateHeavyAttackCoolTimeUI(currentCoolTime, maxCoolTime);
    }

    public void StartHeavyAttackCooldown()
    {
        currentHeavyAttackCoolTime = heavyAttackCoolTime;
        CmdUpdateHeavyAttackCoolTimeUI(currentHeavyAttackCoolTime, heavyAttackCoolTime);
        StartCooldownTimer().Forget();
    }

    private void OnHpChanged(int oldHp, int newHp)
    {
        CmdUpdateHPUI(newHp, maxHp);
    }


    private void OnHeavyAttackCoolTimeChanged(float oldCoolTime, float newCoolTime)
    {
        legendUI.UpdateHeavyAttackCoolTimeUI(newCoolTime, heavyAttackCoolTime);
    }

    [Command]
    public void CmdSmash(bool isHost)
    {
        RpcSmash(isHost);
    }

    [ClientRpc]
    private void RpcSmash(bool isHost)
    {
        Smash(isHost);
    }

    public void Smash(bool isHost)
    {
        gameObject.SetActive(false);
        playerController.ReviveLegend(isHost).Forget();
        DuelUIController.Instance.UpdateScore(isHost);
        DuelUIController.Instance.StartRespawnTimer(isHost);
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


    [Command]
    public void CmdUpdateHealthBar(int currentHp, int maxHp, bool isHost)
    {
        RpcUpdateHealthBar(currentHp, maxHp, isHost);
    }

    [ClientRpc]
    private void RpcUpdateHealthBar(int currentHp, int maxHp, bool isHost)
    {
        DuelUIController.Instance.UpdateHealthBar(currentHp, maxHp, isHost);
    }
}
