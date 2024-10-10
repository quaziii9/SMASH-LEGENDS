using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventLibrary;
using EnumTypes;

public class LobbyLegendSpawn : MonoBehaviour
{
    private void Start()
    {
        // 이벤트 리스너 등록
        LegendSpawn((int)LobbyManager.Instance.legendType, LobbyManager.Instance.legendSkinType);
        EventManager<LobbyEvents>.StartListening<int, int>(LobbyEvents.LegendSpawn, LegendSpawn);
    }

    public void LegendSpawn(int legendType, int legendSkinType)
    {
        DeactivateAllLegendsAndSkins();

        Transform selectedLegend = transform.GetChild(legendType);
        selectedLegend.gameObject.SetActive(true);

        Transform selectedSkin = selectedLegend.GetChild(legendSkinType);
        selectedSkin.gameObject.SetActive(true);
    }

    private void DeactivateAllLegendsAndSkins()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform legend = transform.GetChild(i);
            legend.gameObject.SetActive(false);

            for (int j = 0; j < legend.childCount; j++)
            {
                legend.GetChild(j).gameObject.SetActive(false);
            }
        }
    }
}
