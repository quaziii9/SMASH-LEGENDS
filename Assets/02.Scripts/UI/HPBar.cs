using UnityEngine;
using UnityEngine.UI;

public class HPBar : Singleton<HPBar>
{
    [SerializeField] private Image healthBar;

    public void SetHealth(int health)
    {
        healthBar.fillAmount = (float)health / 9000f; // maxHp가 9000이므로 이를 기준으로 fillAmount를 설정
    }
}
