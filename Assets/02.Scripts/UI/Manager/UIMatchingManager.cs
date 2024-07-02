using Cysharp.Threading.Tasks;
using Mirror;
using System.Linq;
using TMPro;
using UnityEngine;

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
        Debug.Log(playerCount);

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
}