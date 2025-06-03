using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    // ������ ��� ���� ����Ʈ
    private List<WeaponSO> ownedWeapons = new List<WeaponSO>();

    // ���� ������ ���� (�ɼ�)
    private WeaponSO equippedWeapon;

    /// ���⸦ �κ��丮�� �߰��մϴ� (�ߺ� ���� ����)
    public void AddWeapon(WeaponSO weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("�߰��Ϸ��� ���Ⱑ null�Դϴ�.");
            return;
        }

        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
            Debug.Log($"���⸦ ȹ���߽��ϴ�: {weapon.name}");
        }
        else
        {
            Debug.Log($"�̹� ���� ���� �����Դϴ�: {weapon.name}");
        }
    }

    /// ���� ���� ���� ����
    public void EquipWeapon(WeaponSO weapon)
    {
        if (!ownedWeapons.Contains(weapon))
        {
            Debug.LogWarning($"�����Ϸ��� ���⸦ �����ϰ� ���� �ʽ��ϴ�: {weapon.name}");
            return;
        }

        equippedWeapon = weapon;
        Debug.Log($"���⸦ �����߽��ϴ�: {weapon.name}");

        // �ʿ� ��, ���� �ɷ�ġ�� �÷��̾�� ����
        // ��: player.SetDamage(weapon.baseDamage);
    }

    /// ���� ���� ���� ��ȯ
    public WeaponSO GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    /// ������ ��� ���� ��ȯ (���纻)
    public List<WeaponSO> GetAllWeapons()
    {
        return new List<WeaponSO>(ownedWeapons);
    }


    /// ���� ���� ���� Ȯ��
    public bool HasWeapon(WeaponSO weapon)
    {
        return ownedWeapons.Contains(weapon);
    }
}

