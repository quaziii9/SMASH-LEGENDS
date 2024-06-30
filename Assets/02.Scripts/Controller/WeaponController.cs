using UnityEngine;
using Mirror;

public class WeaponController : NetworkBehaviour
{
    Collider weaponCollider;
    AttackController attackController;

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        attackController = GetComponentInParent<AttackController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StatController otherplayer = other.GetComponent<StatController>();
            StateController otherplayerState = other.GetComponent<StateController>();

            if (otherplayerState.IsInvincible == true) return;

            NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
            NetworkConnectionToClient conn = networkIdentity.connectionToClient;
            bool isHost = conn != null && conn == NetworkServer.localConnection; // 서버 (호스트) 플레이어인지 확인


            otherplayer.Hitted(
                attackController.DamageAmount, 
                attackController.KnockBackPower, 
                attackController.transform.position, 
                attackController.KnockBackDireciton, 
                attackController.hitType, 
                attackController.PlusAddForce,
                isHost);

            weaponCollider.enabled = false;
        }
    }
}
