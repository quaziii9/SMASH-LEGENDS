using Mirror;

public class RoomPlayer : NetworkRoomPlayer
{
    private void Start()
    {
        UIMatchingManager.Instance.UpdatePlayerCount();
        UIManager.Instance.LobbyUIDisable();

    }
    private void OnDestroy()
    {
        if (UIMatchingManager.Instance != null)
        {
            UIMatchingManager.Instance.UpdatePlayerCount();
        }
    }

    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
    }

    public override void OnClientExitRoom()
    {
        base.OnClientExitRoom();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        DontDestroyOnLoad(gameObject);
    }

}
