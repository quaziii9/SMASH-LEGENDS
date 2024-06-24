using UnityEngine;
using EventLibrary;
using EnumTypes;
using System;
using UnityEngine.EventSystems;

public class WeaponController : MonoBehaviour
{
    Collider weaponCollider;

    private void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log(other.name);
            EventManager<IngameEvents>.TriggerEvent(IngameEvents.Hitted);
            WeaponColliderDisable();
        }
    }

    public void WeaponColliderEnable()
    {
        weaponCollider.enabled = true;
    }

    public void WeaponColliderDisable()
    {
        weaponCollider.enabled = false;
    }

}
