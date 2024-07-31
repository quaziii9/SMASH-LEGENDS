using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
    private Vector3 startPosition1 = new Vector3(-20, 1.5f, 0); // 호스트 위치
    private Vector3 startPosition2 = new Vector3(20, 1.5f, 0); // 클라이언트 위치
    private Quaternion rotation1 = Quaternion.Euler(new Vector3(0, 90, 0));
    private Quaternion rotation2 = Quaternion.Euler(new Vector3(0, -90, 0));

    [SerializeField] GameObject PeterPrefab;
    [SerializeField] GameObject HookPrefab;

    GameObject pref;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (UIManager.Instance == null)
        {
            Debug.LogError("UIManager instance is null.");
            return;
        }

        CmdSpawnPlayer(UIManager.Instance.legendType);
    }

    [Command(requiresAuthority = false)]
    void CmdSpawnPlayer(UIManager.LegendType legendType)
    {
        NetworkConnectionToClient conn = connectionToClient;

        if (PeterPrefab == null || HookPrefab == null)
        {
            Debug.LogError("Prefabs are not assigned.");
            return;
        }

        SetPlayerPositionAndRotation(conn == NetworkServer.localConnection, legendType);

        if (pref != null)
        {
            var obj = Instantiate(pref, transform.position, transform.rotation);
            NetworkServer.Spawn(obj);
            NetworkServer.ReplacePlayerForConnection(conn, obj);
        }
    }

    private void SetPlayerPositionAndRotation(bool isHost, UIManager.LegendType legendType)
    {
        if (legendType == UIManager.LegendType.Peter)
        {
            pref = PeterPrefab;
            GameManager.Instance?.SetLegendType(isHost, (int)UIManager.LegendType.Peter);
        }
        else
        {
            pref = HookPrefab;
            GameManager.Instance?.SetLegendType(isHost, (int)UIManager.LegendType.Hook);
        }

        if (isHost)
        {
            transform.position = startPosition1;
            transform.rotation = rotation1;
        }
        else
        {
            transform.position = startPosition2;
            transform.rotation = rotation2;
        }
    }
}
