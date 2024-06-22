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
    protected bool isTransitioning = true; // ���� ��ȯ ������ ��Ÿ���� �÷���

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
        Debug.Log($"Entering state: {this.GetType().Name}");

        Player.BindInputCallback(true, OnInputCallback);
        isTransitioning = true;
        Player.StartCoroutine(TransitionDelay()); // ���� ��ȯ ������ ����
    }

    public virtual void Exit()
    {
        if (Player == null)
        {
            Debug.LogError("Player is null in Exit!");
            return;
        }
        Debug.Log($"Exiting state: {this.GetType().Name}");
        Player.BindInputCallback(false, OnInputCallback);
    }

    public virtual void ExecuteOnUpdate() { }

    public virtual void OnInputCallback(InputAction.CallbackContext context)
    {
        if (isTransitioning) return; // ���� ��ȯ �߿��� �Է� ����
    }

    public abstract bool IsTransitioning { get; }

    private System.Collections.IEnumerator TransitionDelay()
    {
        yield return new WaitForSeconds(0.1f); // ���� ��ȯ �� 0.1�� ������
        isTransitioning = false;
    }
}
