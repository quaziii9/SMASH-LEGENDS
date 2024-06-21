using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IState
{
    void Enter();
    void ExecuteOnUpdate();
    void Exit();
    void OnInputCallback(InputAction.CallbackContext context);
}

public class StateBase : IState
{
    protected PlayerController Player { get; private set; }

    protected StateBase(PlayerController player)
    {
        Player = player;
    }

    public virtual void Enter()
    {
        Player.BindInputCallback(true, OnInputCallback);
    }
    public virtual void Exit()
    {
        Player.BindInputCallback(false, OnInputCallback);
    }
    public virtual void ExecuteOnUpdate() { }
    public virtual void OnInputCallback(InputAction.CallbackContext context) { }

}

public class IdleState : StateBase
{
    public IdleState(PlayerController player) : base(player) { }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Jump")
        {
            Player.ChangeState(new JumpState(Player));
        }
    }
}

public class JumpState : StateBase
{
    public JumpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetTrigger("IsJump");
        Player.Jump();
    }

    public override void ExecuteOnUpdate()
    {
        // 땅에 닿았는지 판단 후
        if (Player._isGrounded)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }
}
