using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class StartButton : MonoBehaviour
{
    public RoomManager roomManager;
    public void OnStartButtonClicked()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            roomManager.StartHost();
        }
        else if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
            {
                NetworkClient.AddPlayer();
            }
        }
    }
}