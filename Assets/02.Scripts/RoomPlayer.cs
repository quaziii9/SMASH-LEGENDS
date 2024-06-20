using Mirror;

public class RoomPlayer : NetworkRoomPlayer
{
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
