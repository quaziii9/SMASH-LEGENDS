using Mirror;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        var legend = other.GetComponent<PlayerController>();

        if (legend.StatController.currentHp > 0)
        {
            legend.StatController.currentHp = 0;
        }
        legend.EffectController.SetDieEffect();

        legend.StatController.CmdSmash(legend.IsHost);
    }
}
