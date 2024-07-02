using UnityEngine;
using Mirror;
using System.Threading.Tasks;

public class StartButton : MonoBehaviour
{
    public RoomManager roomManager;
    public void OnStartButtonClicked()
    {
        TryStartClient();
    }

    private async void TryStartClient()
    {
        try
        {
            roomManager.StartClient();
            //UIManager.Instance.MachintPopupEnable();
            await Task.Delay(500);  // 연결 시도 후 대기 시간 설정

            if (!NetworkClient.isConnected)
            {
                Debug.Log("Client connection failed, starting host.");
                roomManager.StartHost();
            }
            else
            {
                Debug.Log("Client connected successfully.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception occurred: {ex.Message}");
            roomManager.StartHost();
            
        }
    }
}
