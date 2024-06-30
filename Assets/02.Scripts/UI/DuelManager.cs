using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : Singleton<DuelManager>
{
    [SerializeField] private HPBar hostHPBar;
    [SerializeField] private HPBar clientHPBar;


    public void UpdateHealthBar(int health, bool isHost)
    {
        if (isHost)
        {
            hostHPBar.SetHealth(health);
        }
        else
        {
            clientHPBar.SetHealth(health);
        }
    }
}
