using UnityEngine.InputSystem;
using UnityEngine;

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
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero)
        {
            Player.ChangeState(new RunState(Player));
        }
    }
}
