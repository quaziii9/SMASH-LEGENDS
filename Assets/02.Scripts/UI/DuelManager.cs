using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelManager : Singleton<DuelManager>
{
    public static DuelManager Instance { get; private set; }

    public Image hostHealthBar;
    public Image clientHealthBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� ����Ǿ UIManager�� �ı����� �ʵ��� ��
        }
        else
        {
            Destroy(gameObject); // �ߺ��� �ν��Ͻ��� ������ �ʵ��� ��
        }
    }

    public void UpdateHealthBar(int currentHp, int maxHp, bool isHost)
    {
        float fillAmount = (float)currentHp / maxHp;

        if (isHost)
        {
            hostHealthBar.fillAmount = fillAmount;
        }
        else
        {
            clientHealthBar.fillAmount = fillAmount;
        }
    }
}
