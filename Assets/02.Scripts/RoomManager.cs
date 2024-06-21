using Mirror;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public class RoomManager : NetworkRoomManager
{
    public static RoomManager Instance;

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
        Debug.Log("Host started");
    }

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);
        Debug.Log("Player connected: " + conn.connectionId);

       // WaitToStartGame().Forget();
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
        await UniTask.Delay(1500); // 1.5초 대기
        if (numPlayers == maxConnections)
        {
            ServerChangeScene(GameplayScene);
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        NetworkClient.Ready();

        if (sceneName == GameplayScene)
        {
            PositionPlayers();
        }
    }

    private void PositionPlayers()
    {
        NetworkStartPosition[] startPositions = FindObjectsOfType<NetworkStartPosition>();
        Transform startPosition1 = null;
        Transform startPosition2 = null;

        foreach (NetworkStartPosition pos in startPositions)
        {
            if (pos.name == "NetworkStartPosition1")
            {
                startPosition1 = pos.transform;
            }
            else if (pos.name == "NetworkStartPosition2")
            {
                startPosition2 = pos.transform;
            }
        }

        if (startPosition1 == null || startPosition2 == null)
        {
            Debug.LogError("Start positions not found in the scene.");
            return;
        }

        NetworkConnectionToClient[] connections = new NetworkConnectionToClient[NetworkServer.connections.Values.Count];
        NetworkServer.connections.Values.CopyTo(connections, 0);

        Debug.Log(connections.Length);
        if (connections.Length == 1)
        {
            NetworkConnectionToClient hostConnection = connections[0];
            if (hostConnection.identity != null)
            {
                hostConnection.identity.transform.position = startPosition1.position;
            }
        }
        Debug.Log(connections.Length);
        if (connections.Length == 2)
        {
            //NetworkConnectionToClient clientConnection = connections[1];
            //if (clientConnection.identity != null)
            //{
                connections[0].identity.transform.position = startPosition1.position;
                connections[1].identity.transform.position = startPosition2.position;
            //}
        }
    }
}

