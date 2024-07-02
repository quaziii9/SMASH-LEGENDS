using Cysharp.Threading.Tasks;
using Mirror;
using System.Linq;
using UnityEngine;

public class UIMatchingManager : NetworkBehaviour
{
    public static UIMatchingManager Instance;
    public GameObject Client;
    public GameObject Matching;

    private void Awake()
    {
        Instance = this;
    }

    public void InClient()
    {
        Debug.Log("inclient");
        Client.SetActive(true);
        Matching.SetActive(true);
    }

    public void Test()
    {
        Matching.SetActive(false);
        Client.SetActive(false);
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
        await UniTask.Delay(500);

        Client.SetActive(true);
        Matching.SetActive(true);
    }
    //[Command]
    //private void CmdSetClientActive(bool isActive)
    //{
    //    Debug.Log("inclientcommand");
    //    RpcSetClientActive(isActive);
    //}

    //[ClientRpc]
    //private void RpcSetClientActive(bool isActive)
    //{
    //    Debug.Log("clientactive");
    //    Client.SetActive(isActive);
    //    Matching.SetActive(isActive);
    //}
}