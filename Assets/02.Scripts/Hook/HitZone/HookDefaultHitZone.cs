using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDefaultHitZone : HookHitZone
{
    public void Start()
    {
        AttackController.SetAttackValues((StatController.defaultAttackDamage - 100) / 4, StatController.defaultKnockBackPower, PlayerController.transform.up * 0.5f, HitType.Hit, false);
    }  
}
