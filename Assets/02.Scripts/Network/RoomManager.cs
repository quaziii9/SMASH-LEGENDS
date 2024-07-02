using Mirror;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class RoomManager : NetworkRoomManager
{

    [SerializeField] GameObject RoomPlayer;

    private Vector3 startPosition1 = new Vector3(-20, 1.5f, 0); // 원하는 위치로 설정
    private Vector3 startPosition2 = new Vector3(20, 1.5f, 0); // 원하는 위치로 설정
    private Quaternion rotation1 = Quaternion.Euler(new Vector3(0, 90, 0));
    private Quaternion rotation2 = Quaternion.Euler(new Vector3(0, -90, 0));

    public static RoomManager Instance;
    private bool isSceneChanging = false;  // 씬 전환 상태를 추적하는 플래그
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        maxConnections = 2;
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
    }

    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        GameObject roomObj;

        if (conn.connectionId == 0)
        {
            roomObj = Instantiate(RoomPlayer, startPosition1, rotation1);
        }
        else
        {
            roomObj = Instantiate(RoomPlayer, startPosition2, rotation2);
        }

        return roomObj;
    }


    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        WaitToStartGame().Forget();
    }

    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnRoomServerAddPlayer(conn);
    }

    public override void OnRoomServerPlayersReady()
    {
        if (allPlayersReady)
        {
            ServerChangeScene(GameplayScene);
        }
    }

    private async UniTaskVoid WaitToStartGame()
    {
        if (numPlayers < maxConnections)
        {
            // 모든 플레이어가 연결될 때까지 대기
            while (numPlayers < maxConnections)
            {
                await UniTask.Yield();
            }
        }

        await UniTask.Delay(1500);

        // 플레이어 수가 최대 연결 수에 도달하면 게임 씬으로 전환
        if (!isSceneChanging && numPlayers == maxConnections)
        {
            isSceneChanging = true;  // 씬 전환 시작

            ServerChangeScene(GameplayScene);
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        NetworkClient.Ready();
    }
}
