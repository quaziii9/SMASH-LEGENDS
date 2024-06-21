using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

public class PlayPositionManager : NetworkBehaviour
{
    private Vector3 startPosition1 = new Vector3(-20, 1.5f, 0); // 원하는 위치로 설정
    private Vector3 startPosition2 = new Vector3(20, 1.5f, 0); // 원하는 위치로 설정

    void Start()
    {
        PositionPlayersAsync().Forget();
    }

    private async UniTaskVoid PositionPlayersAsync()
    {
        await UniTask.Delay(1000); // 플레이어들이 모두 생성되도록 잠시 대기

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(playerObjects.Length);

        foreach (GameObject player in playerObjects)
        {
            Debug.Log("fase");
            player.SetActive(false); // 초기 위치 설정 전에 비활성화
            await UniTask.Delay(500); // 플레이어들이 모두 생성되도록 잠시 대기
        }

        switch (playerObjects.Length)
        {
            case 1:
                playerObjects[0].transform.position = startPosition1;
                Debug.Log(playerObjects[0].transform.position);
                break;
            case 2:
                playerObjects[0].transform.position = startPosition1;
                playerObjects[0].SetActive(true);
                Debug.Log(playerObjects[0].transform.position);
                await UniTask.Delay(500);
                playerObjects[1].transform.position = startPosition2;
                playerObjects[1].SetActive(true);
                Debug.Log(playerObjects[1].transform.position);
                break;
        }
    }
}
