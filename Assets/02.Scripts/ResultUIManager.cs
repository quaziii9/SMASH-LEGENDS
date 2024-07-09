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
        resultCamera.SetActive(true);
        resultPlayer.SetActive(true);
        ResultUI.SetActive(true);
        IsResultUION = true;

        Debug.Log(IsHost);
        Debug.Log(HostLegend);
        Debug.Log(ClientLegend);

        if (WinHost.HasValue)
        {
            if (WinHost.Value)
            {
                if (IsHost)
                {
                    ResultTextSet("승리");
                    bluePositionLegend[HostLegend].SetActive(true);
                    //Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
                    //bluePositionLgendAnim.SetBool("Win", true);

                    redPositionLegend[ClientLegend].SetActive(true);
                    //Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
                    //redPositionLegendAnim.SetBool("Lose", true);
                }
                else
                {
                    ResultTextSet("패배");
                    bluePositionLegend[HostLegend].SetActive(true);
                    //Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
                    //bluePositionLgendAnim.SetBool("Lose", true);

                    redPositionLegend[ClientLegend].SetActive(true);
                    //Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
                    //redPositionLegendAnim.SetBool("Win", true);
                }
            }
            else
            {
                if (IsHost)
                {
                    ResultTextSet("패배");
                    bluePositionLegend[HostLegend].SetActive(true);
                    //Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
                    //bluePositionLgendAnim.SetBool("Lose", true);

                    redPositionLegend[ClientLegend].SetActive(true);
                    //Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
                    //redPositionLegendAnim.SetBool("Win", true);
                }
                else
                {
                    ResultTextSet("승리");
                    bluePositionLegend[HostLegend].SetActive(true);
                   // Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
                   // bluePositionLgendAnim.SetBool("Win", true);

                    redPositionLegend[ClientLegend].SetActive(true);
                    //Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
                   // redPositionLegendAnim.SetBool("Lose", true);
                }
            }
        }
        else
        {
            ResultTextSet("무승부");
            bluePositionLegend[HostLegend].SetActive(true);
            //Animator bluePositionLgendAnim = bluePositionLegend[HostLegend].GetComponent<Animator>();
            //bluePositionLgendAnim.SetBool("Lobby", true);

            redPositionLegend[ClientLegend].SetActive(true);
            //Animator redPositionLegendAnim = redPositionLegend[ClientLegend].GetComponent<Animator>();
            //redPositionLegendAnim.SetBool("Lobby", true);
        }
    }

    public void ExitSetResult()
    {
        resultCamera.SetActive(false);
        resultPlayer.SetActive(false);
        ResultUI.SetActive(false);
        IsResultUION = false;
        UIManager.Instance.LobbyUIEnable();

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
