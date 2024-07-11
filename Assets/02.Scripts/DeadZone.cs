using Mirror;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        var legend = other.GetComponent<PlayerController>();

        if (legend != null)
        {
            legend.StatController.HandleDeadZone(legend.IsHost);
        }

        //if (legend.StatController.currentHp > 0)
        //{
        //    legend.StatController.currentHp = 0;
        //    legend.StatController.CmdUpdateHealthBar(legend.StatController.currentHp, legend.StatController.maxHp, legend.IsHost);
        //}

        //legend.EffectController.CmdSetDieEffect();

        //legend.StatController.CmdSmash(legend.IsHost);



        //DuelUIController.Instance.LocalRespawnTimer().Forget();
    }
}
