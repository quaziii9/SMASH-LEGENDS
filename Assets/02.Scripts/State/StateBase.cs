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

    protected StateBase(PlayerController player)
    {
        Player = player;
    }

    public virtual void Enter()
    {
        if (Player == null)
        {
            Debug.LogError("Player is null in Enter!");
            return;
        }
        Debug.Log($"Entering state: {Player._curState.GetType().Name}");

        Player.BindInputCallback(true, OnInputCallback);
    }

    public virtual void Exit()
    {
        if (Player == null)
        {
            Debug.LogError("Player is null in Exit!");
            return;
        }
        Debug.Log($"Exiting state: {Player._curState.GetType().Name}");
        Player.BindInputCallback(false, OnInputCallback);
    }

    public virtual void ExecuteOnUpdate() { }
    public virtual void OnInputCallback(InputAction.CallbackContext context) { }

    public abstract bool IsTransitioning { get; }

}