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

    [SerializeField] GameObject PeterSkin1Prefab;
    [SerializeField] GameObject PeterSkin2Prefab;
    [SerializeField] GameObject PeterSkin3Prefab;
    [SerializeField] GameObject HookSkin1Prefab;
    [SerializeField] GameObject HookSkin2Prefab;
    [SerializeField] GameObject HookSkin3Prefab;

    GameObject pref;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSpawnPlayer(UIManager.Instance.legendType, UIManager.Instance.legendSkinType);
    }

    [Command(requiresAuthority = false)]
    void CmdSpawnPlayer(LegendType legendType, int skinType)
    {
        NetworkConnectionToClient conn = connectionToClient;

        if (legendType == LegendType.Peter)
        {
            pref = GetPeterPrefab(skinType);
            GameManager.Instance.SetLegendType(conn == NetworkServer.localConnection, (int)LegendType.Peter);
        }
        else if (legendType == LegendType.Hook)
        {
            pref = GetHookPrefab(skinType);
            GameManager.Instance.SetLegendType(conn == NetworkServer.localConnection, (int)LegendType.Hook);
        }

        if (conn == NetworkServer.localConnection)
        {
            transform.position = startPosition1;
            transform.rotation = rotation1;
        }
        else
        {
            transform.position = startPosition2;
            transform.rotation = rotation2;
        }

        var obj = Instantiate(pref, transform.position, transform.rotation);
        NetworkServer.Spawn(obj);
        NetworkServer.ReplacePlayerForConnection(conn, obj);
    }

    GameObject GetPeterPrefab(int skinType)
    {
        switch (skinType)
        {
            case 1: return PeterSkin1Prefab;
            case 2: return PeterSkin2Prefab;
            case 3: return PeterSkin3Prefab;
            default: return PeterSkin1Prefab; // 기본값
        }
    }

    GameObject GetHookPrefab(int skinType)
    {
        switch (skinType)
        {
            case 1: return HookSkin1Prefab;
            case 2: return HookSkin2Prefab;
            case 3: return HookSkin3Prefab;
            default: return HookSkin1Prefab; // 기본값
        }
    }
}

