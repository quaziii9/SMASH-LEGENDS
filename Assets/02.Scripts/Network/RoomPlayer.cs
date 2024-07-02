using Mirror;

public class RoomPlayer : NetworkRoomPlayer
{
    private void Start()
    {
        LoadingUIManager.Instance.MatchingManager.UpdatePlayerCount();
    }
    private void OnDestroy()
    {
        if (LoadingUIManager.Instance != null)
        {
            LoadingUIManager.Instance.MatchingManager.UpdatePlayerCount();
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
