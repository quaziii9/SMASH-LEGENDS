using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class RespawnTimerUI : MonoBehaviour
{
    public TextMeshProUGUI respawnTimeText;
    public bool isHost; // 호스트인지 클라이언트인지 구분하는 변수

    private int respawnTime;
    private bool isRunning = false;

    private void Start()
    {
        // TextMeshProUGUI 컴포넌트 가져오기
        if (respawnTimeText == null)
        {
            respawnTimeText = GetComponent<TextMeshProUGUI>();
            if (respawnTimeText == null)
            {
                Debug.LogError("TextMeshProUGUI component is missing on this GameObject.");
            }
        }
    }

    private void OnEnable()
    {
        // TextMeshProUGUI가 정상적으로 할당되었는지 확인
        if (respawnTimeText != null)
        {
            respawnTime = 5; // 타이머 초기화
            StartRespawnCountdown().Forget();
        }
        else
        {
            return;
        }
    }

    private void OnDisable()
    {
        isRunning = false; // 타이머 중지
    }

    private async UniTaskVoid StartRespawnCountdown()
    {
        isRunning = true;
        while (respawnTime > 0 && isRunning)
        {
            respawnTimeText.text = $"{respawnTime}";
            await UniTask.Delay(1000);
            respawnTime--;
        }
    }
}
