using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropManager : MonoBehaviour
{
    public WeaponSO weaponSO;
    public WeaponInventory weaponInventory;

    private static readonly HashSet<int> ValidWeaponIDs = new HashSet<int> { 1, 2, 3, 4 };

    private void Awake()
    {
        if (weaponInventory == null)
        {
            weaponInventory = FindObjectOfType<WeaponInventory>();
            if (weaponInventory == null)
            {
                Debug.LogWarning("WeaponInventory를 자동으로 찾지 못했습니다. 인스펙터에 수동 연결을 권장합니다.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!IsValidWeapon(weaponSO))
        {
            Debug.LogWarning($"알 수 없는 무기 타입 혹은 ID입니다. [Type: {weaponSO.WeapnType}] [ID: {weaponSO.id}]");
            return;
        }

        HandleWeaponPickup();
    }

    private bool IsValidWeapon(WeaponSO weapon)
    {
        return weapon != null &&
               (weapon.WeapnType == WeaponType.Sword01 || weapon.WeapnType == WeaponType.Gun01) &&
               ValidWeaponIDs.Contains(weapon.id);
    }

    private void HandleWeaponPickup()
    {
        Debug.Log($"플레이어가 [ID: {weaponSO.id}] [Name: {weaponSO.name}] 을 획득했습니다.");

        ApplyWeaponStats();
        AddWeaponToInventory();
        Destroy(gameObject);
    }

    private void ApplyWeaponStats()
    {
        Debug.Log($"{weaponSO.name} 의 설정을 적용합니다. 공격력: {weaponSO.baseDamage}");
        // 예: 플레이어 능력치 반영
    }

    private void AddWeaponToInventory()
    {
        if (weaponInventory != null)
        {
            weaponInventory.AddWeapon(weaponSO);
        }
        else
        {
            Debug.LogWarning("WeaponInventory가 연결되지 않았습니다.");
        }
    }
}
