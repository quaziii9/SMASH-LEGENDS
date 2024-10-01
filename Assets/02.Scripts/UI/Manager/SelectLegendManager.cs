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
                PeterSkin.SetActive(true);
                HookSkin.SetActive(false);
                break;
            case LegendType.Hook:
                PeterSkin.SetActive(false);
                HookSkin.SetActive(true);
                break;
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
