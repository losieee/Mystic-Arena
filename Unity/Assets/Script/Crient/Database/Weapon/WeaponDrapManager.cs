using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaPonDrapManager : MonoBehaviour
{
    public WeaponSO weaponSO;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (weaponSO.WeapnType)
            {
                case WeaponType.Sword01:
                    WeaponType_Sword01();
                    break;
                case WeaponType.Sword02:
                    WeaponType_Sword02();
                    break;
                case WeaponType.Gun01:
                    WeaponType_Gun01();
                    break;
                case WeaponType.Gun02:
                    WeaponType_Gun02();
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

    private void WeaponType_Sword02()
    {
        Debug.Log($"�÷��̾ {weaponSO.name} �� ȹ���߽��ϴ�.");
        Weapon_Handler();
    }

    private void WeaponType_Gun01()
    {
        Debug.Log($"�÷��̾ {weaponSO.name} �� ȹ���߽��ϴ�.");
        Weapon_Handler();
    }

    private void WeaponType_Gun02()
    {
        Debug.Log($"�÷��̾ {weaponSO.name} �� ȹ���߽��ϴ�.");
        Weapon_Handler();
    }

}
