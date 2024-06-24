using UnityEngine;
using Mirror;

public class WeaponController : MonoBehaviour
{
    Collider weaponCollider;

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log(other.name);

            PlayerController player = other.GetComponent<PlayerController>();
            player.Hitted();

            weaponCollider.enabled = false;
        }
    }
}
