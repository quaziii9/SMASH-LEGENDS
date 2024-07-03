using Cysharp.Threading.Tasks;
using EnumTypes;
using EventLibrary;
using Mirror;
using System;
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

    [Header("SkillGage")]
    public float maxSkillGuage = 100;
    public float currentSkillGauge = 0;
    public int AddSkillGuage = 0;
    [SyncVar(hook = nameof(OnCanSkillAttackChanged))] public bool CanSkillAttack = false;

    private PlayerController playerController;
    private EffectController effectController;
    public LegendUI legendUI;

    public bool PlayerDie;

    private void Awake()
    {
        currentHp = maxHp;
        playerController = GetComponent<PlayerController>();
        effectController = GetComponent<EffectController>();
        EventManager<GameEvents>.StartListening(GameEvents.StartSkillGaugeIncrease, () => StartSkillGaugeIncrease().Forget());
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        legendUI.SetHpBar(maxHp);
        legendUI.UpdateHPUI(currentHp, maxHp);

        OnHeavyAttackCoolTimeChanged(currentHeavyAttackCoolTime, currentHeavyAttackCoolTime);
        DuelUIController.Instance.UpdateSkillAttackIconeCoolTime(currentSkillGauge, maxSkillGuage);
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
                DuelUIController.Instance.UpdateHeavyAttackIconeCoolTime(currentHeavyAttackCoolTime, heavyAttackCoolTime, true);
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
        DuelUIController.Instance.UpdateHeavyAttackIconeCoolTime(currentHeavyAttackCoolTime, heavyAttackCoolTime, false);
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

    public async UniTask StartSkillGaugeIncrease()
    {
        float increaseRate = 1.0f; // 초당 증가율
        float updateInterval = 0.02f; // 업데이트 주기 (50FPS 정도의 프레임 레이트)

        while (currentSkillGauge < maxSkillGuage)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(updateInterval));
            currentSkillGauge += increaseRate * updateInterval;

            // currentSkillGauge가 maxSkillGuage를 초과하지 않도록 합니다.
            currentSkillGauge = Mathf.Min(currentSkillGauge, maxSkillGuage);

            if (isLocalPlayer)
            {
                DuelUIController.Instance.UpdateSkillAttackIconeCoolTime(currentSkillGauge, maxSkillGuage);
            }
        }
        SkillAttackAllReady();
        CanSkillAttack = true;
        CmdUpdateSkillAttackAllReady(CanSkillAttack);
    }


    public void SkillGaugeAdd(int addGauge)
    {
        currentSkillGauge += addGauge;
        if (currentSkillGauge > maxSkillGuage)
        {
            SkillAttackAllReady();
            currentSkillGauge = maxSkillGuage;
            CanSkillAttack = true;
            CmdUpdateSkillAttackAllReady(CanSkillAttack);
        }
        DuelUIController.Instance.UpdateSkillAttackIconeCoolTime(currentSkillGauge, maxSkillGuage);
    }

    public void SkillAttackAllReady()
    {
        if(isLocalPlayer)
        {
            DuelUIController.Instance.SkillAttackKeyEnable();
        }
    }

    public void OnCanSkillAttackChanged(bool oldValue, bool newValue)
    {
        CmdUpdateSkillAttackAllReady(newValue);
    }

    [Command]
    public void CmdUpdateSkillAttackAllReady(bool newValue)
    {
        RpcUpdateSkillAttackAllReadyI(newValue);
    }

    [ClientRpc]
    private void RpcUpdateSkillAttackAllReadyI(bool newValue)
    {
        legendUI.LegendUIAllReadySkill(newValue);
    }
}
