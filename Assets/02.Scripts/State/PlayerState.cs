using Cysharp.Threading.Tasks;
using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpUpState : StateBase
{
    public JumpUpState(PlayerController player) : base(player) { }
    public JumpUpState(PlayerController player, AttackController attackController) : base(player, attackController) { }
    public override void Enter()
    {
        base.Enter();
        Player.Ground = false;
        Player.CanMove = true;
        Player.CanLook = true;
        Player.StateController.IsHitted = false;
        Player.AimationController.SetBool(Player.AimationController.IsJumpingUp, true);
        Player.Jump(); // idle에서 점프인지 아닌지 확인
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsJumpingUp, false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player.rigidbody.velocity.y < -1f)
        {           
            Player.ChangeState(PlayerState.JumpDown);
        }

        // 점프 중 이동 처리
        if (Player.IsMoveInputActive)
        {
            
            Player.Move();
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "HeavyAttack" && context.performed && StatController.currentHeavyAttackCoolTime <= 0)
        {
            if (Player.legendType == PlayerController.LegendType.Peter)
                Player.ChangeState(PlayerState.JumpHeavyAttack);
            else if (Player.legendType == PlayerController.LegendType.Hook)
                Player.ChangeState(PlayerState.HookJumpHeavyAttack);
        }
        else if (context.action.name == "DefaultAttack" && context.performed)
        {
            if (Player.legendType == PlayerController.LegendType.Peter)
                Player.ChangeState(PlayerState.JumpAttack);
            else if (Player.legendType == PlayerController.LegendType.Hook && Player.StateController.hookCanDefaultAttack == true)
                Player.ChangeState(PlayerState.HookFirstJumpAttack);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpUp");
}


public class JumpDownState : StateBase
{
    public JumpDownState(PlayerController player) : base(player) { }
    public JumpDownState(PlayerController player, AttackController attackController) : base(player, attackController) { }


    public override void Enter()
    {
        base.Enter();
        Player.AimationController.SetBool((Player.AimationController.IsJumpingDown), true);
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool((Player.AimationController.IsJumpingDown), false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player.IsGrounded)
        {     
            Player.ChangeState(PlayerState.JumpLand);
        }

        // 점프 중 이동 처리
        if (Player.IsMoveInputActive)
        {
            Player.Move();
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "HeavyAttack" && context.performed && Player.Ground == false && StatController.currentHeavyAttackCoolTime <= 0)
        {
            if (Player.legendType == PlayerController.LegendType.Peter)
                Player.ChangeState(PlayerState.JumpHeavyAttack);
            else if (Player.legendType == PlayerController.LegendType.Hook)
                Player.ChangeState(PlayerState.HookJumpHeavyAttack);
        }
        else if (context.action.name == "DefaultAttack" && context.performed && Player.IsGrounded == false)
        {
            if (Player.legendType == PlayerController.LegendType.Peter)
                Player.ChangeState(PlayerState.JumpAttack);
            else if (Player.legendType == PlayerController.LegendType.Hook && Player.StateController.hookCanDefaultAttack == true)
                Player.ChangeState(PlayerState.HookFirstJumpAttack);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpDown");

}


public class JumpLandState : StateBase
{
    public JumpLandState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.AimationController.SetBool(Player.AimationController.IsLanding, true);
        Player.Land();
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsLanding, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        switch (Player.legendType)
        {
            case PlayerController.LegendType.Peter:
                if (Player.IsGrounded)
                {
                    if (Player.IsMoveInputActive)
                    {
                        if (Player.IsGrounded && stateInfo.normalizedTime >= 0.6f)
                            Player.ChangeState(PlayerState.Run);
                    }
                    else
                    {
                        Player.ChangeState(PlayerState.Idle);
                    }
                }
            break;

            case PlayerController.LegendType.Hook:

                if (Player.IsMoveInputActive)
                {
                    if (Player.IsGrounded && stateInfo.normalizedTime >= 0.6f)
                        Player.ChangeState(PlayerState.Run);
                }
                else
                {
                    if (Player.IsGrounded && stateInfo.normalizedTime >= 0.9f)
                        Player.ChangeState(PlayerState.Idle);
                }
            break;
        }
    }
    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpLand");
}





public class JumpAttackLandingState : StateBase
{
    public JumpAttackLandingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.AimationController.SetBool((Player.AimationController.IsLightLanding), true);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.Land();
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool((Player.AimationController.IsLightLanding), false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("JumpLightLanding") && stateInfo.normalizedTime >= 0.6f)
        {
            if (Player.IsMoveInputActive)
            {
                Player.ChangeState(PlayerState.Run);
            }
            else
            {
                Player.ChangeState(PlayerState.Idle);
            }
        }
    }
    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpLightLanding");
}



public class RunState : StateBase
{
    public RunState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.AimationController.SetBool(Player.AimationController.IsRunning, true);
        Player.StateController.hookCanDefaultAttack = true;
        Player.isIdleJump = false;
        Player.CanMove = true;
        Player.CanLook = true;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsRunning, false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player.moveDirection == Vector3.zero)
        {
            Player.ChangeState(PlayerState.Idle);
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Jump" && context.performed)
        {
            Player.ChangeState(PlayerState.JumpUp);
        }
        else if (context.action.name == "DefaultAttack" && context.performed)
        {
            Player.rigidbody.velocity = new Vector3(0, Player.rigidbody.velocity.y, 0);
            if (Player.legendType == PlayerController.LegendType.Peter)
                Player.ChangeState(PlayerState.FirstAttack);
            else if (Player.legendType == PlayerController.LegendType.Hook)
                Player.ChangeState(PlayerState.HookFirstAttack);
        }
        else if (context.action.name == "HeavyAttack" && context.performed && StatController.currentHeavyAttackCoolTime <= 0)
        {
            Player.rigidbody.velocity = new Vector3(0, Player.rigidbody.velocity.y, 0);
            if (Player.legendType == PlayerController.LegendType.Peter)
                Player.ChangeState(PlayerState.HeavyAttack);
            else if (Player.legendType == PlayerController.LegendType.Hook)
                Player.ChangeState(PlayerState.HookHeavyAttack);
        }
        else if (context.action.name == "SkillAttack" && context.performed && StatController.CanSkillAttack)
        {
            if (Player.legendType == PlayerController.LegendType.Peter)
                Player.ChangeState(PlayerState.SkillAttack);
            else if (Player.legendType == PlayerController.LegendType.Hook)
                Player.ChangeState(PlayerState.HookSkillAttack);
        }
    }
    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("Run");
}





public class JumpHeavyAttackLandingState : StateBase
{
    public JumpHeavyAttackLandingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.AimationController.SetBool(Player.AimationController.IsHeavyLanding, true);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.Land();
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsHeavyLanding, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo stateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("JumpHeavyAttackLanding") && stateInfo.normalizedTime >= 1.0f)
        {
            Player.ChangeState(PlayerState.Idle);
        }
    }
    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("JumpHeavyAttack");
}



public class HitState : StateBase
{
    public HitState(PlayerController player) : base(player) { }
    public HitState(PlayerController player, AttackController attackController) : base(player, attackController) { }

    public override void Enter()
    {
        base.Enter();
        Player.AimationController.SetBool(Player.AimationController.IsHit, true);
        Player.EffectController.StartHitFlashEffect();
        Player.StateController.IsHitted = true;
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsHit, false);
    }

    public override void ExecuteOnUpdate()
    {
        Quaternion rotation = Player.transform.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
        Player.transform.rotation = rotation;

        AnimatorStateInfo animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("Hit") && animatorStateInfo.normalizedTime >= .9f)
        {
            Player.ChangeState(PlayerState.Idle);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("Hit");
}

public class HitUpState : StateBase
{
    public HitUpState(PlayerController player) : base(player) { }
    public HitUpState(PlayerController player, AttackController attackController) : base(player, attackController) { }

    public override void Enter()
    {
        base.Enter();
        Player.StateController.IsHitted = true;
        Player.AimationController.SetBool(Player.AimationController.IsHitUp, true);
        Player.EffectController.StartHitFlashEffect();
        Player.CanMove = false;
        Player.CanLook = false;
        Player.isIdleJump = false;

    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsHitUp, false);
    }

    public override void ExecuteOnUpdate()
    {
        Quaternion rotation = Player.transform.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
        Player.transform.rotation = rotation;

        AnimatorStateInfo animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (Player.IsGrounded == true && animatorStateInfo.IsName("HitUp") && animatorStateInfo.normalizedTime >= .5f)
        {
            Player.ChangeState(PlayerState.HitDown);
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Jump" && context.performed && Player.CanChange == true)
        {
            Player.rigidbody.velocity = Vector3.zero;
            Player.ChangeState(PlayerState.JumpUp);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class HitDownState : StateBase
{
    public HitDownState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.StateController.IsHitted = true;
        Player.AimationController.SetBool(Player.AimationController.IsHitDown, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsHitDown, false);
    }

    public override void ExecuteOnUpdate()
    {
        Quaternion rotation = Player.transform.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
        Player.transform.rotation = rotation;

        AnimatorStateInfo animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("HitDown") && animatorStateInfo.normalizedTime >= .5f)
        {
            Player.ChangeState(PlayerState.HitLand);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class HitLandState : StateBase
{
    public HitLandState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.StateController.IsHitted = true;
        Player.AimationController.SetBool(Player.AimationController.IsHitLand, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsHitLand, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("HitLand") && animatorStateInfo.normalizedTime >= .9f)
        {
            Player.ChangeState(PlayerState.DownIdle);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class DownIdleState : StateBase
{
    public DownIdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.StateController.IsHitted = false;
        Player.AimationController.SetBool(Player.AimationController.IsDownIdle, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsDownIdle, false);
    }

    public override void ExecuteOnUpdate()
    {
        if (Player.moveDirection != Vector3.zero)
        {
            Vector3 currentDirection = Player.transform.forward;
            Vector3 inputDirection = new Vector3(Player.moveDirection.x, 0, Player.moveDirection.z).normalized;

            float angle = Vector3.SignedAngle(currentDirection, inputDirection, Vector3.up);

            if (angle > 135 || angle < -135) // 반대 방향이면 RollUpBackState로 전환
            {
                Player.ChangeState(PlayerState.RollUpBack);
            }
            else // 같은 방향이면 좌우 회전 후 RollUpFrontState로 전환
            {
                Player.transform.rotation = Quaternion.LookRotation(inputDirection);
                Player.CanLook = true;
                Player.ChangeState(PlayerState.RollUpFront);
            }
        }
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {       
        if (context.action.name == "Jump" && context.performed)
        {
            Player.ChangeState(PlayerState.StandUp);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class RollUpFrontState : StateBase
{
    public RollUpFrontState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.StartRollUpMove();
        Player.StateController.IsHitted = false;
        Player.EffectController.StartInvincibleFlashEffect(5);          
        Player.AimationController.SetBool(Player.AimationController.IsRollUpFront, true);
        Player.CanMove = false;
        Player.CanLook = true;
    }

    public override void Exit()
    {
        base.Exit();
        Player.StateController.IsInvincible = false;
        Player.AimationController.SetBool(Player.AimationController.IsRollUpFront, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("RollUpFront") && animatorStateInfo.normalizedTime >= .9f)
        {           
            Player.ChangeState(PlayerState.Idle);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class RollUpBackState : StateBase
{
    public RollUpBackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.StartRollUpMove();
        Player.StateController.IsHitted = false;
        Player.EffectController.StartInvincibleFlashEffect(5);
        Player.AimationController.SetBool(Player.AimationController.IsRollUpBack, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.StateController.IsInvincible = false;
        Player.AimationController.SetBool(Player.AimationController.IsRollUpBack, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("RollUpBack") && animatorStateInfo.normalizedTime >= .9f)
        {
            Player.ChangeState(PlayerState.Idle);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}


public class StandUpState : StateBase
{
    public StandUpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.StateController.IsHitted = false;
        Player.EffectController.StartInvincibleFlashEffect(5);
        Player.AimationController.SetBool(Player.AimationController.IsStandUp, true);
        Player.CanMove = false;
        Player.CanLook = false;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsStandUp, false);
    }

    public override void ExecuteOnUpdate()
    {
        AnimatorStateInfo animatorStateInfo = Player.AimationController.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("StandUp") && animatorStateInfo.normalizedTime >= .9f)
        {
            Player.ChangeState(PlayerState.Idle);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("HitUp");
}

public class HangState : StateBase
{
    private float initialY;
    public HangState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.StateController.IsHitted = false;
        Player.AimationController.SetBool(Player.AimationController.IsHang, true);
        Player.CanMove = false;
        Player.CanLook = false;
        Player.rigidbody.useGravity = false;
        Player.FallAsync().Forget();
        Player.EffectController.StartInvincibleFlashEffect(5);
        initialY = Player.transform.position.y;
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsHang, false);
        Player.EffectController.StartInvincibleFlashEffect(3);
        Player.EscapeInHang();
        Player.rigidbody.useGravity = true;
    }

    public override void ExecuteOnUpdate()
    {
        base.ExecuteOnUpdate();

        // y 값을 고정
        Vector3 fixedPosition = new Vector3(Player.transform.position.x, initialY, Player.transform.position.z);
        Player.transform.position = fixedPosition;
    }

    public override void OnInputCallback(InputAction.CallbackContext context)
    {
        if (context.action.name == "Jump" && context.performed)
        {
            Player.ChangeState(PlayerState.JumpUp);
        }
    }

    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("Hang");
}

public class HangFallState : StateBase
{
    public HangFallState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Player.AimationController.SetBool(Player.AimationController.IsHangFalling, true);
        Player.rigidbody.velocity = Vector3.zero;
        Debug.Log(Player.rigidbody.velocity.y);
        Player.HangFall().Forget();
    }

    public override void Exit()
    {
        base.Exit();
        Player.AimationController.SetBool(Player.AimationController.IsHangFalling, false);
    }


    public override bool IsTransitioning => !Player.AimationController.GetCurrentAnimatorStateInfo(0).IsName("Hang");
}


