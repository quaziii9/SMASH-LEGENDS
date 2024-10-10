using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventLibrary;
using EnumTypes;

public class LobbyLegendSpawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> legends = new List<GameObject>(); 
    [SerializeField] private List<List<GameObject>> legendSkins = new List<List<GameObject>>(); 

    private void Start()
    {
        PopulateLegendAndSkinLists();

        LegendSpawn((int)LobbyManager.Instance.legendType, LobbyManager.Instance.legendSkinType);
        EventManager<LobbyEvents>.StartListening<int, int>(LobbyEvents.LegendSpawn, LegendSpawn);
    }

    private void PopulateLegendAndSkinLists()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform legend = transform.GetChild(i);
            legends.Add(legend.gameObject);

            List<GameObject> skins = new List<GameObject>();
            for (int j = 0; j < legend.childCount; j++)
            {
                skins.Add(legend.GetChild(j).gameObject); // 각 스킨을 리스트에 추가
            }
            legendSkins.Add(skins);
        }
    }

    public void LegendSpawn(int legendType, int legendSkinType)
    {
        DeactivateAllLegendsAndSkins();

        if (legendType >= 0 && legendType < legends.Count)
        {
            legends[legendType].SetActive(true);

            if (legendSkinType >= 0 && legendSkinType < legendSkins[legendType].Count)
            {
                legendSkins[legendType][legendSkinType].SetActive(true);
            }
        }
    }

    private void DeactivateAllLegendsAndSkins()
    {
        for (int i = 0; i < legends.Count; i++)
        {
            legends[i].SetActive(false); 

            for (int j = 0; j < legendSkins[i].Count; j++)
            {
                legendSkins[i][j].SetActive(false);
            }
        }
    }
}