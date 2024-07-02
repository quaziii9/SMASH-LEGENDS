using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject GameStartUI;
    public GameObject LoadingUI;
    public GameObject DuelModePopup;
    private int gameDuration = 150; // 2분 30초

    private void Start()
    {
        SettingGame().Forget();
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
        Debug.Log("complete");
        await Task.Delay(1500);

        GameStartUI.SetActive(false);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length != 2)
        {
            Debug.LogWarning("There are not exactly two players in the scene.");
            return;
        }

        foreach (GameObject player in players)
        {
            // StatController 컴포넌트를 가져오기
            StateController stateController = player.GetComponent<StateController>();
            if (stateController != null)
            {
                // StatController의 함수 실행
                stateController.PositionPlayersAsync();
            }
            else
            {
                Debug.LogWarning($"StatController not found on player: {player.name}");
            }
        }

        DuelModePopup.SetActive(true);
        StartGameTimer().Forget();
    }

    private async UniTaskVoid StartGameTimer()
    {
        float timeRemaining = gameDuration;

        while (timeRemaining > 0)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            // DuelManager에 시간 업데이트 요청
            DuelUIController.Instance.UpdateGameTime(minutes, seconds);

            await UniTask.Delay(1000);

            timeRemaining -= 1;
        }

        DuelUIController.Instance.UpdateGameTime(0, 0);
        EndGame();
    }

    private void EndGame()
    {
        // 게임 종료 로직
        Debug.Log("Game Over");
    }
}
    

