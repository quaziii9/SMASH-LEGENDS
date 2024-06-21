using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IState
{
    void Enter();
    void ExecuteOnUpdate();
    void Exit();
    void OnInputCallback(InputAction.CallbackContext context);
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

        Player.BindInputCallback(true, OnInputCallback);
    }

    public virtual void Exit()
    {
        if (Player == null)
        {
            Debug.LogError("Player is null in Exit!");
            return;
        }
        Player.BindInputCallback(false, OnInputCallback);
    }

    public virtual void ExecuteOnUpdate() { }
    public virtual void OnInputCallback(InputAction.CallbackContext context) { }
}

public class IdleState : StateBase
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        
        Player._animator.SetBool(nameof(Player.IsIdle), true);
    }
    
    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(nameof(Player.IsIdle), false);
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Jump" && context.performed)
        {
            Player.ChangeState(new JumpUpState(Player));
        }
    }
}
    
public class JumpUpState : StateBase
{
    public JumpUpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(nameof(Player.IsJumpingUp), true);
        Player.Jump();
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(nameof(Player.IsJumpingUp), false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._rigidBody.velocity.y < -1f)
        {
            Player.ChangeState(new JumpDownState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        Debug.Log(context.action.name);
        if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player.ChangeState(new AirAttackState(Player));
        }
    }
}
    
 public class JumpDownState : StateBase
 {
     public JumpDownState(PlayerController player) : base(player) { }
 
     public override void Enter()
     {
         base.Enter();
         Player._animator.SetBool(nameof(Player.IsJumpingDown), true);
     }
 
     public override void Exit()
     {
         base.Exit();
         Player._animator.SetBool(nameof(Player.IsJumpingDown), false);
     }
 
     public override void ExecuteOnUpdate()
     {
         if (Player._isGrounded)
         {
             Player.ChangeState(new JumpLandState(Player));
         }
     }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player.ChangeState(new AirAttackState(Player));
        }
    }
}
    
public class JumpLandState : StateBase
{

    public JumpLandState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(Player.IsLanding, true);
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(Player.IsLanding, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo stateInfo = Player._animator.GetCurrentAnimatorStateInfo(0);

        Debug.Log(stateInfo.normalizedTime);
        if (Player._isGrounded)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }
    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player.ChangeState(new AirAttackState(Player));
        }
    }
}

public class AirAttackState : StateBase
{
    public AirAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(nameof(Player.IsAirAttacking), true);
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(nameof(Player.IsAirAttacking), false);
    }

    public override void ExecuteOnUpdate()
    {
        // Attack animation finished
        AnimatorStateInfo stateInfo = Player._animator.GetCurrentAnimatorStateInfo(0);
        if (Player._isGrounded)
        {
            if (stateInfo.IsName("JumpAttack") && stateInfo.normalizedTime >= 0.3f)
            {
                Player.ChangeState(new JumpLandState(Player));
            }
            else
            {
                Player.ChangeState(new JumpLightLandingState(Player));
            }
        }
    }
}

public class JumpLightLandingState : StateBase
{
    public JumpLightLandingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(nameof(Player.IsLightLanding), true);
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(nameof(Player.IsLightLanding), false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo stateInfo = Player._animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("JumpLightLanding") && stateInfo.normalizedTime >= .5f)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }
}