using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalRespawnTimerFillAmount : MonoBehaviour
{
    public Image fillImage;
    public float duration = 5f; // fillAmount가 1이 되기까지 걸리는 시간

    private void Start()
    {
        // TextMeshProUGUI 컴포넌트 가져오기
        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
            if (fillImage == null)
            {
                Debug.LogError("TextMeshProUGUI component is missing on this GameObject.");
            }
        }
    }

    private void OnEnable()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = 0.1f; // fillAmount 초기화
            StartFillAmountUpdate().Forget();
        }
        else
        {
            return;
        }
    }

    private async UniTaskVoid StartFillAmountUpdate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsedTime / duration);
            await UniTask.Yield();
        }
    }
}
