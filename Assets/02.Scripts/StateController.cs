using Mirror;
using System;
using UnityEngine;

public class StateController : NetworkBehaviour
{
    public IState CurrentStateInstance { get; private set; }
    private PlayerController _playerController;
    private AttackController _attackController;

    [SyncVar(hook = nameof(OnStateChanged))] public PlayerState _curState;
    [SyncVar] public bool IsHitted;
    [SyncVar] public bool PositionSet;

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
    }

    [Command]
    public void CmdUpdateState(PlayerState newState)
    {
        _curState = newState;
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void OnStateChanged(PlayerState oldState, PlayerState newState)
    {
        _attackController.HandleAttack(newState); // 상태 변경 시 공격 값 설정
    }

    public void ExecuteOnUpdate()
    {
        CurrentStateInstance?.ExecuteOnUpdate();
    }
}
