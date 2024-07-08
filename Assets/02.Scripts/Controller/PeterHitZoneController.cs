using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeterHitZoneController : MonoBehaviour
{
    [SerializeField] private GameObject _attackHitZone;
    [SerializeField] private GameObject _secondAttackHitZone;
    [SerializeField] private GameObject _heavyAttackHitZone;
    [SerializeField] private GameObject _heavyJumpAttackHitZone;
    [SerializeField] private GameObject _jumpAttackHitZone;
    [SerializeField] private GameObject _skillAttackHitZone;

    [SerializeField] private Collider _heavyJumpAttackHitZoneCollider;


    private void Awake()
    {
        _heavyJumpAttackHitZoneCollider = _heavyJumpAttackHitZone.GetComponent<Collider>();
    }


    private void EnableAttackHitZone() => _attackHitZone.SetActive(true);
    private void DisableAttackHitZone() => _attackHitZone.SetActive(false);

    private void EnableSecondAttackHitZone() => _secondAttackHitZone.SetActive(true);
    private void DisableSecondAttackHitZone() => _secondAttackHitZone.SetActive(false);

    private void EnableHeavyAttackHitZone() => _heavyAttackHitZone.SetActive(true);
    private void DisableHeavyAttackHitZone() => _heavyAttackHitZone.SetActive(false);


    private void EnableHeavyJumpAttackHitZone()
    {
        _heavyJumpAttackHitZone.SetActive(true);
        _heavyJumpAttackHitZoneCollider.enabled = true;

    }
    private void DisableHeavyJumpAttackHitZone()
    {
        _heavyJumpAttackHitZone.SetActive(false);
        _heavyJumpAttackHitZoneCollider.enabled = false;
    }


    private void EnableJumpAttackHitZone() => _jumpAttackHitZone.SetActive(true);
    private void DisableJumpAttackHitZone() => _jumpAttackHitZone.SetActive(false);

    private void EnableSkillAttackHitZone() => _skillAttackHitZone.SetActive(true);
    private void DisableSkillAttackHitZone() => _skillAttackHitZone.SetActive(false);
}
