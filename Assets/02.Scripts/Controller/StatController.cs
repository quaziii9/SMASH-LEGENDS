using Cysharp.Threading.Tasks;
using EnumTypes;
using EventLibrary;
using Mirror;
using System;
using System.Threading;
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
    [SerializeField] public float maxSkillGuage = 30;
    public float currentSkillGauge = 0;
    [SyncVar] public int AddSkillGuage = 0;
    [SyncVar] public bool CanSkillAttack = false;

    private PlayerController playerController;
    private EffectController effectController;
    private StateController stateController;
    private HookBulletController hookBulletController;
    public LegendUI legendUI;

    public bool PlayerDie;


    public float hookSkillTime;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        effectController = GetComponent<EffectController>();
        stateController = GetComponent<StateController>();

        if (TryGetComponent(out hookBulletController))
        {
            hookBulletController = GetComponent<HookBulletController>();
        }
        EventManager<GameEvents>.StartListening(GameEvents.StartSkillGaugeIncrease, () => StartSkillGaugeIncrease().Forget());
    }

    public void Start()
    {
        switch(playerController.legendType)
        {
            case PlayerController.LegendType.Peter:
                maxSkillGuage = 1600;
                maxHp = 4000;
                currentHp = maxHp;
                moveSpeed = 5.4f;
                jumpForce = 14.28f;
                defaultAttackDamage = 600;
                heavyAttackDamage = 900;
                skillAttackDamage = 1500;
                heavyAttackCoolTime = 4f;
                defaultKnockBackPower = 0.2f;
                heavyKnockBackPower = 0.38f;
                break;
            case PlayerController.LegendType.Hook:
                maxSkillGuage = 100;
                maxHp = 3300;
                currentHp = maxHp;
                moveSpeed = 5.6f;
                jumpForce = 14.7f;
                defaultAttackDamage = 700;
                heavyAttackDamage = 900;
                skillAttackDamage = 50;
                heavyAttackCoolTime = 5f;
                hookSkillTime = 9.3f;
                defaultKnockBackPower = 0.16f;
                heavyKnockBackPower = 0.38f;
                break;
        }
    }

    private void OnDestroy()
    {
        EventManager<GameEvents>.StopListening(GameEvents.StartSkillGaugeIncrease, () => StartSkillGaugeIncrease().Forget());
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        legendUI.SetHpBar(maxHp);
        legendUI.UpdateHPUI(currentHp, maxHp);

        OnHeavyAttackCoolTimeChanged(currentHeavyAttackCoolTime, currentHeavyAttackCoolTime);
        DuelUIController.Instance.UpdateSkillAttackIconeCoolGuage(currentSkillGauge, maxSkillGuage);
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
        DuelUIController.Instance.UpdateScore(isHost);

        if (GameManager.Instance.MatchOver == true) return;
        {
            playerController.ChangeState(PlayerState.Idle);

            playerController.ReviveLegend(isHost).Forget();
            DuelUIController.Instance.StartRespawnTimer(isHost);
        }
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
        if (isLocalPlayer)
        {
            float increaseRate = 2f; // 초당 증가율
            float updateInterval = .2f; // 1초 간격으로 업데이트

            while (currentSkillGauge < maxSkillGuage)
            {
                while (stateController.CurState == PlayerState.SkillAttack)
                {
                    // 상태가 SkillAttack인 동안 잠시 대기
                    await UniTask.Yield();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(updateInterval));
                currentSkillGauge += increaseRate;

                // currentSkillGauge가 maxSkillGuage를 초과하지 않도록 합니다.
                currentSkillGauge = Mathf.Min(currentSkillGauge, maxSkillGuage);

                if (isLocalPlayer)
                {
                    UpdateSkillGaugeUI();
                }
            }
            CanSkillAttack = true;
            //SkillAttackAllReady();
        }
        CmdUpdateSkillAttackAllReady(CanSkillAttack);
    }

    private void UpdateSkillGaugeUI()
    {
        DuelUIController.Instance.UpdateSkillAttackIconeCoolGuage(currentSkillGauge, maxSkillGuage);
    }


    public void SkillGaugeAdd(int addGauge)
    {
        if(isLocalPlayer)
        {
            currentSkillGauge += addGauge;
            if (currentSkillGauge >= maxSkillGuage)
            {
                currentSkillGauge = maxSkillGuage;
                CanSkillAttack = true;
                //SkillAttackAllReady();
            }
            DuelUIController.Instance.UpdateSkillAttackIconeCoolGuage(currentSkillGauge, maxSkillGuage);
        }
       CmdUpdateSkillAttackAllReady(CanSkillAttack);
    }

    public void SkillAttackAllReady()
    {
        //DuelUIController.Instance.SkillAttackKeyEnable(CanSkillAttack);
        
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


    public void StartSkill()
    {
        if (isLocalPlayer)
        {
            currentSkillGauge = 0;
            CanSkillAttack = false;
            DuelUIController.Instance.UpdateSkillAttackIconeCoolGuage(currentSkillGauge, maxSkillGuage);
            //SkillAttackAllReady();
        }
        CmdUpdateSkillAttackAllReady(CanSkillAttack);
    }


    private CancellationTokenSource cts; // 취소 토큰 소스

    public void StartHookSkillTime()
    {
        if (cts != null)
        {
            // 이미 실행 중인 작업이 있으면 취소
            cts.Cancel();
        }

        // 새로운 취소 토큰 소스를 생성
        cts = new CancellationTokenSource();
        HookSkillTime(cts.Token).Forget();
    }

    // HookSkillTime 비동기 메서드
    private async UniTask HookSkillTime(CancellationToken token)
    {
        int delayMilliseconds = Mathf.RoundToInt(hookSkillTime * 1000f); // 초 단위를 밀리초 단위로 변환

        try
        {
            await UniTask.Delay(delayMilliseconds, cancellationToken: token); // 변환된 밀리초 단위로 딜레이 
            hookBulletController.SetParrotOff();
        }
        catch (OperationCanceledException)
        {
            Debug.Log("HookSkillTime canceled");
        }
    }
}
