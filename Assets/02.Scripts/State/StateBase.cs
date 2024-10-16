using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IState
{
    void Enter();
    void ExecuteOnUpdate();
    void Exit();
    void OnInputCallback(InputAction.CallbackContext context);
    bool IsTransitioning { get; }
}

public abstract class StateBase : IState
{
    protected PlayerController Player { get; private set; }
    protected AttackController AttackController { get; private set; }
    protected StatController StatController { get; private set; }
    protected bool isTransitioning = true; // 상태 전환 중임을 나타내는 플래그

    protected StateBase(PlayerController player, AttackController attackController)
    {
        Player = player;
        AttackController = attackController;
    }

    protected StateBase(PlayerController player)
    {
        Player = player;
        AttackController = player.AttackController;
        StatController = player.StatController;
    }

    public virtual void Enter()
    {
        if (Player == null)
        {
            return;
        }
        Player.BindInputCallback(true, OnInputCallback);
        isTransitioning = true;
        TransitionDelay().Forget();
    }

    public virtual void Exit()
    {
        if (Player == null)
        {
            return;
        }
        Player.BindInputCallback(false, OnInputCallback);
    }

    public virtual void ExecuteOnUpdate() { }

    public virtual void OnInputCallback(InputAction.CallbackContext context)
    {
        if (isTransitioning) return; // 상태 전환 중에는 입력 무시
    }

    public abstract bool IsTransitioning { get; }

    private async UniTask TransitionDelay()
    {
        await UniTask.Delay(100); // 상태 전환 후 0.1초 딜레이
        isTransitioning = false;
    }
}
