using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class SelectLegendManager : Singleton<SelectLegendManager>
{
    [SerializeField] GameObject SelectUICamera;
    [SerializeField] GameObject SelectLegnedUI;
    [SerializeField] GameObject SelectSkinUI;
    [SerializeField] GameObject PeterSkin;
    [SerializeField] GameObject HookSkin;
    public LegendType SkinLegendType = LegendType.Peter;


    public void SelectLegendUIOnEnable()
    {
        SelectUICamera.SetActive(true);
        SelectLegnedUI.SetActive(true);
    }

    public void SelectLegendUIOnDisable()
    {
        SelectUICamera.SetActive(false);
        SelectLegnedUI.SetActive(false);
    }


    public void SelectSkinUIOnEnable()
    {
        SelectSkinUI.SetActive(true);

        switch (SkinLegendType)
        {
            case LegendType.Peter:
                ActivateFirstChild(PeterSkin);  // PeterSkin의 0번째 자식만 활성화
                DeactivateAllChildren(HookSkin);  // HookSkin의 모든 자식 비활성화
                break;
            case LegendType.Hook:
                ActivateFirstChild(HookSkin);  // HookSkin의 0번째 자식만 활성화
                DeactivateAllChildren(PeterSkin);  // PeterSkin의 모든 자식 비활성화
                break;
        }
    }

    // 특정 오브젝트의 0번째 자식만 활성화하는 함수
    private void ActivateFirstChild(GameObject parent)
    {
        parent.SetActive(true);
        if (parent.transform.childCount > 0)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                if (i == 0)
                    parent.transform.GetChild(i).gameObject.SetActive(true);  // 0번째 자식 활성화
                else
                    parent.transform.GetChild(i).gameObject.SetActive(false); // 나머지 비활성화
            }
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
        SelectSkinUI.SetActive(false);
    }


        public void GetLegendType(LegendType LegendType)
        {
        if (SkinLegendType != LegendType)
        {
            SkinLegendType = LegendType;
        }
    }
}
