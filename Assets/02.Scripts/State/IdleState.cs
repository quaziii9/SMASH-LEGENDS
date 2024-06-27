using UnityEngine.InputSystem;
using UnityEngine;

public class IdleState : StateBase
{
    private float enterTime;

    public IdleState(PlayerController player) : base(player) { }
    public IdleState(PlayerController player, AttackController attackController) : base(player, attackController) { }

    public override void Enter()
    {
        base.Enter();
        Player._rigidbody.velocity = new Vector3(0, Player._rigidbody.velocity.y, 0);
        Player._animationController.SetBool(Player._animationController.IsIdle, true);
        Player.IsHitted = false;
        Player.CanMove = true;
        Player.CanLook = true;
        enterTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsIdle, false);
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

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("Idle");
}

