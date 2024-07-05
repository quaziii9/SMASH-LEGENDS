using UnityEngine;
using TMPro;

public class ResultUIManager : Singleton<ResultUIManager>
{
    public TextMeshProUGUI ResultText;
    public GameObject resultCamera;
    public GameObject resultPlayer;
    public GameObject ResultUI;

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

        if (WinHost.HasValue)
        {
            if (WinHost.Value)
            {
                if (IsHost)
                {
                    ResultTextSet("승리");
                    Debug.Log("You (Host) win!");
                }
                else
                {
                    ResultTextSet("패배");
                    Debug.Log("Host wins!");
                }
            }
            else
            {
                if (IsHost)
                {
                    ResultTextSet("패배");
                    Debug.Log("Client wins!");
                }
                else
                {
                    ResultTextSet("승리");
                    Debug.Log("You (Client) win!");
                }
            }
        }
        else
        {
           Debug.Log("It's a draw!");
           ResultTextSet("무승부");
        }
    }

    public void ExitSetResult()
    {
        resultCamera.SetActive(false);
        resultPlayer.SetActive(false);
        ResultUI.SetActive(false);

        UIManager.Instance.LobbyUIEnable();
    }
}
