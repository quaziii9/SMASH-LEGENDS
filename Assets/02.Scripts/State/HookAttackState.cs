using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.InputSystem;

public class HookFirstAttackState : StateBase
{
    public HookFirstAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.AimationController.SetBool(Player.AimationController.IsComboAttack1, true);
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
            Player.ChangeState(PlayerState.HookSecondAttack);
        }
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("FirstAttack");
}

public class HookSecondAttackState : StateBase
{
    public HookSecondAttackState(PlayerController player) : base(player) { }


    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = -.5f;
        AttackController.attackMoveDuration = 0.1f;
        Player.AimationController.SetBool(Player.AimationController.IsComboAttack2, true);
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
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(PlayerState.Run);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("SecondAttack");
}

public class HookHeavyAttackState : StateBase
{
    public HookHeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = -.7f;
        AttackController.attackMoveDuration = 0.1f;
        Player.AimationController.SetBool(Player.AimationController.IsHeavyAttacking, true);
        Player.StatController.StartHeavyAttackCooldown();
        Player.StatController.StartCooldownTimer().Forget();
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




public class HookFirstJumpAttackState : StateBase
{
    public HookFirstJumpAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = -.5f;
        AttackController.attackMoveDuration = 0.1f;
        Player.rigidbody.velocity = new Vector3(0, 2f, 0);
        Player.StateController.hookCanDefaultAttack = false;
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack1, true);
        AttackController.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
        Player.rigidbody.useGravity = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack1, false);
        Player.rigidbody.useGravity = true;
    }

    public override void ExecuteOnUpdate()
    {
        // Attack animation finished
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.IsName("JumpAttackCombo1") && stateInfo.normalizedTime >=0.9f)
        {
            if(Player.IsGrounded)
            {
                Player.ChangeState(PlayerState.JumpAttackLanding);
            }
            else
            {
                Player.ChangeState(PlayerState.JumpDown);
            }
        }
       Player.rigidbody.velocity = new Vector3(Player.rigidbody.velocity.x, Player.rigidbody.velocity.y * 0.2f, Player.rigidbody.velocity.z);
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player.ChangeState(PlayerState.HookSecondJumpAttack);
        }

    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpAttackCombo1");
}

public class HookSecondJumpAttackState : StateBase
{
    public HookSecondJumpAttackState(PlayerController player) : base(player) { }


    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = -.5f;
        AttackController.attackMoveDuration = 0.1f;
        Player.rigidbody.velocity = new Vector3(0, 2f, 0);
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack2, true);
        AttackController.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
        Player.rigidbody.useGravity = false;

    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack2, false);
        Player.rigidbody.useGravity = true;
    }

    public override void ExecuteOnUpdate()
    {
        // Attack animation finished
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("JumpAttackCombo2") && stateInfo.normalizedTime >= 0.9f)
        {
            if (Player.IsGrounded)
            {
                Player.ChangeState(PlayerState.JumpAttackLanding);
            }
            else
            {
                Player.ChangeState(PlayerState.JumpDown);
            }
        }
       Player.rigidbody.velocity = new Vector3(Player.rigidbody.velocity.x, Player.rigidbody.velocity.y * 0.2f, Player.rigidbody.velocity.z);
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed && Player.CanChange)
        {
            Player.ChangeState(PlayerState.HookFinsihJumpAttack);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpAttackCombo2");
}

public class HookFinishJumpAttackState : StateBase
{
    public HookFinishJumpAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController.attackMoveDistance = -.5f;
        AttackController.attackMoveDuration = 0.1f;
        Player.rigidbody.velocity = new Vector3(0, 2f, 0);
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack3, true);
        AttackController.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
        Player.rigidbody.useGravity = false;

    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack3, false);
        Player.rigidbody.useGravity = true;
    }

    public override void ExecuteOnUpdate()
    {
        // Attack animation finished
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("JumpAttackCombo3") && stateInfo.normalizedTime >= 0.9f)
        {
            if (Player.IsGrounded)
            {
                Player.ChangeState(PlayerState.JumpAttackLanding);
            }
            else
            {
                Player.ChangeState(PlayerState.JumpDown);
            }
        }
        Player.rigidbody.velocity = new Vector3(Player.rigidbody.velocity.x, Player.rigidbody.velocity.y * 0.2f, Player.rigidbody.velocity.z);
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpAttackCombo3");
}