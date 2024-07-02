using Cysharp.Threading.Tasks;
using Mirror;
using System;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Run,
    JumpUp,
    JumpDown,
    JumpLand,
    JumpAttack,
    JumpHeavyAttack,
    JumpHeavyAttackLanding,
    JumpAttackLanding,
    SkillAttack,
    FirstAttack,
    SecondAttack,
    FinishAttack,
    HeavyAttack,
    Hit,
    HitUp,
    HitDown,
    HitLand,
    DownIdle,
    RollUpFront,
    RollUpBack,
    StandUp,
    Hang,
    HangFall
}
public class StateController : NetworkBehaviour
{
    public IState CurrentStateInstance { get; private set; }
    private PlayerController _playerController;
    private AttackController _attackController;

    [SyncVar(hook = nameof(OnStateChanged))] public PlayerState _curState;
    [SyncVar(hook = nameof(OnInvincibleChanged))] public bool IsInvincible;
    [SyncVar] public bool PositionSet;
    public bool IsHitted;
 
    public void Initialize(PlayerController playerController, AttackController attackController)
    {
        _playerController = playerController;
        _attackController = attackController;
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentStateInstance?.Exit(); // 현재 상태 종료

        _curState = newState;
        CmdUpdateState(newState);

        CurrentStateInstance = CreateStateInstance(newState); // 새로운 상태 인스턴스 생성
        CurrentStateInstance?.Enter(); // 새로운 상태 진입

        if (newState == PlayerState.RollUpFront || newState == PlayerState.RollUpBack)
        {
            CmdUpdateInvincibleState(true);
        }
        else
        {
            CmdUpdateInvincibleState(false);
        }
    }

    [Command]
    public void CmdUpdateState(PlayerState newState)
    {
        _curState = newState;
    }

    [Command]
    public void CmdUpdateInvincibleState(bool invincible)
    {
        IsInvincible = invincible;
    }

    private IState CreateStateInstance(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                return new IdleState(_playerController, _attackController);
            case PlayerState.Run:
                return new RunState(_playerController);
            case PlayerState.JumpUp:
                return new JumpUpState(_playerController);
            case PlayerState.JumpDown:
                return new JumpDownState(_playerController);
            case PlayerState.JumpLand:
                return new JumpLandState(_playerController);
            case PlayerState.JumpAttack:
                return new JumpAttackState(_playerController);
            case PlayerState.JumpHeavyAttack:
                return new JumpHeavyAttackState(_playerController);
            case PlayerState.JumpHeavyAttackLanding:
                return new JumpHeavyAttackLandingState(_playerController);
            case PlayerState.JumpAttackLanding:
                return new JumpAttackLandingState(_playerController);
            case PlayerState.SkillAttack:
                return new SkillAttackState(_playerController);
            case PlayerState.FirstAttack:
                return new FirstAttackState(_playerController);
            case PlayerState.SecondAttack:
                return new SecondAttackState(_playerController);
            case PlayerState.FinishAttack:
                return new FinishAttackState(_playerController);
            case PlayerState.HeavyAttack:
                return new HeavyAttackState(_playerController);
            case PlayerState.Hit:
                return new HitState(_playerController);
            case PlayerState.HitUp:
                return new HitUpState(_playerController);
            case PlayerState.HitDown:
                return new HitDownState(_playerController);
            case PlayerState.HitLand:
                return new HitLandState(_playerController);
            case PlayerState.DownIdle:
                return new DownIdleState(_playerController);
            case PlayerState.RollUpFront:
                return new RollUpFrontState(_playerController);
            case PlayerState.RollUpBack:
                return new RollUpBackState(_playerController);
            case PlayerState.StandUp:
                return new StandUpState(_playerController);
            case PlayerState.Hang:
                return new HangState(_playerController);
            case PlayerState.HangFall:
                return new HangFallState(_playerController);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void OnStateChanged(PlayerState oldState, PlayerState newState)
    {
        _attackController.HandleAttack(newState); // 상태 변경 시 공격 값 설정
    }

    private void OnInvincibleChanged(bool oldState, bool newState)
    {
       
    }

    public void ExecuteOnUpdate()
    {
        CurrentStateInstance?.ExecuteOnUpdate();
    }

    //public void Start()
    //{
    //    PositionPlayersAsync().Forget();
    //}

    public void PositionPlayersAsync()
    {
        PositionSet = true;
    }
}
