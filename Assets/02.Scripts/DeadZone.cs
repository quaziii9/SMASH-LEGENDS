using Mirror;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        var legend = other.GetComponent<PlayerController>();

        if (legend.StatController.currentHp > 0)
        {
            legend.StatController.currentHp = 0;
            legend.StatController.CmdUpdateHealthBar(legend.StatController.currentHp, legend.StatController.maxHp, legend.IsHost);
        }

        legend.EffectController.CmdSetDieEffect();

        legend.StatController.CmdSmash(legend.IsHost);

        if(legend.isLocalPlayer)
        {
            DuelUIController.Instance.LocalRespawnTimer().Forget();
        }
    }
}
