using UnityEngine;

namespace EnumTypes
{
    public enum LegendType { Peter, Hook }

    public enum GlobalEvents
    {
        PlayerDead,
        PlayerSpawned,
        PlayerInactive,
        PlayerDamaged,
    }


    public enum SoundEvents
    {
        FadeIn,
        FadeOut,
        MineBgmPlay,
    }

    public enum UIEvents
    {
        atkTime,
        addKillLog,
        atkImageSetActiveFalse,
        BackGroundUION,
        WinnerUION,
    }

    public enum GameEvents
    {
        playerDie,
        StartSkillGaugeIncrease,
        SetIcon,
        SetFaceImage
    }

    public enum IngameEvents
    {
        Hitted,
    }

    public enum PlayerEvents
    {
        isAtkTrue,
        isAtkFalse,
        WeaponColliderTrue,
        WeaponColliderFalse,
    }

    public enum LobbyEvents
    {
        LegendSpawn
    }

    public class EnumTypes : MonoBehaviour { }
}