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
            Debug.Log(Player.KnockBackPower);
            PlayerController otherplayer = other.GetComponent<PlayerController>();
            otherplayer.Hitted(Player.DamageAmount, Player.KnockBackPower, Player.transform.position, Player.KnockBackDireciton, Player.hitType); // 수정된 부분

            weaponCollider.enabled = false;
        }
    }



    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log(Player.DamageAmount);
    //        Debug.Log(Player.KnockBackPower);
    //        PlayerController otherplayer = other.GetComponent<PlayerController>();
    //        otherplayer.Hitted(Player.DamageAmount, Player.KnockBackPower, Player.KnockBackDireciton, Player.hitType, Player.transform.position);

    //        weaponCollider.enabled = false;
    //    }
    //}
}
