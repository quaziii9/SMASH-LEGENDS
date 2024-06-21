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
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void ExecuteOnUpdate() { }
    public virtual void OnInputCallback(InputAction.CallbackContext context) { }

}

public class IdleState : StateBase
{
    private readonly PlayerController _player;

    public IdleState(PlayerController player)
    {
        _player = player;
        _player.BindInputCallback(isBind: true, OnInputCallback);
    }

    public override void Exit()
    {
        base.Exit(); // 오버라이딩 하는 경우 이렇게 base. 문법을 적어줘야하는 경우도 있음을 명심할것
        _player.BindInputCallback(isBind: false, OnInputCallback);
    }

    public override void ExecuteOnUpdate()
    {

    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Jump")
        {
            _player._animator.SetTrigger("IsJump");
            _player.Jump();
            _player.ChangeState(new JumpState(_player));
            Debug.Log("짬프 Input 들어옴");
        }
    }
}

public class JumpState : StateBase
{
    private readonly PlayerController _player;
    public JumpState(PlayerController player)
    {
        _player = player;
         _player.BindInputCallback(isBind:true, OnInputCallback);
    }

    public override void Exit() 
    {
        _player.BindInputCallback(isBind: false, OnInputCallback);
    }

    public override void ExecuteOnUpdate()
    {
        //땅에 닿았는지 판단 후
         if (_player._isGrounded)
        {
            _player.ChangeState(new IdleState(_player));
        }
    }
}
