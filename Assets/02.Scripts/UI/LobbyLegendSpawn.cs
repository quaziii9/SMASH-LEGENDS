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

    // 전설과 스킨을 활성화하는 함수
    public void LegendSpawn(int legendType, int legendSkinType)
    {
        // 모든 전설과 스킨 비활성화
        DeactivateAllLegendsAndSkins();

        // legendType에 따라 첫 번째(피터) 또는 두 번째(후크) 전설 활성화
        Transform selectedLegend = transform.GetChild(legendType);
        selectedLegend.gameObject.SetActive(true);

        // legendSkinType에 따라 선택된 전설의 하위 스킨 활성화
        Transform selectedSkin = selectedLegend.GetChild(legendSkinType);
        selectedSkin.gameObject.SetActive(true);
    }

    // 모든 전설과 스킨을 비활성화하는 함수
    private void DeactivateAllLegendsAndSkins()
    {
        // 각 전설(Peter, Hook 등)의 하위 오브젝트들을 순회하며 모두 비활성화
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform legend = transform.GetChild(i);
            legend.gameObject.SetActive(false);

            // 각 전설의 하위 스킨들도 모두 비활성화
            for (int j = 0; j < legend.childCount; j++)
            {
                legend.GetChild(j).gameObject.SetActive(false);
            }
        }
    }
}
