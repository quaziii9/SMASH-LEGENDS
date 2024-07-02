using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class ImageRotator : MonoBehaviour
{
    public Image targetImage; // 변경할 이미지
    public Sprite[] images;   // 변경될 이미지 배열
    private int currentIndex = 0; // 현재 이미지 인덱스

    void Start()
    {
        if (images.Length == 0)
        {
            Debug.LogWarning("Image array is empty. Please add images to the array.");
            return;
        }

        // 첫 번째 이미지 설정
        targetImage.sprite = images[currentIndex];

        // 1초마다 이미지를 변경하는 작업 시작
        ChangeImagePeriodically().Forget();
    }

    private async UniTaskVoid ChangeImagePeriodically()
    {
        while (true)
        {
            // 1초 대기
            await UniTask.Delay(1000);

            // 다음 이미지 인덱스 계산
            currentIndex = (currentIndex + 1) % images.Length;

            // 이미지 변경
            targetImage.sprite = images[currentIndex];
        }
    }
}
