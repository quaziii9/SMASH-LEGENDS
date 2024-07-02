using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject LoadingImage;


    private void Start()
    {
        SettingGame();
    }

    public async void SettingGame()
    {
        await Task.Delay(4000);

        UIManager.Instance.SetPlaySceneInternal();

        LoadingImage.SetActive(false);
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
    }
}
    

