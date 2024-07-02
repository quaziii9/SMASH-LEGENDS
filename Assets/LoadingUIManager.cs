using UnityEngine;

public class LoadingUIManager : MonoBehaviour
{
    public static LoadingUIManager Instance;

    [SerializeField]
    private UIMatchingManager matchingManager;
    public UIMatchingManager MatchingManager { get { return matchingManager; } }

    private void Awake()
    {
        Instance = this;
    }
}


