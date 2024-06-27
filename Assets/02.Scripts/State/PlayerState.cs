using UnityEngine;
using UnityEngine.InputSystem;

public class JumpUpState : StateBase
{
    public JumpUpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._Ground = false;
        Player._animationController.SetBool(Player._animationController.IsJumpingUp, true);
        Player.Jump(Player._moveDirection == Vector3.zero); // idle에서 점프인지 아닌지 확인
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsJumpingUp, false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._rigidbody.velocity.y < -1f)
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

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("JumpUp");
}


public class JumpDownState : StateBase
{
    public JumpDownState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animationController.SetBool((Player._animationController.IsJumpingDown), true);
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool((Player._animationController.IsJumpingDown), false);
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
        if (context.action.name == "HeavyAttack" && context.performed && Player._Ground == false)
        {
            Player.ChangeState(new JumpHeavyAttackState(Player));
        }
        else if (context.action.name == "DefaultAttack" && context.performed && Player._isGrounded == false)
        {
            Player.ChangeState(new JumpAttackState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("JumpDown");

}


public class JumpLandState : StateBase
{
    public JumpLandState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animationController.SetBool(Player._animationController.IsLanding, true);
        Player.Land();
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsLanding, false);
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
    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("JumpLand");
}



public class JumpAttackState : StateBase
{
    public JumpAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animationController.SetBool((Player._animationController.IsJumpAttacking), true);
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool((Player._animationController.IsJumpAttacking), false);
    }

    public override void ExecuteOnUpdate()
    {
        // Attack animation finished
        AnimatorStateInfo stateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
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
    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack");

}

public class JumpLightLandingState : StateBase
{
    public JumpLightLandingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animationController.SetBool((Player._animationController.IsLightLanding), true);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.Land();
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool((Player._animationController.IsLightLanding), false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo stateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
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
    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("JumpLightLanding");
}



public class RunState : StateBase
{
    public RunState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animationController.SetBool(Player._animationController.IsRunning, true);
        Player.CanMove = true;
        Player.CanLook = true;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsRunning, false);
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
            Player._rigidbody.velocity = new Vector3(0, Player._rigidbody.velocity.y, 0);
            Player.ChangeState(new FirstAttackState(Player));
        }
        else if (context.action.name == "HeavyAttack" && context.performed)
        {
            Player._rigidbody.velocity = new Vector3(0, Player._rigidbody.velocity.y, 0);
            Player.ChangeState(new HeavyAttackState(Player));
        }
        else if (context.action.name == "SkillAttack" && context.performed)
        {
            Player.ChangeState(new SkillAttackState(Player));
        }
    }
    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("Run");
}


public class FirstAttackState : StateBase
{
    public FirstAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController._attackMoveDistance = 1.5f;
        AttackController._attackMoveDuration = 0.3f;
        Player._animationController.SetBool(Player._animationController.IsComboAttack1, true);
        Player.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsComboAttack1, false);
    }

    public override void ExecuteOnUpdate()
    {
        var animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);

        // FirstAttack 애니메이션이 끝났는지 확인
        if (animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(new IdleState(Player));
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed && Player.CanChange)
        {
            Player.ChangeState(new SecondAttackState(Player));
        }
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("FirstAttack");
}

public class SecondAttackState : StateBase
{
    public SecondAttackState(PlayerController player) : base(player) { }


    public override void Enter()
    {
        base.Enter();
        AttackController._attackMoveDistance = 1.5f;
        AttackController._attackMoveDuration = 0.3f;
        Player._animationController.SetBool(Player._animationController.IsComboAttack2, true);
        Player.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsComboAttack2, false);
    }

    public override void ExecuteOnUpdate()
    {
        var animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);

        // SecondAttack 애니메이션이 끝났는지 확인
        if (animatorStateInfo.IsName("SecondAttack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(new IdleState(Player));
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "DefaultAttack" && context.performed && Player.CanChange)
        {
            Player.ChangeState(new FinishAttackState(Player));
        }
        else if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("SecondAttack");
}

public class FinishAttackState : StateBase
{
    public FinishAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController._attackMoveDistance = 1.5f;
        AttackController._attackMoveDuration = 0.3f;
        Player._animationController.SetBool(Player._animationController.IsComboAttack3, true);
        Player.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsComboAttack3, false);
    }

    public override void ExecuteOnUpdate()
    {
        var animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);

        // FinishAttack 애니메이션이 끝났는지 확인
        if (animatorStateInfo.IsName("FinishAttack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(new IdleState(Player));
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("FinishAttack");
}

public class HeavyAttackState : StateBase
{
    public HeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController._attackMoveDistance = 1.5f;
        AttackController._attackMoveDuration = 0.3f;
        Player._animationController.SetBool(Player._animationController.IsHeavyAttacking, true);
        Player.StartAttackMove();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsHeavyAttacking, false);
        Player._attackController.StartHeavyAttackCooldown();

    }

    public override void ExecuteOnUpdate()
    {
        var animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);

        // HeavyAttack 애니메이션이 끝났는지 확인
        if (animatorStateInfo.IsName("HeavyAttack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(new IdleState(Player));
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("HeavyAttack");
}



public class JumpHeavyAttackState : StateBase
{
    public JumpHeavyAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController._attackMoveDistance = 2.5f;
        AttackController._attackMoveDuration = 0.3f;
        Player._rigidbody.velocity = new Vector3(0, AttackController.jumpForce, 0);
        Player._animationController.SetBool(Player._animationController.IsJumpHeavyAttacking, true);
        Player.StartAttackMove();
        Player._rigidbody.velocity = new Vector3(Player._rigidbody.velocity.x, AttackController.jumpForce, Player._rigidbody.velocity.z);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsJumpHeavyAttacking, false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._Ground)
        {
            Player.ChangeState(new JumpHeavyAttackLandingState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        // No specific input handling for JumpHeavyAttackState
    }
    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("JumpHeavyAttack");

}


public class JumpHeavyAttackLandingState : StateBase
{
    public JumpHeavyAttackLandingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animationController.SetBool(Player._animationController.IsHeavyLanding, true);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.Land();
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsHeavyLanding, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo stateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("JumpHeavyAttackLanding") && stateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }
    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("JumpHeavyAttack");
}



public class SkillAttackState : StateBase
{
    public SkillAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player._animationController.SetBool(Player._animationController.IsSkillAttack, true);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.CanChange = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsSkillAttack, false);
    }

    public override void ExecuteOnUpdate()
    {
        // 스킬 애니메이션이 끝났는지 확인
        var animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("SkillAttack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(new IdleState(Player));
        }

        // CanMove가 true일 때 이동 처리
        if (Player.CanMove && Player.IsMoveInputActive)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move" && context.ReadValue<Vector2>() != Vector2.zero && Player.CanMove)
        {
            Player.ChangeState(new RunState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("SkillAttack");
}


public class HitState : StateBase
{
    public HitState(PlayerController player) : base(player) { }
    public HitState(PlayerController player, AttackController attackController) : base(player, attackController) { }

    public override void Enter()
    {
        base.Enter();
        Player._animationController.SetBool(Player._animationController.IsHit, true);
        Player.IsHitted = true;
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsHit, false);
    }

    public override void ExecuteOnUpdate()
    {
        Quaternion rotation = Player.transform.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
        Player.transform.rotation = rotation;

        AnimatorStateInfo animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("Hit") && animatorStateInfo.normalizedTime >= .9f)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("Hit");
}

public class HitUpState : StateBase
{
    public HitUpState(PlayerController player) : base(player) { }
    public HitUpState(PlayerController player, AttackController attackController) : base(player, attackController) { }

    public override void Enter()
    {
        base.Enter();
        Player.IsHitted = true;
        Player._animationController.SetBool(Player._animationController.IsHitUp, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsHitUp, false);
    }

    public override void ExecuteOnUpdate()
    {
        Quaternion rotation = Player.transform.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
        Player.transform.rotation = rotation;

        AnimatorStateInfo animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
        if (Player._isGrounded == true && animatorStateInfo.IsName("HitUp") && animatorStateInfo.normalizedTime >= .5f)
        {
            Player.ChangeState(new HitDownState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class HitDownState : StateBase
{
    public HitDownState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("enterhitdown");
        Player.IsHitted = true;
        Player._animationController.SetBool(Player._animationController.IsHitDown, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsHitDown, false);
    }

    public override void ExecuteOnUpdate()
    {
        Quaternion rotation = Player.transform.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
        Player.transform.rotation = rotation;

        AnimatorStateInfo animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("HitDown") && animatorStateInfo.normalizedTime >= .5f)
        {
            Player.ChangeState(new HitLandState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class HitLandState : StateBase
{
    public HitLandState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.IsHitted = true;
        Player._animationController.SetBool(Player._animationController.IsHitLand, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsHitLand, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("HitLand") && animatorStateInfo.normalizedTime >= .9f)
        {
            Player.ChangeState(new DownIdleState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class DownIdleState : StateBase
{
    public DownIdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.IsHitted = false;
        Player._animationController.SetBool(Player._animationController.IsDownIdle, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsDownIdle, false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player._moveDirection != Vector3.zero)
        {
            Vector3 currentDirection = Player.transform.forward;
            Vector3 inputDirection = new Vector3(Player._moveDirection.x, 0, Player._moveDirection.z).normalized;

            float angle = Vector3.SignedAngle(currentDirection, inputDirection, Vector3.up);

            if (angle > 135 || angle < -135) // 반대 방향이면 RollUpBackState로 전환
            {
                Player.ChangeState(new RollUpBackState(Player));
            }
            else // 같은 방향이면 좌우 회전 후 RollUpFrontState로 전환
            {
                Player.transform.rotation = Quaternion.LookRotation(inputDirection);
                Player.CanLook = true;
                Player.ChangeState(new RollUpFrontState(Player));
            }

            //if (Player._moveDirection.x != 0)
            //{
            //    float angle = Vector3.SignedAngle(currentDirection, inputDirection, Vector3.up);
            //    if (angle > 90 || angle < -90) // 반대 방향이면 RollUpBackState로 전환
            //    {
            //        Player.ChangeState(new RollUpBackState(Player));
            //    }
            //    else // 같은 방향이면 RollUpFrontState로 전환
            //    {
            //        Player.ChangeState(new RollUpFrontState(Player));
            //    }
            //}
            //// z축으로 이동할 때는 회전하고 RollUpFrontState로 전환
            //else if (Player._moveDirection.z != 0)
            //{
            //    Player.transform.rotation = Quaternion.LookRotation(inputDirection);
            //    Player.CanLook = true;
            //    Player.ChangeState(new RollUpFrontState(Player));
            //}
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {       

        if (context.action.name == "Jump" && context.performed)
        {
            Player.ChangeState(new StandUpState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class RollUpFrontState : StateBase
{
    public RollUpFrontState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController._attackMoveDistance = 2.5f;
        AttackController._attackMoveDuration = 0.3f;
        Player.StartAttackMove();
        Player.IsHitted = false;
        Player._animationController.SetBool(Player._animationController.IsRollUpFront, true);
        Player.CanMove = false;
        Player.CanLook = true;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsRollUpFront, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("RollUpFront") && animatorStateInfo.normalizedTime >= .9f)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class RollUpBackState : StateBase
{
    public RollUpBackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        AttackController._attackMoveDistance = 2.5f;
        AttackController._attackMoveDuration = 0.3f;
        Player.StartAttackMove();
        Player.IsHitted = false;
        Player._animationController.SetBool(Player._animationController.IsRollUpBack, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsRollUpBack, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("RollUpBack") && animatorStateInfo.normalizedTime >= .9f)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}


public class StandUpState : StateBase
{
    public StandUpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.IsHitted = false;
        Player._animationController.SetBool(Player._animationController.IsStandUp, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player._animationController.SetBool(Player._animationController.IsStandUp, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo animatorStateInfo = Player._animationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("StandUp") && animatorStateInfo.normalizedTime >= .9f)
        {
            Player.ChangeState(new IdleState(Player));
        }
    }

    public override bool IsTransitioning => !Player._animationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}


