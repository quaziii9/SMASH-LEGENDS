using Cysharp.Threading.Tasks;
using UnityEngine;
using EventLibrary;
using EnumTypes;
using System.Threading;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private int gameDuration = 150; // 2분 30초
    private int timeRemaining;
    private CancellationTokenSource gameTimerCancellationTokenSource;

    public GameObject MainCamera;
    public GameObject Map;
    public GameObject PlayUI;
    public GameObject ResultUI;

    [SyncVar] public int HostLegend;
    [SyncVar] public int ClientLegend;

    public bool MatchOver = false;
    private bool isHostPlayer;



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

    public void SetHostStatus(bool isHost)
    {
        isHostPlayer = isHost;
    }

    public void SetLegendType( bool isHost, int LegendType)
    {
        if(isHost) { HostLegend = LegendType; }
        else { ClientLegend = LegendType; }
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
            if (MatchOver == true) return;
            DuelUIController.Instance.UpdateGameTime(timeRemaining);

            await UniTask.Delay(1000, cancellationToken: cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Game timer cancelled");
                return;
            }

            timeRemaining -= 1;
        }
        MatchOver = true;
        DuelUIController.Instance.UpdateGameTime(timeRemaining);
        PlayUIManager.Instance.MatchOverUI();
    }




    public void OnAnimationComplete()
    {
        // 비동기 메서드를 호출하는 동기 메서드
        RunDetermineWinner().Forget();
    }

    // 비동기 메서드를 동기 메서드로 래핑
    private async UniTaskVoid RunDetermineWinner()
    {
        await DetermineWinner();
    }


    private async UniTask DetermineWinner()
    {
        await UniTask.Delay(2000);

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
        // 결과 값 설정
        ResultUIManager.Instance.SetResult(isHostPlayer, WinHost ,HostLegend, ClientLegend);

        // 모든 플레이어를 디스커넥트
        if (NetworkServer.active)
        {
            NetworkManager.singleton.StopHost();
            SceneManager.LoadScene(NetworkManager.singleton.offlineScene); // 클라이언트는 오프라인 씬으로 이동
        }
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene(NetworkManager.singleton.offlineScene); // 클라이언트는 오프라인 씬으로 이동
        }
    }
}
