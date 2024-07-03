using Cysharp.Threading.Tasks;
using UnityEngine;
using EventLibrary;
using EnumTypes;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int gameDuration = 150; // 2분 30초
    private int timeRemaining;
    private CancellationTokenSource gameTimerCancellationTokenSource;

    public GameObject MainCamera;
    public GameObject Map;
    public GameObject PlayUI;
    public GameObject ResultUI;

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

    private void OnDestroy()
    {
        if (gameTimerCancellationTokenSource != null)
        {
            gameTimerCancellationTokenSource.Cancel();
        }
    }

    public void InitializePlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length != 2)
        {
            Debug.LogWarning("There are not exactly two players in the scene.");
            return;
        }

        foreach (GameObject player in players)
        {
            StateController stateController = player.GetComponent<StateController>();
            if (stateController != null)
            {
                stateController.PositionPlayersAsync();
            }
            else
            {
                Debug.LogWarning($"StatController not found on player: {player.name}");
            }
        }
    }

    public void StartGame()
    {
        if (gameTimerCancellationTokenSource != null)
        {
            gameTimerCancellationTokenSource.Cancel();
        }

        gameTimerCancellationTokenSource = new CancellationTokenSource();
        StartGameTimer(gameTimerCancellationTokenSource.Token).Forget();
        EventManager<GameEvents>.TriggerEvent(GameEvents.StartSkillGaugeIncrease);
    }

    private async UniTaskVoid StartGameTimer(CancellationToken cancellationToken)
    {
        timeRemaining = gameDuration;

        while (timeRemaining > 0)
        {
            DuelUIController.Instance.UpdateGameTime(timeRemaining);

            await UniTask.Delay(1000, cancellationToken: cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Game timer cancelled");
                return;
            }

            timeRemaining -= 1;
        }

        DuelUIController.Instance.UpdateGameTime(timeRemaining);
        DetermineWinner();
    }

    private void DetermineWinner()
    {
        int hostScore = DuelUIController.Instance.hostScore;
        int clientScore = DuelUIController.Instance.clientScore;

        if (hostScore > clientScore)
        {
            EndGame(true); // Host wins
        }
        else if (clientScore > hostScore)
        {
            EndGame(false); // Client wins
        }
        else
        {
            float hostHpRatio = DuelUIController.Instance.GetPlayerHpRatio(true);
            float clientHpRatio = DuelUIController.Instance.GetPlayerHpRatio(false);

            if (hostHpRatio > clientHpRatio)
            {
                EndGame(true); // Host wins by HP ratio
            }
            else if (clientHpRatio > hostHpRatio)
            {
                EndGame(false); // Client wins by HP ratio
            }
            else
            {
                EndGame(null); // It's a draw
            }
        }
    }

    public void EndGame(bool? WinHost)
    {
        MainCamera.SetActive(false);
        Map.SetActive(false);
        PlayUI.SetActive(false);
        ResultUI.SetActive(true);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.SetActive(false);
        }


        if (WinHost.HasValue)
        {
            if (WinHost.Value)
            {
                ResultUIManager.Instance.ResultTextSet("HostWin");
                Debug.Log("Host wins!");
            }
            else
            {
                Debug.Log("Client wins!");
                ResultUIManager.Instance.ResultTextSet("ClientWin");

            }
        }
        else
        {
            Debug.Log("It's a draw!");
            ResultUIManager.Instance.ResultTextSet("Draw");
        }
    }
}
