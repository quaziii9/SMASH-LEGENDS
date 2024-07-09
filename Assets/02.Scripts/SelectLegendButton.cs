using DG.Tweening;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLegendButton : MonoBehaviour
{
    private Button button;
    private DOTweenAnimation dotweenAnimation;
    public GameObject Frame;

    private void Start()
    {
        button = GetComponent<Button>();
        dotweenAnimation = GetComponent<DOTweenAnimation>();

        button.onClick.AddListener(SelectLegendButtonClick);
        UIManager.Instance.RegisterButton(this);
    }

    private void SelectLegendButtonClick()
    {
        dotweenAnimation.DOPlayForward();
        bool isActive = Frame.activeSelf;
        Frame.SetActive(!isActive);

        UIManager.Instance.DeselectOtherButtons(this);

        string objectName = gameObject.name;
        if (objectName.Contains("Peter"))
        {
            UIManager.Instance.GetLegendType(UIManager.LegendType.Peter);
        }
        else if (objectName.Contains("Hook"))
        {
            UIManager.Instance.GetLegendType(UIManager.LegendType.Hook);
        }
    }

    public void PlayEnd()
    {
        dotweenAnimation.DORewind();
    }
}
