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
        LobbyManager.Instance.RegisterButton(this);
    }

    private void SelectLegendButtonClick()
    {
        dotweenAnimation.DOPlayForward();
        bool isActive = Frame.activeSelf;
        Frame.SetActive(!isActive);

        LobbyManager.Instance.DeselectOtherButtons(this);

        string objectName = gameObject.name;
        if (objectName.Contains("Peter"))
        {
            LobbyManager.Instance.GetLegendType(LegendType.Peter);
        }
        else if (objectName.Contains("Hook"))
        {
            LobbyManager.Instance.GetLegendType(LegendType.Hook);
        }
    }

    public void PlayEnd()
    {
        dotweenAnimation.DORewind();
    }
}
