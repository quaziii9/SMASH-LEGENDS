using Mirror;
using UnityEngine;
using System.Collections;

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
    }

    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnRoomServerAddPlayer(conn);

        if (roomSlots.Count == maxConnections)
        {
            StartCoroutine(WaitToStartGame());
        }
    }

    public override void OnRoomServerPlayersReady()
    {
        if (allPlayersReady)
        {
            StartCoroutine(WaitToStartGame());
        }
    }

    private IEnumerator WaitToStartGame()
    {
        yield return new WaitForSeconds(3);
        ServerChangeScene(GameplayScene);
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

        if (connections.Length > 0)
        {
            NetworkConnectionToClient hostConnection = connections[0];
            if (hostConnection.identity != null)
            {
                hostConnection.identity.transform.position = startPosition1.position;
            }
        }

        if (connections.Length > 1)
        {
            NetworkConnectionToClient clientConnection = connections[1];
            if (clientConnection.identity != null)
            {
                clientConnection.identity.transform.position = startPosition2.position;
            }
        }
    }
}

