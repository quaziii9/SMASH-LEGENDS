using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalRespawnTiemrUI : MonoBehaviour
{
    public TextMeshProUGUI LocalrespawnTimeText;
    private int respawnTime;
    private bool isRunning = false;

    private void Start()
    {
        // TextMeshProUGUI 컴포넌트 가져오기
        if (LocalrespawnTimeText == null)
        {
            LocalrespawnTimeText = GetComponent<TextMeshProUGUI>();
            if (LocalrespawnTimeText == null)
            {
                Debug.LogError("TextMeshProUGUI component is missing on this GameObject.");
            }
        }
    }

    private void OnEnable()
    {
        // TextMeshProUGUI가 정상적으로 할당되었는지 확인
        if (LocalrespawnTimeText != null)
        {
            respawnTime = 5; // 타이머 초기화
            StartRespawnCountdown().Forget();
        }
        else
        {
            Debug.LogError("respawnTimeText is not assigned.");
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
            LocalrespawnTimeText.text = $"부활 ({respawnTime})";
            await UniTask.Delay(1000);
            respawnTime--;
        }
    }
}
