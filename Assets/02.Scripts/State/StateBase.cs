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
    protected bool isTransitioning = true; // 상태 전환 중임을 나타내는 플래그

    protected StateBase(PlayerController player)
    {
        Player = player;
    }

    public virtual void Enter()
    {
        if (Player == null)
        {
            return;
        }

        Player.BindInputCallback(true, OnInputCallback);
        isTransitioning = true;
        Player.StartCoroutine(TransitionDelay()); // 상태 전환 딜레이 시작
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

    private System.Collections.IEnumerator TransitionDelay()
    {
        yield return new WaitForSeconds(0.1f); // 상태 전환 후 0.1초 딜레이
        isTransitioning = false;
    }
}
