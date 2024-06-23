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
        Player.CanMove = true;
        Player.CanLook = true;
        enterTime = Time.time;
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

        if (Time.time - enterTime > 0.5f)
        {
            // 0.5초 후 상태 변경을 위한 입력 처리
            var keyboard = Keyboard.current;

            if (keyboard.zKey.wasPressedThisFrame)
            {
                Player.ChangeState(new FirstAttackState(Player));
                return;
            }
            else if (keyboard.xKey.wasPressedThisFrame)
            {
                Player.ChangeState(new HeavyAttackState(Player));
                return;
            }
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (isTransitioning) return; // 상태 전환 중에는 입력 무시

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
