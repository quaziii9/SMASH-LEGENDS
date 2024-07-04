using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DuelUIController : MonoBehaviour
{
    public static DuelUIController Instance { get; private set; }

    public Image hostHealthBar;
    public Image clientHealthBar;

    public GameObject[] hostScoreBar;
    public GameObject[] clientScoreBar;

    public GameObject hostRespawnTimer;
    public GameObject clientRespawnTimer;

    public GameObject RespawnTimer;

    public int hostScore = 0;
    public int clientScore = 0;

    public int RespawnTime = 5;
    public int hostRespawnTime;
    public int clientRespawnTime;

    public GameObject DefualtAttackIcon;
    public Image HeavyAttackIconeBar;
    public GameObject HeavyAttackTextObject;
    public TextMeshProUGUI HeavyAttackText;
    public Image SkillAttackIconeBar;
    public GameObject SkillAttackKey;

    public TextMeshProUGUI GameTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void Start()
    {
        StartActive().Forget();
        HeavyAttackText = HeavyAttackTextObject.GetComponent<TextMeshProUGUI>();
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

    public void UpdateScore(bool DeadIsHost)
    {
        if (DeadIsHost)
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
                    EndGame(false);
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
                    EndGame(true);
                    break;
                default:
                    break;
            }
        }
    }

    private void EndGame(bool WinHost)
    {
        GameManager.Instance.EndGame(WinHost);
    }

    public float GetPlayerHpRatio(bool isHost)
    {
        if (isHost)
        {
            return hostHealthBar.fillAmount;
        }
        else
        {
            return clientHealthBar.fillAmount;
        }
    }

    public void StartRespawnTimer(bool isHost)
    {
        if (isHost)
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

    public void UpdateHeavyAttackIconeCoolTime(float currentCoolTime, float maxCoolTime, bool TextSet)
    {
        float fillAmount = 1 - (currentCoolTime / maxCoolTime);
        HeavyAttackIconeBar.fillAmount = fillAmount;
        if (TextSet)
        {
            HeavyAttackTextObject.SetActive(true);
            HeavyAttackText.text = Mathf.CeilToInt(currentCoolTime).ToString();
            if (currentCoolTime == 0) HeavyAttackTextObject.SetActive(false);
        }
    }

    public void UpdateSkillAttackIconeCoolGuage(float currentGauge, float maxGauge)
    {
        //SkillAttackIconeBar.fillAmount = Mathf.Lerp(SkillAttackIconeBar.fillAmount, currentGauge / maxGauge, Time.deltaTime * 10f);
        float fillAmount = currentGauge / maxGauge;
        SkillAttackIconeBar.fillAmount = fillAmount;
    }

    public void SkillAttackKeyEnable(bool Active)
    {
        SkillAttackKey.SetActive(Active);
    }

    public void DefualtAttackIconEnable()
    {
        DefualtAttackIcon.SetActive(true);
    }

    public void DefualtAttackIconDisable()
    {
        DefualtAttackIcon.SetActive(false);
    }

    // GameManager에서 호출하는 시간 업데이트 메서드
    public void UpdateGameTime(int remainTime)
    {
        GameTime.text = $"{remainTime / 60:D2}:{remainTime % 60:D2}";
    }
}
