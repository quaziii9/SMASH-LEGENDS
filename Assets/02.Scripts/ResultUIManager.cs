using UnityEngine;
using TMPro;

public class ResultUIManager : Singleton<ResultUIManager>
{
    public TextMeshProUGUI ResultText;
    public GameObject resultCamera;
    public GameObject resultPlayer;
    public GameObject ResultUI;



    [SerializeField] private GameObject bluePositionLegend;
    [SerializeField] private GameObject redPositionLegend;
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

    public void SetResult(bool IsHost, bool? WinHost, int HostLegend, int HostLegendSkin, int ClientLegend, int ClientLegendSkin)
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
                    Animator bluePositionLgendAnim  = LegendSpawn(HostLegend, HostLegendSkin, bluePositionLegend);
                    bluePositionLgendAnim.SetTrigger("Win");

                    Animator redPositionLegendAnim = LegendSpawn(ClientLegend, ClientLegendSkin, redPositionLegend);
                    redPositionLegendAnim.SetTrigger("Lose");
                }
                // 내가 호스트가 아니면
                else
                {
                    ResultTextSet("패배");
                    Animator bluePositionLgendAnim = LegendSpawn(HostLegend, HostLegendSkin, bluePositionLegend);
                    bluePositionLgendAnim.SetTrigger("Win");

                    Animator redPositionLegendAnim = LegendSpawn(ClientLegend, ClientLegendSkin, redPositionLegend);
                    redPositionLegendAnim.SetTrigger("Lose");
                }
            }
            else
            {
                if (IsHost)
                {
                    ResultTextSet("패배");
                    Animator bluePositionLgendAnim = LegendSpawn(HostLegend, HostLegendSkin, bluePositionLegend);
                    bluePositionLgendAnim.SetTrigger("Lose");

                    Animator redPositionLegendAnim = LegendSpawn(ClientLegend, ClientLegendSkin, redPositionLegend);
                    redPositionLegendAnim.SetTrigger("Win");
                }
                else
                {
                    ResultTextSet("승리");
                    Animator bluePositionLgendAnim = LegendSpawn(HostLegend, HostLegendSkin, bluePositionLegend);
                    bluePositionLgendAnim.SetTrigger("Lose");

                    Animator redPositionLegendAnim = LegendSpawn(ClientLegend, ClientLegendSkin, redPositionLegend);
                    redPositionLegendAnim.SetTrigger("Win");
                }
            }
        }
        else
        {
            ResultTextSet("무승부");
            Animator bluePositionLgendAnim = LegendSpawn(HostLegend, HostLegendSkin, bluePositionLegend);
            bluePositionLgendAnim.SetTrigger("Lobby");

            Animator redPositionLegendAnim = LegendSpawn(ClientLegend, ClientLegendSkin, redPositionLegend);
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

        DeactivateAllLegendsAndSkins(bluePositionLegend);
        DeactivateAllLegendsAndSkins(redPositionLegend);
    }


    public Animator LegendSpawn(int legendType, int legendSkinType, GameObject TeamPosition)
    {
        // 모든 전설과 스킨 비활성화
        DeactivateAllLegendsAndSkins(TeamPosition);

        // legendType에 따라 첫 번째(피터) 또는 두 번째(후크) 전설 활성화
        Transform selectedLegend = TeamPosition.transform.GetChild(legendType);
        selectedLegend.gameObject.SetActive(true);

        // legendSkinType에 따라 선택된 전설의 하위 스킨 활성화
        Transform selectedSkin = selectedLegend.GetChild(legendSkinType);
        selectedSkin.gameObject.SetActive(true);
        return selectedSkin.GetComponent<Animator>();
    }

    // 모든 전설과 스킨을 비활성화하는 함수
    private void DeactivateAllLegendsAndSkins(GameObject TeamPosition)
    {
        // 각 전설(Peter, Hook 등)의 하위 오브젝트들을 순회하며 모두 비활성화
        for (int i = 0; i < TeamPosition.transform.childCount; i++)
        {
            Transform legend = TeamPosition.transform.GetChild(i);
            legend.gameObject.SetActive(false);

            // 각 전설의 하위 스킨들도 모두 비활성화
            for (int j = 0; j < legend.childCount; j++)
            {
                legend.GetChild(j).gameObject.SetActive(false);
            }
        }
    }
}
