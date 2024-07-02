using Cysharp.Threading.Tasks;
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

    public int RespawnTime = 5;
    public int hostRespawnTime;
    public int clientRespawnTime;

    public Image HeavyAttackIconeBar;
    public Image SkillAttackIconeBar;
    public GameObject DefualtAttackIcon;

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

    public void Start()
    {
        StartActive().Forget();
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

    public void StartRespawnTimer(bool isHost)
    {
        if(isHost == true)
        {
            hostRespawnTimer.SetActive(true);
            HostRespawnCountdown().Forget();
        }
        else
        {
            clientRespawnTimer.SetActive(true);
            ClientRespawnCountdown().Forget();
  
        }
    }

    private async UniTaskVoid StartActive()
    {
        await UniTask.Delay(200);
        hostRespawnTimer.SetActive(false);
        clientRespawnTimer.SetActive(false);
        RespawnTimer.SetActive(false);
    }

    private async UniTaskVoid ClientRespawnCountdown()
    {
        await UniTask.Delay(5000); // 5초 대기
        clientRespawnTimer.SetActive(false);
    }

    private async UniTaskVoid HostRespawnCountdown()
    {
        await UniTask.Delay(5000); // 5초 대기
        hostRespawnTimer.SetActive(false);
    }

    public async UniTaskVoid LocalRespawnTimer()
    {
        RespawnTimer.SetActive(true);
        await UniTask.Delay(5000); // 5초 대기
        RespawnTimer.SetActive(false);
    }

    public void UpdateHeavyAttackIconeCoolTime(float currentCoolTime, float maxCoolTime)
    {
        float fillAmount = 1 - (currentCoolTime / maxCoolTime);
        HeavyAttackIconeBar.fillAmount = fillAmount;
    }
 
    public void DefualtAttackIconEnable()
    {
        DefualtAttackIcon.SetActive(true);
    }

    public void DefualtAttackIconDisable()
    {
        DefualtAttackIcon.SetActive(false);
    }
}
