using UnityEngine.InputSystem;
using UnityEngine;


public class JumpUpState : StateBase
{
    public JumpUpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(nameof(Player.IsJumpingUp), true);
        Player.Jump(Player._moveDirection == Vector3.zero); // idle에서 점프인지 아닌지 확인
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

        // 점프 중 이동 처리
        if (Player.IsMoveInputActive)
        {
            Player.Move();
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

        // 점프 중 이동 처리
        if (Player.IsMoveInputActive)
        {
            Player.Move();
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
        Player.Land();
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(Player.IsLanding, false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._isGrounded)
        {
            if (Player.IsMoveInputActive)
            {
                Player.ChangeState(new RunState(Player));
            }
            else
            {
                Player.ChangeState(new IdleState(Player));
            }
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
        Player.CanMove = false;
        Player.CanLook = false;
        Player.Land();
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(nameof(Player.IsLightLanding), false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo stateInfo = Player._animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("JumpLightLanding") && stateInfo.normalizedTime >= 0.8f)
        {
            if (Player.IsMoveInputActive)
            {
                Player.ChangeState(new RunState(Player));
            }
            else
            {
                Player.ChangeState(new IdleState(Player));
            }
        }
    }
}



public class RunState : StateBase
{
    public RunState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(Player.IsRunning, true);
        Player.CanMove = true;
        Player.CanLook = true;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(Player.IsRunning, false);
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
            Player.ChangeState(new ComboAttack1State(Player));
        }
        else if (context.action.name == "HeavyAttack" && context.performed)
        {
            Player._rigidBody.velocity = new Vector3(0, Player._rigidBody.velocity.y, 0);
            Player.ChangeState(new HeavyAttackState(Player));
        }
        else if (context.action.name == "SkillAttack" && context.performed)
        {
            Player.ChangeState(new SkillAttackState(Player));
        }
    }
}


public class ComboAttack1State : StateBase
{
    public ComboAttack1State(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(Player.IsComboAttack1, true);
        Player.StartAttackMove(1.0f);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void ExecuteOnUpdate()
    {
        if (!Player._animator.GetBool(Player.IsComboAttack1))
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed && Player.CanChange)
        {
            Player.ChangeState(new ComboAttack2State(Player));
        }
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanChange)
        {
            Player.ChangeState(new RunState(Player));
        }
    }
}

public class ComboAttack2State : StateBase
{
    public ComboAttack2State(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(Player.IsComboAttack2, true);
        Player.StartAttackMove(1.0f);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void ExecuteOnUpdate()
    {
        if (!Player._animator.GetBool(Player.IsComboAttack2))
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed && Player.CanChange)
        {
            Player.ChangeState(new ComboAttack3State(Player));
        }
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanChange)
        {
            Player.ChangeState(new RunState(Player));
        }
    }
}


public class ComboAttack3State : StateBase
{
    public ComboAttack3State(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(Player.IsComboAttack3, true);
        Player.StartAttackMove(1.0f);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void ExecuteOnUpdate()
    {
        if (!Player._animator.GetBool(Player.IsComboAttack3))
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanChange)
        {
            Player.ChangeState(new RunState(Player));
        }
    }
}

public class HeavyAttackState : StateBase
{
    public HeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(Player.IsHeavyAttacking, true);
        Player.StartAttackMove(1.5f);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void ExecuteOnUpdate()
    {
        // Attack animation has ended
        if (!Player._animator.GetBool(Player.IsHeavyAttacking))
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanChange)
        {
            Player.ChangeState(new RunState(Player));
        }
    }
}

public class JumpHeavyAttackState : StateBase
{
    public JumpHeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetTrigger("AirHeavyAttack");
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

public class SkillAttackState : StateBase
{
    public SkillAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(Player.IsSkillAttack, true);
        Player.StartAttackMove(1.5f);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void ExecuteOnUpdate()
    {
        if (!Player._animator.GetBool(Player.IsSkillAttack))
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanChange)
        {
            Player.ChangeState(new RunState(Player));
        }
    }
}
