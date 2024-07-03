using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LegendUI : MonoBehaviour
{
    public TextMeshProUGUI HPText;
    public GameObject[] HPBar;
    public Image HPFillAmount;
    public Image HeavyAttackCoolTimeFillAmount;
    public GameObject IsSkillReady;

    private void LateUpdate()
    {
        // 캔버스의 회전을 고정
        transform.rotation = Quaternion.identity;
    }

    public void UpdateHPUI(int currentHp, int maxHp)
    {
        HPText.text = $"{currentHp}";
        HPFillAmount.fillAmount = (float)currentHp / maxHp;
    }

    public void SetHpBar(int maxHp)
    {
        int BarNum = maxHp / 1000;
        for(int i =0; i <HPBar.Length; i++)
        {
            if(i < BarNum)
            {
                HPBar[i].SetActive(true);
            }
            else
            {
                HPBar[i].SetActive(false);
            }

        }
    }

    public void UpdateHeavyAttackCoolTimeUI(float currentCoolTime, float maxCoolTime)
    {
        float fillAmount = 1 - (currentCoolTime / maxCoolTime);
        HeavyAttackCoolTimeFillAmount.fillAmount = fillAmount;

        if (fillAmount == 1)
        {
            Color color = new Color32(43, 165, 240, 255);
            HeavyAttackCoolTimeFillAmount.color = color;
            
        }
        else
        {
            Color color = new Color32(162, 158, 151, 255);
            HeavyAttackCoolTimeFillAmount.color = color;
        }
    }

    public void LegendUIAllReadySkill()
    {
        IsSkillReady.SetActive(true);
    }
}
