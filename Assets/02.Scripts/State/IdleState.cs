using UnityEngine.InputSystem;
using UnityEngine;

public class IdleState : StateBase
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animator.SetBool(Player.IsIdle, true);
        Player.CanMove = true;
        Player.CanLook = true;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(Player.IsIdle, false);
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Jump" && context.performed)
        {
            Player.ChangeState(new JumpUpState(Player));
        }
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero)
        {
            Player.ChangeState(new RunState(Player));
        }
        else if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player.ChangeState(new ComboAttack1State(Player));
        }
        else if (context.action.name == "HeavyAttack" && context.performed)
        {
            Player.ChangeState(new HeavyAttackState(Player));
        }
        else if (context.action.name == "SkillAttack" && context.performed)
        {
            Player.ChangeState(new SkillAttackState(Player));
        }
    }
}
