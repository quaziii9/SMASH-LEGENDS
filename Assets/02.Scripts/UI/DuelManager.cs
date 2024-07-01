using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelManager : Singleton<DuelManager>
{
    public static DuelManager Instance { get; private set; }

    public Image hostHealthBar;
    public Image clientHealthBar;

    public GameObject []hostScoreBar;
    public GameObject []clientScoreBar;

    public GameObject hostRespawnTimer;
    public GameObject clientRespawnTimer;

    public GameObject RespawnTimer;

    public int hostScore = 0;
    public int clientScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 UIManager가 파괴되지 않도록 함
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스가 생기지 않도록 함
        }
    }

    public void UpdateHealthBar(int currentHp, int maxHp, bool isHost)
    {
        float fillAmount = (float)currentHp / maxHp;

        if (isHost)
        {
            hostHealthBar.fillAmount = fillAmount;
        }
        else
        {
            clientHealthBar.fillAmount = fillAmount;
        }
    }

    public void UpdateScore(bool isHost)
    {
        if(isHost == true)
        {
            clientScore++;
            switch (clientScore)
            {
                case 1:
                    clientScoreBar[0].SetActive(true);
                    break;
                case 2:
                    clientScoreBar[1].SetActive(true);
                    break;
                case 3:
                    clientScoreBar[2].SetActive(true);
                    break;
                default:
                    break;
            }
        }
        else
        {
            hostScore++;
            switch (hostScore)
            {
                case 1:
                    hostScoreBar[0].SetActive(true);
                    break;
                case 2:
                    hostScoreBar[1].SetActive(true);
                    break;
                case 3:
                    hostScoreBar[2].SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}
