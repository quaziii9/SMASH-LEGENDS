using UnityEngine;
using TMPro;

public class ResultUIManager : Singleton<ResultUIManager>
{
    public TextMeshProUGUI ResultText;
    public GameObject resultCamera;
    public GameObject resultPlayer;
    public GameObject ResultUI;
    public bool IsReusltUION = false;

    public void ResultTextSet(string result)
    {
        if (ResultText != null)
        {
            ResultText.text = result;
        }
        else
        {
            Debug.LogError("ResultText is not assigned in the inspector.");
        }
    }


    public void SetResult(bool IsHost, bool? WinHost)
    {
        resultCamera.SetActive(true);
        resultPlayer.SetActive(true);
        ResultUI.SetActive(true);
        IsReusltUION = true;

        if (WinHost.HasValue)
        {
            if (WinHost.Value)
            {
                if (IsHost)
                {
                    ResultTextSet("승리");
                }
                else
                {
                    ResultTextSet("패배");
                }
            }
            else
            {
                if (IsHost)
                {
                    ResultTextSet("패배");
                }
                else
                {
                    ResultTextSet("승리");
                }
            }
        }
        else
        {
           ResultTextSet("무승부");
        }
    }

    public void ExitSetResult()
    {
        resultCamera.SetActive(false);
        resultPlayer.SetActive(false);
        ResultUI.SetActive(false);
        IsReusltUION = false;
        UIManager.Instance.LobbyUIEnable();
    }
}
