using Cysharp.Threading.Tasks;
using Mirror;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMatchingManager : NetworkBehaviour
{
    public static UIMatchingManager Instance;
    public GameObject Client;
    public GameObject Matching;
    public TextMeshProUGUI LoadingText;

    private void Awake()
    {
        Instance = this;
    }


    public void UpdatePlayerCount()
    {
        var players = FindObjectsOfType<RoomPlayer>();
        int playerCount = players.Length;
        

        if(playerCount == 2)
        {
            UpdatePlayerCountTest().Forget();
        }
    }

    public async UniTaskVoid UpdatePlayerCountTest()
    { 
        await UniTask.Delay(1500);
        Client.SetActive(true);
        Matching.SetActive(true);
        LoadingText.text = "아레나가 열리고 있습니다";
    }

    public void OnExitButtonClicked()
    {
        if (!NetworkServer.active)
        {
            Debug.Log("You are not the host. Exiting to lobby is not allowed for clients.");
            return; // 클라이언트는 메서드를 종료
        }

        // 호스트인 경우 ExitToLobby 메서드를 호출
        ExitToLobby();
    }

    private async void ExitToLobby()
    {
        if (NetworkServer.active)
        {
            NetworkManager.singleton.StopHost();
        }

        await UniTask.Yield(); // 서버/클라이언트 정지 후 잠시 대기

        // 오프라인 씬으로 전환
        SceneManager.LoadScene(NetworkManager.singleton.offlineScene);
    }

}