using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EventLibrary;
using EnumTypes;

public class GameManager : MonoBehaviour
{
    public GameObject GameStartUI;
    public GameObject LoadingUI;
    public GameObject DuelModePopup;
    private int gameDuration = 150; // 2분 30초
    private int timeRemaining;

    private void Start()
    {
        SettingGame().Forget();
        timeRemaining = 150;
        Debug.Log(gameDuration);
        Debug.Log(timeRemaining);
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
        EventManager<GameEvents>.TriggerEvent(GameEvents.StartSkillGaugeIncrease);
        
    }

    private async UniTaskVoid StartGameTimer()
    {
        timeRemaining = gameDuration;

        while (timeRemaining > 0)
        {
            // DuelManager에 시간 업데이트 요청
            DuelUIController.Instance.UpdateGameTime(timeRemaining);

            await UniTask.Delay(1000);

            timeRemaining -= 1;
        }

        DuelUIController.Instance.UpdateGameTime(timeRemaining);
        EndGame();
    }

    private void EndGame()
    {
        // 게임 종료 로직
        Debug.Log("Game Over");
    }
}
    

