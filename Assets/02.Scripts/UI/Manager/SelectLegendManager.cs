using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class SelectLegendManager : Singleton<SelectLegendManager>
{
    [SerializeField] GameObject SelectUICamera;
    [SerializeField] GameObject SelectLegnedUI;
    [SerializeField] GameObject SelectSkinUI;
    [SerializeField] GameObject PeterSkin;
    [SerializeField] GameObject HookSkin;
    public LegendType SkinLegendType = LegendType.Peter;

    [SerializeField]private int currentSkinIndex = 0;  // 현재 선택된 스킨 인덱스
    private const int totalSkins = 3;  // 총 스킨 개수 (0, 1, 2)

    public void SelectLegendUIOnEnable()
    {
        SelectUICamera.SetActive(true);
        SelectLegnedUI.SetActive(true);
    }

    public void SelectLegendUIOnDisable()
    {
        SelectUICamera.SetActive(false);
        SelectLegnedUI.SetActive(false);
        EventManager<LobbyEvents>.TriggerEvent(LobbyEvents.LegendSpawn, (int)LobbyManager.Instance.legendType, LobbyManager.Instance.legendSkinType);
    }

    public void SelectSkinUIOnEnable()
    {
        SelectSkinUI.SetActive(true);

        switch (SkinLegendType)
        {
            case LegendType.Peter:
                ActivateSkin(PeterSkin, 0);
                DeactivateAllChildren(HookSkin);
                break;
            case LegendType.Hook:
                ActivateSkin(HookSkin, 0);
                DeactivateAllChildren(PeterSkin);
                break;
        }
    }

    // 스킨 변경 (왼쪽 또는 오른쪽 버튼 누를 때 호출할 함수)
    public void ChangeSkin(bool isLeft)
    {
        if (SkinLegendType == LegendType.Peter)
        {
            ChangeSelectedSkin(PeterSkin, isLeft);
        }
        else if (SkinLegendType == LegendType.Hook)
        {
            ChangeSelectedSkin(HookSkin, isLeft);
        }
    }

    // 스킨 변경 로직
    private void ChangeSelectedSkin(GameObject parent, bool isLeft)
    {
        // 현재 스킨 비활성화
        DeactivateAllChildren(parent);

        // 왼쪽 버튼을 눌렀을 때는 인덱스를 감소, 오른쪽 버튼은 증가
        if (isLeft)
        {
            currentSkinIndex = (currentSkinIndex - 1 + totalSkins) % totalSkins;
        }
        else
        {
            currentSkinIndex = (currentSkinIndex + 1) % totalSkins;
        }

        // 새로운 스킨 활성화
        ActivateSkin(parent, currentSkinIndex);
    }

    // 특정 인덱스의 스킨만 활성화하는 함수
    private void ActivateSkin(GameObject parent, int index)
    {
        if (index >= 0 && index < parent.transform.childCount)
        {
            parent.SetActive(true);  // 부모 오브젝트가 비활성화된 상태일 경우 활성화
            parent.transform.GetChild(index).gameObject.SetActive(true);  // 해당 스킨 활성화
        }
    }

    // 특정 오브젝트의 모든 자식들을 비활성화하는 함수
    private void DeactivateAllChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(false);  // 모든 자식 비활성화
        }
    }

    public void SelectSkinUIOnDisable()
    {
        DeactivateAllChildren(PeterSkin);
        DeactivateAllChildren(HookSkin);
        LobbyManager.Instance.legendSkinType = currentSkinIndex;
        currentSkinIndex = 0;  // 전설을 변경할 때 스킨 인덱스를 초기화
        SelectSkinUI.SetActive(false);
    }

    public void GetLegendType(LegendType LegendType)
    {
        if (SkinLegendType != LegendType)
        {
            SkinLegendType = LegendType;
            currentSkinIndex = 0;  // 전설을 변경할 때 스킨 인덱스를 초기화
        }
    }
}

