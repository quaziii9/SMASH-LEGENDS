
using UnityEngine;


public class UIMatchingManager : Singleton<UIMatchingManager>
{
    public GameObject Client;
    public GameObject Matching;

    public void InClient()
    {
        Debug.Log("inclient");
        Client.SetActive(true);
        Matching.SetActive(true);
    }

    //[Command]
    //private void CmdSetClientActive(bool isActive)
    //{
    //    Debug.Log("inclientcommand");
    //    RpcSetClientActive(isActive);
    //}

    //[ClientRpc]
    //private void RpcSetClientActive(bool isActive)
    //{
    //    Debug.Log("clientactive");
    //    Client.SetActive(isActive);
    //    Matching.SetActive(isActive);
    //}
}