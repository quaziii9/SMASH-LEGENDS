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
    }
}
