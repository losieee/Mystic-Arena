using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaPonDrapManager : MonoBehaviour
{
    public WeaponSO weaponSO;
    public WeaponData weaponData;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (weaponSO.WeapnType)
            {
                case WeaponType.Sword01:
                    WeaponType_Sword01();
                    break;
                case WeaponType.Gun01:
                    WeaponType_Gun01();
                    break;
                default:
                    Debug.LogWarning("�� �� ���� ���� Ÿ���Դϴ�.");
                    break;
            }
        }
    }


    private void Weapon_Handler()
    { 
        Destroy(gameObject);
    }

    private void WeaponType_Sword01()
    {
        Debug.Log($"�÷��̾ {weaponSO.name} �� ȹ���߽��ϴ�.");
        Weapon_Handler();
    }


    private void WeaponType_Gun01()
    {
        Debug.Log($"�÷��̾ {weaponSO.name} �� ȹ���߽��ϴ�.");
        Weapon_Handler();
    }

}
