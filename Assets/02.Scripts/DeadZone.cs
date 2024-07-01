using Mirror;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        var legend = other.GetComponent<PlayerController>();

        NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
        NetworkConnectionToClient conn = networkIdentity.connectionToClient;
        bool isHost = conn != null && conn == NetworkServer.localConnection; // 서버 (호스트) 플레이어인지 확인

        if (legend.StatController.currentHp > 0)
        {
            legend.StatController.currentHp = 0;
        }
        legend.EffectController.SetDieEffect();
        legend.gameObject.SetActive(false);
    }
}
