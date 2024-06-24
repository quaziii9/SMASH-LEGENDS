using UnityEngine;
using Mirror;

public class WeaponController : MonoBehaviour
{
    Collider weaponCollider;
    PlayerController player;

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        player = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log(player.DamageAmount);
            PlayerController otherplayer = other.GetComponent<PlayerController>();
            otherplayer.Hitted(player.DamageAmount);

            weaponCollider.enabled = false;
        }
    }
}
