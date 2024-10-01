using UnityEngine;
using TMPro;

public class ResultUIManager : Singleton<ResultUIManager>
{
    public TextMeshProUGUI ResultText;
    public GameObject resultCamera;
    public GameObject resultPlayer;
    public GameObject ResultUI;



    [SerializeField] private GameObject[] bluePositionLegend;
    [SerializeField] private GameObject[] redPositionLegend;
    public bool IsResultUION = false;

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

    public void SetResult(bool IsHost, bool? WinHost, int HostLegend, int ClientLegend)
    {
        LobbyManager.Instance.LobbyUIDisable();
        resultCamera.SetActive(true);
        resultPlayer.SetActive(true);
        ResultUI.SetActive(true);
        IsResultUION = true;


        if (WinHost.HasValue)
        {
            // 호스트가 이길시 
            if (WinHost.Value)
            {
                // 내가 호스트면
                if (IsHost)
                {
                    ResultTextSet("승리");
                    bluePositionLegend[HostLegend].SetActive(true);
                    Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
                    bluePositionLgendAnim.SetTrigger("Win");

                    redPositionLegend[ClientLegend].SetActive(true);
                    Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
                    redPositionLegendAnim.SetTrigger("Lose");
                }
                // 내가 호스트가 아니면
                else
                {
                    ResultTextSet("패배");
                    bluePositionLegend[HostLegend].SetActive(true);
                    Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
                    bluePositionLgendAnim.SetTrigger("Win");

                    redPositionLegend[ClientLegend].SetActive(true);
                    Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
                    redPositionLegendAnim.SetTrigger("Lose");
                }
            }
            else
            {
                if (IsHost)
                {
                    ResultTextSet("패배");
                    bluePositionLegend[HostLegend].SetActive(true);
                    Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
                    bluePositionLgendAnim.SetTrigger("Lose");

                    redPositionLegend[ClientLegend].SetActive(true);
                    Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
                    redPositionLegendAnim.SetTrigger("Win");
                }
                else
                {
                    ResultTextSet("승리");
                    bluePositionLegend[HostLegend].SetActive(true);
                    Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
                    bluePositionLgendAnim.SetTrigger("Lose");

                    redPositionLegend[ClientLegend].SetActive(true);
                    Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
                    redPositionLegendAnim.SetTrigger("Win");
                }
            }
        }
        else
        {
            ResultTextSet("무승부");
            bluePositionLegend[HostLegend].SetActive(true);
            Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
            bluePositionLgendAnim.SetTrigger("Lobby");

            redPositionLegend[ClientLegend].SetActive(true);
            Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
            redPositionLegendAnim.SetTrigger("Lobby");
        }
    }

    public void ExitSetResult()
    {
        resultCamera.SetActive(false);
        resultPlayer.SetActive(false);
        ResultUI.SetActive(false);
        IsResultUION = false;
        LobbyManager.Instance.LobbyUIEnable();

        foreach (var legend in bluePositionLegend)
        {
            legend.SetActive(false);
        }
        foreach (var legend in redPositionLegend)
        {
            legend.SetActive(false);
        }
    }
}
