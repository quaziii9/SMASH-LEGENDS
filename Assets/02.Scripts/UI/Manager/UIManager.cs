using Cysharp.Threading.Tasks;
using Mirror;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject LobbyPopup;
    public GameObject Title;

    public void Start()
    {
        SetUI();
    }

    private async void SetUI()
    {
        await Task.Delay(2000); // 추가로 2초 대기 (총 3초)
        Title.SetActive(false);
    }
}
