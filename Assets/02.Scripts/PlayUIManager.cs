using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

public class PlayUIManager : MonoBehaviour
{
    public GameObject GameStartUI;
    public GameObject LoadingUI;
    public GameObject DuelModePopup;
    public GameObject matchOverUI;

    public static PlayUIManager Instance { get; private set; }

    private void Start()
    {
        SettingGame().Forget();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async UniTaskVoid SettingGame()
    {
        await Task.Delay(1000);

        DuelModePopup.SetActive(false);
        await Task.Delay(3000);

        LoadingUI.SetActive(false);
        GameStartUI.SetActive(true);
    }

    public async void CompleteDotween()
    {
        await Task.Delay(1500);

        GameStartUI.SetActive(false);
        GameManager.Instance.InitializePlayers();
        DuelModePopup.SetActive(true);

        GameManager.Instance.StartGame();
    }

    public void MatchOverUI()
    {
        GameManager.Instance.MatchOver = true;
        matchOverUI.SetActive(true);
    }
}
