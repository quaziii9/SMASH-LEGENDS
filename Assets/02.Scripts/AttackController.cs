using UnityEngine;
using Mirror;

public enum HitType
{
    Hit,
    HitUp,
}

public class AttackController : NetworkBehaviour
{
    private PlayerController player;
    private StatController statController; // StatController 추가

    public float currentHeavyAttackCoolTime = 0f;

    [SyncVar] public float DamageAmount;
    [SyncVar] public float KnockBackPower = 1;
    [SyncVar] public Vector3 KnockBackDireciton;
    [SyncVar] public HitType hitType;

    [Header("Detection")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;

    public float _attackMoveDistance;
    public float _attackMoveDuration;
    private float _attackMoveStartTime;
    private float _currentMoveDistance;
    private Vector3 _attackMoveDirection;

    public void Initialize(PlayerController playerController, StatController statCtrl)
    {
        player = playerController;
        statController = statCtrl; // StatController 초기화
    }

    public void HandleAttack(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.FirstAttack:
                SetAttackValues(statController.defaultAttackDamage / 3, statController.defaultKnockBackPower, player.transform.up * 0.5f, HitType.Hit);
                break;
            case PlayerState.SecondAttack:
                SetAttackValues(statController.defaultAttackDamage / 6, statController.defaultKnockBackPower, player.transform.up * 0.5f, HitType.Hit);
                break;
            case PlayerState.FinishAttack:
                SetAttackValues(statController.defaultAttackDamage / 3, statController.heavyKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case PlayerState.JumpAttack:
                SetAttackValues(statController.defaultAttackDamage * 0.6f, statController.heavyKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case PlayerState.HeavyAttack:
                SetAttackValues(statController.heavyAttackDamage, statController.heavyKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case PlayerState.JumpHeavyAttackLanding:
            case PlayerState.JumpHeavyAttack:
                SetAttackValues(statController.heavyAttackDamage / 3 * 2, statController.heavyKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case PlayerState.SkillAttack:
                SetAttackValues((statController.skillAttackDamage - 500) / 5, statController.defaultKnockBackPower, player.transform.up, HitType.Hit);
                break;
        }
    }

    private void SetAttackValues(float damage, float knockBackPower, Vector3 knockBackDirection, HitType hitType)
    {
        DamageAmount = damage;
        KnockBackPower = knockBackPower;
        KnockBackDireciton = knockBackDirection;
        this.hitType = hitType;
    }

    public void SkillLastAttackDamage()
    {
        SetAttackValues(statController.skillAttackDamage / 5 + 500, statController.heavyKnockBackPower, player.transform.forward + player.transform.up * 1.2f, HitType.HitUp);
    }

    public void UpdateCooldowns()
    {
        if (currentHeavyAttackCoolTime > 0)
        {
            currentHeavyAttackCoolTime -= Time.deltaTime;
        }
    }

    public void StartHeavyAttackCooldown()
    {
        currentHeavyAttackCoolTime = statController.heavyAttackCoolTime;
    }

    public void HandleAttackMove()
    {
        float elapsedTime = Time.time - _attackMoveStartTime;
        float fraction = elapsedTime / _attackMoveDuration;
        float distanceToMove = Mathf.Lerp(0, _attackMoveDistance, fraction);

        Vector3 forwardMovement = _attackMoveDirection * (distanceToMove - _currentMoveDistance);
        player._rigidbody.MovePosition(player._rigidbody.position + forwardMovement);

        _currentMoveDistance = distanceToMove;

        if (player.CanLook && player._moveDirection != Vector3.zero)
        {
            player.LookAt();
        }
    }

    public void StartAttackMove()
    {
        if (!player.isLocalPlayer) return;
        if (player._curState != PlayerState.RollUpBack && player._curState != PlayerState.RollUpFront)
            RotateTowardsNearestPlayer();

        if (player._curState == PlayerState.SkillAttack)
        {
            _attackMoveDistance = 8f;
            _attackMoveDuration = 1.2f;
        }
        _attackMoveStartTime = Time.time;
        _currentMoveDistance = 0;
        if (player._curState == PlayerState.RollUpBack) _attackMoveDirection = -player.transform.forward;
        else
            _attackMoveDirection = player.transform.forward;
    }

    public void PlayerGetKnockBack(float knockBackPower, Vector3 knockBackDirection, HitType hitType)
    {
        switch (hitType)
        {
            case HitType.Hit:
                player.ChangeState(PlayerState.Hit);
                break;
            case HitType.HitUp:
                player.ChangeState(PlayerState.HitUp);
                break;
        }
        player._rigidbody.velocity = Vector3.zero;
        player._rigidbody.AddForce(knockBackDirection * knockBackPower, ForceMode.Impulse);
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
}
