using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropManager : MonoBehaviour
{
    public WeaponSO weaponSO;
    public WeaponInventory weaponInventory;

    private static readonly HashSet<int> ValidWeaponIDs = new HashSet<int> { 1, 2, 3, 4 };

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!IsValidWeapon(weaponSO))
        {
            Debug.LogWarning($"�� �� ���� ���� Ÿ�� Ȥ�� ID�Դϴ�. [Type: {weaponSO.WeapnType}] [ID: {weaponSO.id}]");
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
        Debug.Log($"�÷��̾ [ID: {weaponSO.id}] [Name: {weaponSO.name}] �� ȹ���߽��ϴ�.");

        ApplyWeaponStats();
        AddWeaponToInventory();
        Destroy(gameObject);
    }

    private void ApplyWeaponStats()
    {
        Debug.Log($"{weaponSO.name} �� ������ �����մϴ�. ���ݷ�: {weaponSO.baseDamage}");
        // ��: �÷��̾� �ɷ�ġ �ݿ� ��
        // player.SetDamage(weaponSO.baseDamage); �� ���� ���
    }

    private void AddWeaponToInventory()
    {
        if (weaponInventory != null)
        {
            weaponInventory.AddWeapon(weaponSO); // ����: �� �޼��尡 ������
        }
        else
        {
            Debug.LogWarning("WeaponInventory�� ������� �ʾҽ��ϴ�.");
        }
    }
}
