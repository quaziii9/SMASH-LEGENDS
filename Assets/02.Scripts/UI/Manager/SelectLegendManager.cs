using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class SelectLegendManager : Singleton<SelectLegendManager>
{
    [SerializeField] private GameObject _selectUICamera;
    [SerializeField] private GameObject _selectLegnedUI;
    [SerializeField] private GameObject _selectSkinUI;
    [SerializeField] private GameObject _peterSkin;
    [SerializeField] private GameObject _hookSkin;
    public LegendType SkinLegendType = LegendType.Peter;

    [SerializeField] private List<GameObject> _peterSkins = new List<GameObject>();
    [SerializeField] private List<GameObject> _hookSkins = new List<GameObject>();

    [SerializeField] private int _currentSkinIndex = 0;  // 현재 선택된 스킨 인덱스
    private const int _totalSkins = 3;  // 총 스킨 개수 (0, 1, 2)

    private void Start()
    {
        // 자식 오브젝트들을 리스트에 저장
        PopulateSkinList(_peterSkin, _peterSkins);
        PopulateSkinList(_hookSkin, _hookSkins);

        // 스킨 비활성화: 시작 시 모든 스킨이 비활성화되어 있어야 합니다.
        DeactivateAll(_peterSkins);
        DeactivateAll(_hookSkins);
    }

    // 하위 자식 오브젝트들을 리스트로 저장하는 함수
    private void PopulateSkinList(GameObject parent, List<GameObject> list)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            list.Add(parent.transform.GetChild(i).gameObject);
        }
    }

    public void SelectLegendUIOnEnable()
    {
        _selectUICamera.SetActive(true);
        _selectLegnedUI.SetActive(true);
    }

    public void SelectLegendUIOnDisable()
    {
        _selectUICamera.SetActive(false);
        _selectLegnedUI.SetActive(false);
        EventManager<LobbyEvents>.TriggerEvent(LobbyEvents.LegendSpawn, (int)LobbyManager.Instance.legendType, LobbyManager.Instance.legendSkinType);
    }

    public void SelectSkinUIOnEnable()
    {
        _selectSkinUI.SetActive(true);

        // Peter와 Hook의 자식들을 리스트로 관리
        if (SkinLegendType == LegendType.Peter)
        {
            // Hook 스킨은 비활성화하고 Peter 스킨을 활성화
            _peterSkin.SetActive(true);
            _hookSkin.SetActive(false);
            DeactivateAll(_hookSkins);
            ActivateSkin(_peterSkins, 0);
        }
        else
        {
            // Peter 스킨은 비활성화하고 Hook 스킨을 활성화
            _peterSkin.SetActive(false);
            _hookSkin.SetActive(true);
            DeactivateAll(_peterSkins);
            ActivateSkin(_hookSkins, 0);
        }
    }

    // 스킨 변경 (왼쪽 또는 오른쪽 버튼 누를 때 호출할 함수)
    public void ChangeSkin(bool isLeft)
    {
        List<GameObject> currentSkinList = SkinLegendType == LegendType.Peter ? _peterSkins : _hookSkins;

        // 현재 스킨 비활성화
        currentSkinList[_currentSkinIndex].SetActive(false);

        // 스킨 인덱스 업데이트
        if (isLeft)
        {
            _currentSkinIndex = (_currentSkinIndex - 1 + _totalSkins) % _totalSkins;
        }
        else
        {
            _currentSkinIndex = (_currentSkinIndex + 1) % _totalSkins;
        }

        // 새로운 스킨 활성화
        currentSkinList[_currentSkinIndex].SetActive(true);
    }

    // 리스트에서 특정 인덱스의 스킨만 활성화
    private void ActivateSkin(List<GameObject> skinList, int index)
    {
        if (index >= 0 && index < skinList.Count)
        {
            skinList[index].SetActive(true);  // 해당 스킨 활성화
        }
    }

    // 리스트의 모든 오브젝트 비활성화
    private void DeactivateAll(List<GameObject> skinList)
    {
        foreach (GameObject skin in skinList)
        {
            skin.SetActive(false);
        }
    }

    public void SelectSkinUIOnDisable()
    {
        DeactivateAll(_peterSkins);
        DeactivateAll(_hookSkins);
        LobbyManager.Instance.legendSkinType = _currentSkinIndex;
        _currentSkinIndex = 0;  
        _selectSkinUI.SetActive(false);
    }

    public void GetLegendType(LegendType LegendType)
    {
        if (SkinLegendType != LegendType)
        {
            SkinLegendType = LegendType;
            _currentSkinIndex = 0;  
        }
    }
}