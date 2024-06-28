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

    private float heavyAttackCoolTime = 4f;
    public float currentHeavyAttackCoolTime = 0f;
    [SerializeField] private float _defaultAttackDamage = 600;
    [SerializeField] private float _heavyAttackDamage = 900;
    [SerializeField] private float _skillAttackDamage = 1500;

    [Header("Knockback")]
    private float _defaultAttackKnockBackPower = 0.2f;
    private float _heavyAttackKnockBackPower = 0.38f;

    [SyncVar] public float DamageAmount;
    [SyncVar] public float KnockBackPower = 1;
    [SyncVar] public Vector3 KnockBackDireciton;
    [SyncVar] public HitType hitType;

    public float jumpForce = 14.28f;
    public float gravityScale = 36f;
    public float maxFallSpeed = 20f;

    [Header("GroundCheck")]
    public float groundCheckDistance = 1f;

    [Header("Detection")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;

    private Vector3 _attackMoveDirection;
    public float _attackMoveDistance;
    public float _attackMoveDuration;
    public float _attackMoveStartTime;
    public float _currentMoveDistance;

    public void Initialize(PlayerController playerController)
    {
        player = playerController;
    }

    public void HandleAttack(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.FirstAttack:
                SetAttackValues(_defaultAttackDamage / 3, _defaultAttackKnockBackPower, player.transform.up * 0.5f, HitType.Hit);
                break;
            case PlayerState.SecondAttack:
                SetAttackValues(_defaultAttackDamage / 6, _defaultAttackKnockBackPower, player.transform.up * 0.5f, HitType.Hit);
                break;
            case PlayerState.FinishAttack:
                SetAttackValues(_defaultAttackDamage / 3, _heavyAttackKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case PlayerState.JumpAttack:
                SetAttackValues(_defaultAttackDamage * 0.6f, _heavyAttackKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case PlayerState.HeavyAttack:
                SetAttackValues(_heavyAttackDamage, _heavyAttackKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case PlayerState.JumpHeavyAttackLanding:
            case PlayerState.JumpHeavyAttack:
                SetAttackValues(_heavyAttackDamage / 3 * 2, _heavyAttackKnockBackPower, player.transform.up * 1.2f, HitType.HitUp);
                break;
            case PlayerState.SkillAttack:
                SetAttackValues((_skillAttackDamage - 500) / 5, _defaultAttackKnockBackPower, player.transform.up, HitType.Hit);
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
        SetAttackValues(_skillAttackDamage / 5 + 500, _heavyAttackKnockBackPower, player.transform.forward + player.transform.up * 1.2f, HitType.HitUp);
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
        currentHeavyAttackCoolTime = heavyAttackCoolTime;
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
