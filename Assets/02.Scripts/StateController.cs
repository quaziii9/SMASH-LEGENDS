using System;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public IState CurrentStateInstance { get; private set; }
    private PlayerController _playerController;
    private AttackController _attackController;

    public void Initialize(PlayerController playerController, AttackController attackController)
    {
        _playerController = playerController;
        _attackController = attackController;
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentStateInstance?.Exit(); // 현재 상태 종료

        _playerController._curState = newState;
        _playerController.CmdUpdateState(newState);

        CurrentStateInstance = CreateStateInstance(newState); // 새로운 상태 인스턴스 생성
        CurrentStateInstance?.Enter(); // 새로운 상태 진입
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

    public void ExecuteOnUpdate()
    {
        CurrentStateInstance?.ExecuteOnUpdate();
    }
}
