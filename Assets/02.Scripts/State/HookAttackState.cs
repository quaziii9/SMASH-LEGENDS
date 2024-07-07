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
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack1, false);
    }

    public override void ExecuteOnUpdate()
    {
        // Attack animation finished
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        if (Player.IsGrounded)
        {
            if (stateInfo.IsName("JumpAttackCombo1") && stateInfo.normalizedTime >= 0.3f)
            {
                Player.ChangeState(PlayerState.JumpLand);
            }
            else
            {
                Player.ChangeState(PlayerState.JumpAttackLanding);
            }
        }
        else
        {
            if (stateInfo.IsName("JumpAttackCombo1") && stateInfo.normalizedTime >= 0.9f)
                Player.ChangeState(PlayerState.JumpDown);
        }
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
        Player.rigidbody.velocity = new Vector3(0, 3f, 0);
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack2, true);
        AttackController.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack2, false);
    }

    public override void ExecuteOnUpdate()
    {
        // Attack animation finished
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        if (Player.IsGrounded)
        {
            if (stateInfo.IsName("JumpAttackCombo2") && stateInfo.normalizedTime >= 0.3f)
            {
                Player.ChangeState(PlayerState.JumpLand);
            }
            else
            {
                Player.ChangeState(PlayerState.JumpAttackLanding);
            }
        }
        else
        {
            if (stateInfo.IsName("JumpAttackCombo2") && stateInfo.normalizedTime >= 0.9f)
                Player.ChangeState(PlayerState.JumpDown);
        }
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
        Player.rigidbody.velocity = new Vector3(0, 4f, 0);
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack3, true);
        AttackController.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsJumpComboAttack3, false);
    }

    public override void ExecuteOnUpdate()
    {
        // Attack animation finished
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);

        if(Player.IsGrounded)
        {
            if (stateInfo.IsName("JumpAttackCombo3") && stateInfo.normalizedTime >= 0.3f)
            {
                Player.ChangeState(PlayerState.JumpLand);
            }
            else
            {
                Player.ChangeState(PlayerState.JumpAttackLanding);
            }
        }
        else
        {
            if (stateInfo.IsName("JumpAttackCombo3") && stateInfo.normalizedTime >= 0.9f)
                Player.ChangeState(PlayerState.JumpDown);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpAttackCombo3");
}


public class HookJumpHeavyAttackState : StateBase
{
    private float initialY;
    public HookJumpHeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();

        initialY = Player.transform.position.y; // 현재 Y 위치를 저장
        Player.AimationController.SetBool(Player.AimationController.IsJumpHeavyAttacking, true);
        Player.StatController.StartHeavyAttackCooldown();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.rigidbody.useGravity = false;
        Player.HookJumpHeavyAttackMove = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.StatController.StartCooldownTimer().Forget();
        Player.AimationController.SetBool(Player.AimationController.IsJumpHeavyAttacking, false);
        Player.rigidbody.useGravity = true; // 중력 활성화
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 0.9f)
        {
            Player.ChangeState(PlayerState.JumpDown);
        }

        if (Player.HookJumpHeavyAttackMove == false)
        {
            // Y 위치를 고정
            Player.transform.position = new Vector3(Player.transform.position.x, initialY, Player.transform.position.z);
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        // No specific input handling for JumpHeavyAttackState
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpHeavyAttack");


}

