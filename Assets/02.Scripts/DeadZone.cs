using Mirror;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        var legend = other.GetComponent<PlayerController>();

        Debug.Log(other.name);

        if (legend.StatController.currentHp > 0)
        {
            legend.StatController.currentHp = 0;
            legend.StatController.CmdUpdateHealthBar(legend.StatController.currentHp, legend.StatController.maxHp, legend.IsHost);
        }

        legend.EffectController.SetDieEffect();

        legend.StatController.CmdSmash(legend.IsHost);

        if(legend.isLocalPlayer)
        {
            DuelManager.Instance.LocalRespawnTimer().Forget();
        }
    }
}
