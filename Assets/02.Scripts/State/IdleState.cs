using UnityEngine.InputSystem;
using UnityEngine;

public class IdleState : StateBase
{
    private float enterTime;

    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._rigidBody.velocity = new Vector3(0, Player._rigidBody.velocity.y, 0);
        Player._animator.SetBool(Player.IsIdle, true);
        Player.CanMove = true; // Idle 상태에서는 이동 가능하게 설정
        Player.CanLook = true; // Idle 상태에서는 Look 가능하게 설정
        enterTime = Time.time; // 현재 시간을 저장
    }

    public override void Exit()
    {
        base.Exit();
        Player._animator.SetBool(Player.IsIdle, false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._moveDirection != Vector3.zero)
        {
            Player.ChangeState(new RunState(Player));
        }
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
            Player.ChangeState(new FirstAttackState(Player));
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
    public override bool IsTransitioning => !Player._animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
}
