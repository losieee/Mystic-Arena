using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    // �� ���� �� ������ �����ϴ� ��ųʸ�
    [SerializeField]
    private Dictionary<WeaponSO, int> weaponCounts = new Dictionary<WeaponSO, int>();

    // ���� ������ ����
    private WeaponSO equippedWeapon;

    // �� ���� ī��Ʈ�� �ܺο��� ��ȸ �����ϵ��� public getter �߰�
    public IReadOnlyDictionary<WeaponSO, int> WeaponCounts => weaponCounts;

    /// ���� �߰�
    public void AddWeapon(WeaponSO weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("�߰��Ϸ��� ���Ⱑ null�Դϴ�.");
            return;
        }

        if (weaponCounts.ContainsKey(weapon))
        {
            weaponCounts[weapon]++;
            Debug.Log($"�̹� ���� ���� �����Դϴ�: {weapon.name}, ���� ���� �� {weaponCounts[weapon]}");
        }
        else
        {
            weaponCounts[weapon] = 1;
            Debug.Log($"���ο� ���⸦ ȹ���߽��ϴ�: {weapon.name} (1��)");
        }
    }

    /// ���� ����
    public void EquipWeapon(WeaponSO weapon)
    {
        if (!weaponCounts.ContainsKey(weapon))
        {
            Debug.LogWarning($"�����Ϸ��� ���⸦ �����ϰ� ���� �ʽ��ϴ�: {weapon.name}");
            return;
        }

        equippedWeapon = weapon;
        Debug.Log($"���⸦ �����߽��ϴ�: {weapon.name}");

        // ���� �ɷ�ġ�� �÷��̾�� �����ϴ� ������ ���� �߰� ����
    }

    /// ���� ���� ���� ��ȯ
    public WeaponSO GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    /// ���� ���� ��� ���� ��� ��ȯ
    public List<WeaponSO> GetAllWeapons()
    {
        return new List<WeaponSO>(weaponCounts.Keys);
    }

    /// ���� ���� ���� Ȯ��
    public bool HasWeapon(WeaponSO weapon)
    {
        return weaponCounts.ContainsKey(weapon);
    }

    /// Ư�� ������ ���� ���� ��ȯ
    public int GetWeaponCount(WeaponSO weapon)
    {
        return weaponCounts.TryGetValue(weapon, out int count) ? count : 0;
    }
}
