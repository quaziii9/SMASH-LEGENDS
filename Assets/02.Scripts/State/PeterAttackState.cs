using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstAttackState : StateBase
{
    public FirstAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = 1.5f;
        AttackController.attackMoveDuration = 0.3f;
        Player.AimationController.SetBool(Player.AimationController.IsComboAttack1, true);
        AttackController.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsComboAttack1, false);
    }

    public override void ExecuteOnUpdate()
    {
        var animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        // FirstAttack 애니메이션이 끝났는지 확인
        if (animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(PlayerState.Idle);
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed && Player.CanChange)
        {
            Player.ChangeState(PlayerState.SecondAttack);
        }
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("FirstAttack");
}

public class SecondAttackState : StateBase
{
    public SecondAttackState(PlayerController player) : base(player) { }


    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = 1.5f;
        AttackController.attackMoveDuration = 0.3f;
        Player.AimationController.SetBool(Player.AimationController.IsComboAttack2, true);
        AttackController.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsComboAttack2, false);
    }

    public override void ExecuteOnUpdate()
    {
        var animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        // SecondAttack 애니메이션이 끝났는지 확인
        if (animatorStateInfo.IsName("SecondAttack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(PlayerState.Idle);
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed && Player.CanChange)
        {
            Player.ChangeState(PlayerState.FinishAttack);
        }
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("SecondAttack");
}

public class FinishAttackState : StateBase
{
    public FinishAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = 1.5f;
        AttackController.attackMoveDuration = 0.3f;
        Player.AimationController.SetBool(Player.AimationController.IsComboAttack3, true);
        AttackController.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsComboAttack3, false);
    }

    public override void ExecuteOnUpdate()
    {
        var animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        // FinishAttack 애니메이션이 끝났는지 확인
        if (animatorStateInfo.IsName("FinishAttack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(PlayerState.Idle);
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("FinishAttack");
}

public class HeavyAttackState : StateBase
{
    public HeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = 1.5f;
        AttackController.attackMoveDuration = 0.3f;
        Player.AimationController.SetBool(Player.AimationController.IsHeavyAttacking, true);
        Player.StatController.StartHeavyAttackCooldown();
        Player.StatController.StartCooldownTimer().Forget();

        AttackController.StartAttackMove();

        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsHeavyAttacking, false);
    }

    public override void ExecuteOnUpdate()
    {
        var animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        // HeavyAttack 애니메이션이 끝났는지 확인
        if (animatorStateInfo.IsName("HeavyAttack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(PlayerState.Idle);
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("HeavyAttack");
}

public class JumpHeavyAttackState : StateBase
{
    public JumpHeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = 2.5f;
        AttackController.attackMoveDuration = 0.3f;
        Player.rigidbody.velocity = new Vector3(0, StatController.jumpForce, 0);
        Player.AimationController.SetBool(Player.AimationController.IsJumpHeavyAttacking, true);
        Player.StatController.StartHeavyAttackCooldown();
        AttackController.StartAttackMove();
        Player.rigidbody.velocity = new Vector3(Player.rigidbody.velocity.x, StatController.jumpForce, Player.rigidbody.velocity.z);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.StatController.StartCooldownTimer().Forget();
        Player.AimationController.SetBool(Player.AimationController.IsJumpHeavyAttacking, false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player.Ground)
        {
            Player.ChangeState(PlayerState.JumpHeavyAttackLanding);
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        // No specific input handling for JumpHeavyAttackState
    }
    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpHeavyAttack");

}

public class SkillAttackState : StateBase
{
    public SkillAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.AimationController.SetBool(Player.AimationController.IsSkillAttack, true);
        Player.StatController.StartSkill();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsSkillAttack, false);
        EventManager<GameEvents>.TriggerEvent(GameEvents.StartSkillGaugeIncrease);
    }

    public override void ExecuteOnUpdate()
    {
        // 스킬 애니메이션이 끝났는지 확인
        var animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("SkillAttack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(PlayerState.Idle);
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("SkillAttack");
}
