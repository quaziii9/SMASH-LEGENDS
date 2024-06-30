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
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 UIManager가 파괴되지 않도록 함
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스가 생기지 않도록 함
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
