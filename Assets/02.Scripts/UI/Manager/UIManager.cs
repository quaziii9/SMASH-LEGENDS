using Cysharp.Threading.Tasks;
using Mirror;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject LobbyPopup;
    public GameObject Title;
    public int first = 0;

    public static UIManager Instance { get; private set; }
    public RoomManager roomManager;
    public RoomManager roomManagerPrefab;

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
        if (scene.name == "LobbyScene") // 원하는 씬 이름으로 변경
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
                Debug.Log("Client connection failed, starting host.");
                roomManager.StartHost();
            }
            else
            {
                Debug.Log("Client connected successfully.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception occurred: {ex.Message}");
            roomManager.StartHost();

        }
    }
}
