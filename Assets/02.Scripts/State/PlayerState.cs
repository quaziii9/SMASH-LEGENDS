using UnityEngine.InputSystem;
using UnityEngine;


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
            Player.ChangeState(new JumpAttack(Player));
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
            Player.ChangeState(new JumpAttack(Player));
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
            Player.ChangeState(new JumpAttack(Player));
        }
    }
}

public class JumpAttack : StateBase
{
    public JumpAttack(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(nameof(Player.IsJumpAttacking), true);
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(nameof(Player.IsJumpAttacking), false);
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


public class RunState : StateBase
{
    public RunState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool("IsRunning", true);
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool("IsRunning", false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._moveDirection == Vector3.zero)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Jump" && context.performed)
        {
            Player.ChangeState(new JumpUpState(Player));
        }
    }
}
