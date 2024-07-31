using Cysharp.Threading.Tasks;
using EnumTypes;
using EventLibrary;
using Mirror;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject LobbyPopup;
    public GameObject Title;
    public GameObject SelectLegendPopup;
    public int first = 0;

    public static UIManager Instance { get; private set; }
    public RoomManager roomManager;
    public RoomManager roomManagerPrefab;


    public List<SelectLegendButton> buttons = new List<SelectLegendButton>();
    public enum LegendType { Peter, Hook }
    public LegendType legendType = LegendType.Peter;

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
        roomManager = GetComponentInChildren<RoomManager>();  
        if(first ==0)
            SetUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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


    public void OnStartButtonClicked()
    {
        TryStartClient();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LobbyScene" && ResultUIManager.Instance.IsResultUION == false ) // 원하는 씬 이름으로 변경
        {
            LobbyUIEnable();
        }
    }


    private async void TryStartClient()
    {
        if (roomManager == null)
        {
            GameObject roomManagerObject = Instantiate(roomManagerPrefab.gameObject);
            roomManager = roomManagerObject.GetComponent<RoomManager>();
            roomManagerObject.transform.SetParent(this.transform);
        }


        try
        {
            roomManager.StartClient();
            //UIManager.Instance.MachintPopupEnable();
            await Task.Delay(500);  // 연결 시도 후 대기 시간 설정

            if (!NetworkClient.isConnected)
            {
                roomManager.StartHost();
            }
            else
            {
            }
        }
        catch (System.Exception ex)
        {
            roomManager.StartHost();

        }
    }

    public void LegendPopUpUIOn()
    {
        SelectLegendPopup.SetActive(true);
    }

    public void LegendPopUpUIOff()
    {
        SelectLegendPopup.SetActive(false);
    }

    public void GetLegendType(LegendType LegendType)
    {
        if (legendType != LegendType)
        {
            legendType = LegendType; // 새 값으로 설정
            if (legendType == LegendType.Peter)
                EventManager<LobbyEvents>.TriggerEvent(LobbyEvents.LegendSpawn, 0);
            else if (legendType == LegendType.Hook)
                EventManager<LobbyEvents>.TriggerEvent(LobbyEvents.LegendSpawn, 1);
        }
    }

    public void RegisterButton(SelectLegendButton button)
    {
        buttons.Add(button);
    }

    public void DeselectOtherButtons(SelectLegendButton selectedButton)
    {
        foreach (var button in buttons)
        {
            if (button != selectedButton)
            {
                button.Frame.SetActive(false);
            }
            else
            {
                button.Frame.SetActive(true); // 선택된 버튼의 프레임은 활성화 상태 유지
            }
        }
    }
}
