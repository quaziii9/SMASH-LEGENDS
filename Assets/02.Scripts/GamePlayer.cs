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

     [SerializeField] GameObject PeterPrefab;
    [SerializeField] GameObject HookPrefab;

    GameObject pref;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSpawnPlayer(UIManager.Instance.legendType);
    }

    [Command(requiresAuthority = false)]
    void CmdSpawnPlayer(UIManager.LegendType legendType)
    {
        NetworkConnectionToClient conn = connectionToClient;

        if (conn == NetworkServer.localConnection)
        {
            if (legendType == UIManager.LegendType.Peter)
            {
                pref = PeterPrefab;
                GameManager.Instance.SetLegendType(true, (int)UIManager.LegendType.Peter);
            }
            else
            {
                pref = HookPrefab;
                GameManager.Instance.SetLegendType(true, (int)UIManager.LegendType.Hook);

            }
            transform.position = startPosition1;
            transform.rotation = rotation1;
        }
        else
        {
            if (legendType == UIManager.LegendType.Peter)
            {
                pref = PeterPrefab;
                GameManager.Instance.SetLegendType(false, (int)UIManager.LegendType.Peter);

            }
            else
            {
                pref = HookPrefab;
                GameManager.Instance.SetLegendType(false, (int)UIManager.LegendType.Hook);
            }
            transform.position = startPosition2;
            transform.rotation = rotation2;
        }

        var obj = Instantiate(pref, transform.position, transform.rotation);
        NetworkServer.Spawn(obj);
        NetworkServer.ReplacePlayerForConnection(conn, obj);
    }
}
