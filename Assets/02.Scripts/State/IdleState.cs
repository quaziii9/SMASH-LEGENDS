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
        Player.rigidbody.velocity = new Vector3(0, Player.rigidbody.velocity.y, 0);
        Player.AimationController.SetBool(Player.AimationController.IsIdle, true);
        Player.StateController.IsHitted = false;
        Player.CanMove = true;
        Player.CanLook = true;
        Player.isIdleJump = true;
        enterTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
        if (Player.AimationController != null)
        {
            Player.AimationController.SetBool(Player.AimationController.IsIdle, false);
        }
    }

    public override void ExecuteOnUpdate()
    {
        if (Player.StateController.PositionSet == false) return;
        if (Player.moveDirection != Vector3.zero)
        {            
            Player.ChangeState(PlayerState.Run);
        }

        //if (Time.time - enterTime > 0.5f)
        //{
        //    // 0.5초 후 상태 변경을 위한 입력 처리
        //    var keyboard = Keyboard.current;

        //    if (keyboard.zKey.wasPressedThisFrame)
        //    {                
        //        Player.ChangeState(PlayerState.FirstAttack);
        //        return;
        //    }
        //    else if (keyboard.xKey.wasPressedThisFrame && StatController.currentHeavyAttackCoolTime <=0)
        //    {               
        //        Player.ChangeState(PlayerState.HeavyAttack);
        //        return;
        //    }
        //}
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (Player.StateController.PositionSet == false) return;
        if (isTransitioning) return; // 상태 전환 중에는 입력 무시

        if (context.action.name == "Jump" && context.performed)
        {          
            Player.ChangeState(PlayerState.JumpUp);
        }
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero)
        {
            Player.ChangeState(PlayerState.Run);
        }
        else if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player.ChangeState(PlayerState.FirstAttack);
        }
        else if (context.action.name == "HeavyAttack" && context.performed && StatController.currentHeavyAttackCoolTime <= 0)
        {
            Player.ChangeState(PlayerState.HeavyAttack);
        }
        else if (context.action.name == "SkillAttack" && context.performed)
        {
            Player.ChangeState(PlayerState.SkillAttack);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("Idle");
}

