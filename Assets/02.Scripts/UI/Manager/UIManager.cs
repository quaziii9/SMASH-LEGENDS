using Cysharp.Threading.Tasks;
using Mirror;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameObject LobbyPopup;
    public GameObject DuelModePopup;
   // public GameObject MatchingPopup;
    public GameObject Title;

    public void Start()
    {
        SetUI();
    }

    private async void SetUI()
    {
        await Task.Delay(500); // 1초 대기
        DuelModePopup.SetActive(false);
       // MatchingPopup.SetActive(false);

        await Task.Delay(2000); // 추가로 2초 대기 (총 3초)
        Title.SetActive(false);
    }

    public async void MachintPopupEnable()
    {
        //MatchingPopup.SetActive(true);

        await Task.Delay(500);

        LobbyPopup.SetActive(false);
    }

    public async UniTaskVoid SetPlaySceneInternal()
    {
        DuelModePopup.SetActive(true);

        await UniTask.Delay(1000);

        //MatchingPopup.SetActive(false);
    }
}
