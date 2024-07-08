using UnityEngine;
using Mirror;

public enum HitType
{
    Hit,
    HitUp,
    None,
}

public class AttackController : NetworkBehaviour
{
    private PlayerController player;
    private StatController statController; // StatController 추가

    [SyncVar] public int DamageAmount;
    [SyncVar] public float KnockBackPower = 1;
    [SyncVar] public Vector3 KnockBackDireciton;
    [SyncVar] public HitType hitType;
    [SyncVar] public bool PlusAddForce = true;

    [Header("Detection")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;

    public float attackMoveDistance;
    public float attackMoveDuration;
    private float _attackMoveStartTime;
    private float _currentMoveDistance;
    private Vector3 _attackMoveDirection;

    public AnimationCurve movementCurve;

    public void Initialize(PlayerController playerController, StatController StatController)
    {
        player = playerController;
        statController = StatController; 
    }

    public void HandleAttack(PlayerState state)
    {
        if(player.legendType ==  PlayerController.LegendType.Peter)
        {
            switch (state)
            {
                case PlayerState.FirstAttack:
                    SetAttackValues(statController.defaultAttackDamage / 3, statController.defaultKnockBackPower, player.transform.up * 0.5f, HitType.Hit, false);
                    break;
                case PlayerState.SecondAttack:
                    SetAttackValues(statController.defaultAttackDamage / 6, statController.defaultKnockBackPower, player.transform.up * 0.5f, HitType.Hit, false);
                    break;
                case PlayerState.FinishAttack:
                    SetAttackValues(statController.defaultAttackDamage / 3, statController.heavyKnockBackPower, player.transform.up * 1.2f, HitType.HitUp, true);
                    break;
                case PlayerState.JumpAttack:
                    SetAttackValues((int)(statController.defaultAttackDamage * 0.6f), statController.heavyKnockBackPower, player.transform.up * 1.2f, HitType.HitUp, false);
                    break;
                case PlayerState.HeavyAttack:
                    SetAttackValues(statController.heavyAttackDamage, statController.heavyKnockBackPower, player.transform.up * 1.2f, HitType.HitUp, true);
                    break;
                case PlayerState.JumpHeavyAttackLanding:
                case PlayerState.JumpHeavyAttack:
                    SetAttackValues(statController.heavyAttackDamage / 3 * 2, statController.heavyKnockBackPower, player.transform.up * 1.2f, HitType.HitUp, true);
                    break;
                case PlayerState.SkillAttack:
                    SetAttackValues((statController.skillAttackDamage - 500) / 5, statController.defaultKnockBackPower, player.transform.up, HitType.Hit, false);
                    break;
            }
        }
        else if (player.legendType == PlayerController.LegendType.Hook)
        {
            switch (state)
            {
                case PlayerState.HookFirstAttack:
                    SetAttackValues((statController.defaultAttackDamage - 100) / 4, 0, Vector3.zero , HitType.Hit, false);
                    break;
                case PlayerState.HookSecondAttack:
                    SetAttackValues((statController.defaultAttackDamage - 100) / 4 + 50, 0, Vector3.zero, HitType.Hit, false);
                    break;
                case PlayerState.HookFirstJumpAttack:
                case PlayerState.HookSecondJumpAttack:
                    SetAttackValues(statController.defaultAttackDamage / 7, statController.defaultKnockBackPower * 0.2f, Vector3.up * 0.5f, HitType.Hit, false);
                    break;
                case PlayerState.HookFinsihJumpAttack:
                    SetAttackValues(statController.defaultAttackDamage / 7, statController.defaultKnockBackPower, Vector3.up , HitType.Hit, false);
                    break;
                case PlayerState.HookHeavyAttack:
                case PlayerState.HookJumpHeavyAttack:
                    SetAttackValues(statController.heavyAttackDamage / 9, statController.defaultKnockBackPower * 0.2f, Vector3.up * 0.5f, HitType.Hit, false);
                    break;

            }
        }
    }

    public void SetAttackValues(int damage, float knockBackPower, Vector3 knockBackDirection, HitType hitType, bool plusAddForce)
    {
        DamageAmount = damage;
        KnockBackPower = knockBackPower;
        KnockBackDireciton = knockBackDirection;
        this.hitType = hitType;
        PlusAddForce = plusAddForce;
        if (hitType == HitType.Hit) statController.AddSkillGuage = 100;
        else statController.AddSkillGuage = 300;

    }

    public void SkillLastAttackDamage()
    {
        SetAttackValues(((statController.skillAttackDamage -500 ) / 5 + 500), statController.heavyKnockBackPower, player.transform.forward + player.transform.up * 1.2f, HitType.HitUp, true);
    }

   

    public void HandleAttackMove()
    {
        float elapsedTime = Time.time - _attackMoveStartTime;
        float fraction = elapsedTime / attackMoveDuration;
        float distanceToMove = Mathf.Lerp(0, attackMoveDistance, fraction);

        Vector3 forwardMovement = _attackMoveDirection * (distanceToMove - _currentMoveDistance);
        player.rigidbody.MovePosition(player.rigidbody.position + forwardMovement);

        _currentMoveDistance = distanceToMove;

        if (player.CanLook && player.moveDirection != Vector3.zero)
        {
            player.LookAt();
        }
    }

    public void HandleHookAttackMove()
    {
        float elapsedTime = Time.time - _attackMoveStartTime;
        float fraction = elapsedTime / attackMoveDuration;
        float movementFactor = movementCurve.Evaluate(fraction); // 애니메이션 커브를 통해 절도 있게 이동

        float distanceToMove = attackMoveDistance * movementFactor;

        Vector3 forwardMovement = _attackMoveDirection * (distanceToMove - _currentMoveDistance);
        player.rigidbody.MovePosition(player.rigidbody.position + forwardMovement);

        _currentMoveDistance = distanceToMove;

        if (player.CanLook && player.moveDirection != Vector3.zero)
        {
            player.LookAt();
        }
    }

    public void StartAttackMove()
    {
        if (!player.isLocalPlayer) return;
        if (player.StateController.CurState != PlayerState.RollUpBack && player.StateController.CurState != PlayerState.RollUpFront)
            RotateTowardsNearestPlayer();

        if (player.StateController.CurState == PlayerState.SkillAttack)
        {
            attackMoveDistance = 8f;
            attackMoveDuration = 1.2f;
        }
        _attackMoveStartTime = Time.time;
        _currentMoveDistance = 0;
        if (player.StateController.CurState == PlayerState.RollUpBack || player.StateController.CurState == PlayerState.HookSecondAttack ||
            player.StateController.CurState == PlayerState.HookFirstJumpAttack || player.StateController.CurState == PlayerState.HookSecondJumpAttack ||
            player.StateController.CurState == PlayerState.HookFinsihJumpAttack || player.StateController.CurState == PlayerState.HookHeavyAttack || 
            player.StateController.CurState == PlayerState.HookJumpHeavyAttack)
        {
            _attackMoveDirection = -player.transform.forward;
        }
        else
            _attackMoveDirection = player.transform.forward;
    }

    public void PlayerGetKnockBack(float knockBackPower, Vector3 knockBackDirection, HitType hitType, bool plusAddForce)
    {
        float finalKnockBackPower = knockBackPower;
        float currentHp = player.StatController.currentHp;
        float maxHp = player.StatController.maxHp;

        switch (hitType)
        {
            case HitType.Hit:
                player.ChangeState(PlayerState.Hit);
                break;
            case HitType.HitUp:
                player.ChangeState(PlayerState.HitUp);
                break;
            default:
                break;
        }

        if(currentHp <=0)
        {
            if (knockBackDirection.y < 1.2f)
            {
                knockBackDirection.y = 1.2f;
            }

            float dieKnockbackPower = 5f;
            player.rigidbody.velocity = Vector3.zero;
            player.rigidbody.AddForce(knockBackDirection * dieKnockbackPower, ForceMode.Impulse);
            return;
        }

        if (plusAddForce == true)
        {
            if (currentHp > maxHp / 3 * 2)
            {
                finalKnockBackPower = knockBackPower;
            }
            else if (currentHp > maxHp / 3 && currentHp <= maxHp / 3 * 2)
            {
                finalKnockBackPower = knockBackPower * 1.4f;
            }
            else if (currentHp > 0 && currentHp <= maxHp / 3)
            {
                finalKnockBackPower = knockBackPower * 1.8f;
            }
        }
        player.rigidbody.velocity = Vector3.zero;
        player.rigidbody.AddForce(knockBackDirection * finalKnockBackPower, ForceMode.Impulse);
    }

    public void RotateTowardsNearestPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, detectionRadius, playerLayer);
        if (hitColliders.Length > 0)
        {
            Transform nearestPlayer = null;
            float minDistance = Mathf.Infinity;

            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject == player.gameObject)
                    continue;

                float distance = Vector3.Distance(player.transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPlayer = collider.transform;
                }
            }

            if (nearestPlayer != null)
            {
                Vector3 direction = (nearestPlayer.position - player.transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    player.transform.rotation = lookRotation;
                }
            }
        }
    }


    public void HookFirstAttackFinish()
    {
        SetAttackValues((statController.defaultAttackDamage - 100) / 4, statController.defaultKnockBackPower, player.transform.up * 0.5f, HitType.Hit, false);
    }

    public void HookSecondAttackFinish()
    {  
        SetAttackValues((statController.defaultAttackDamage - 100) / 4 + 50, statController.heavyKnockBackPower, player.transform.up * 1.2f, HitType.HitUp, true);
    }

    public void HookHeavyAttackFinish()
    {
        SetAttackValues(statController.heavyAttackDamage - (statController.heavyAttackDamage /9 *4), statController.heavyKnockBackPower, Vector3.up * 1.2f, HitType.HitUp, true);
    }

}
