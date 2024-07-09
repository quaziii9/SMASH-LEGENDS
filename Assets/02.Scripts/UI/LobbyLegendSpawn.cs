using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventLibrary;
using EnumTypes;

public class LobbyLegendSpawn : MonoBehaviour
{
    public GameObject[] Legend;

    private void Start()
    {
        EventManager<LobbyEvents>.StartListening<int>(LobbyEvents.LegendSpawn, LegendSpawn);
    }

    public void LegendSpawn(int arr)
    {
        switch (arr)
        {
            case 0:
                Legend[0].SetActive(true);
                Legend[1].SetActive(false);
                break;
            case 1:
                Legend[0].SetActive(false);
                Legend[1].SetActive(true);
                break;
        }
    }
}
