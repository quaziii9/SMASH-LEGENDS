using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
    private Vector3 startPosition1 = new Vector3(-20, 1.5f, 0); // 원하는 위치로 설정
    private Vector3 startPosition2 = new Vector3(20, 1.5f, 0); // 원하는 위치로 설정
    private Quaternion rotation1 = Quaternion.Euler(new Vector3(0, 90, 0));
    private Quaternion rotation2 = Quaternion.Euler(new Vector3(0, -90, 0));

    [SerializeField] GameObject Peter1;
    [SerializeField] GameObject Peter2;

    GameObject pref;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSpawnPlayer();
    }

    [Command(requiresAuthority = false)]
    void CmdSpawnPlayer()
    {
        NetworkConnectionToClient conn = connectionToClient;

        if (conn == NetworkServer.localConnection)
        {
            pref = Peter1;
            transform.position = startPosition1;
            transform.rotation = rotation1;
        }
        else
        {
            pref = Peter2;
            transform.position = startPosition2;
            transform.rotation = rotation2;
        }

        var obj = Instantiate(pref, transform.position, transform.rotation);
        NetworkServer.Spawn(obj);
        NetworkServer.ReplacePlayerForConnection(conn, obj);
    }
}
