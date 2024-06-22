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
        if (context.action.name == "HeavyAttack" && context.performed)
        {
            Player.ChangeState(new JumpHeavyAttackState(Player));
        }
        else if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player.ChangeState(new JumpAttackState(Player));
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
        if (context.action.name == "HeavyAttack" && context.performed)
        {
            Player.ChangeState(new JumpHeavyAttackState(Player));
        }
        else if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player.ChangeState(new JumpAttackState(Player));
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
            Player.ChangeState(new JumpAttackState(Player));
        }
    }
}

public class JumpAttackState : StateBase
{
    public JumpAttackState(PlayerController player) : base(player) { }

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
        else if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player._rigidBody.velocity = new Vector3(0, Player._rigidBody.velocity.y, 0);
            Player.ChangeState(new AttackState(Player));
        }
    }
}

public class AttackState : StateBase
{
    private int comboCounter = 0;

    public AttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        PerformComboAttack();
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetTrigger("EndCombo");
        comboCounter = 0;
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed && comboCounter < 3)
        {
            comboCounter++;
            PerformComboAttack();
        }
        else if (context.action.name == "Move")
        {
            Vector2 input = context.ReadValue<Vector2>();
            if (input != Vector2.zero && Player._animator.GetBool("CanMove"))
            {
                Player.ChangeState(new RunState(Player));
            }
        }
    }

    private void PerformComboAttack()
    {
        Player._animator.SetInteger("ComboCounter", comboCounter);
        Player._animator.SetTrigger("IsAttack");
        Player.StartAttackMove(1.5f);
    }
}



public class HeavyAttackState : StateBase
{
    public HeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetTrigger("HeavyAttack");
        Player.StartAttackMove(1.5f);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._animator.GetBool("CanMove"))
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        // No specific input handling for HeavyAttackState
    }
}

public class JumpHeavyAttackState : StateBase
{
    public JumpHeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetTrigger("AirHeavyAttack");
        Player.StartAttackMove(2.5f);
        Player._rigidBody.velocity = new Vector3(Player._rigidBody.velocity.x, Player.jumpForce, Player._rigidBody.velocity.z);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._isGrounded)
        {
            Player.ChangeState(new JumpHeavyAttackLandingState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        // No specific input handling for JumpHeavyAttackState
    }
}

public class JumpHeavyAttackLandingState : StateBase
{
    public JumpHeavyAttackLandingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(nameof(Player.IsLanding), true);
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(nameof(Player.IsLanding), false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._isGrounded)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }
}
