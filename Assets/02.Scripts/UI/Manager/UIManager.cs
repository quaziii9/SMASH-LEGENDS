using Cysharp.Threading.Tasks;
using Mirror;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject LobbyPopup;
    public GameObject Title;
    public int first = 0;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 오브젝트가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        if(first ==0)
            SetUI();
    }

    private async void SetUI()
    {
        Title.SetActive(true);
        LobbyPopup.SetActive(true);
        first = 1;
        await Task.Delay(2000); // 추가로 2초 대기 (총 3초)
        Title.SetActive(false);
    }

    public async void LobbyUIEnable()
    {
        Title.SetActive(true);
        LobbyPopup.SetActive(true);
        await Task.Delay(2000); // 추가로 2초 대기 (총 3초)
        Title.SetActive(false);
    }

    public void LobbyUIDisable()
    {
        LobbyPopup.SetActive(false);
        Title.SetActive(false);
    }
}
