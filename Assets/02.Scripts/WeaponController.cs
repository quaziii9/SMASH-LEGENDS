using UnityEngine;
using Mirror;

public class WeaponController : NetworkBehaviour
{
    Collider weaponCollider;
    PlayerController Player;

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        Player = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(Player.DamageAmount);
            PlayerController otherplayer = other.GetComponent<PlayerController>();
            otherplayer.Hitted(Player.DamageAmount);

            weaponCollider.enabled = false;
        }
    }


    //public void GetPlayerDamageAmount()
    //{
    //    switch(Player._curState.ToString())
    //    {
    //        case nameof(FirstAttackState):
    //        case nameof(FinishAttackState):
    //            Player.DamageAmount = Player._defaultAttackDamage / 3;
    //            break;
    //        case nameof(SecondAttackState):
    //            Player.DamageAmount = Player._defaultAttackDamage / 6;
    //            break;
    //        case nameof(HeavyAttackState):
    //            Player.DamageAmount = Player._heavyAttackDamage;
    //            break;
    //        case nameof(JumpHeavyAttackState):
    //            Player.DamageAmount = Player._heavyAttackDamage / 3 * 2;
    //            break;
    //        case nameof(SkillAttackState):
    //            Player.DamageAmount = (Player._skillAttackDamage - 500) / 5;
    //            break;
    //    }

    //}
}
