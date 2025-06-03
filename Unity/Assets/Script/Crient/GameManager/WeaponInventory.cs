using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    // 각 무기 별 개수를 저장하는 딕셔너리
    [SerializeField]
    private Dictionary<WeaponSO, int> weaponCounts = new Dictionary<WeaponSO, int>();

    // 현재 장착된 무기
    private WeaponSO equippedWeapon;

    // 각 무기 카운트를 외부에서 조회 가능하도록 public getter 추가
    public IReadOnlyDictionary<WeaponSO, int> WeaponCounts => weaponCounts;

    /// 무기 추가
    public void AddWeapon(WeaponSO weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("추가하려는 무기가 null입니다.");
            return;
        }

        if (weaponCounts.ContainsKey(weapon))
        {
            weaponCounts[weapon]++;
            Debug.Log($"이미 보유 중인 무기입니다: {weapon.name}, 개수 증가 → {weaponCounts[weapon]}");
        }
        else
        {
            weaponCounts[weapon] = 1;
            Debug.Log($"새로운 무기를 획득했습니다: {weapon.name} (1개)");
        }
    }

    /// 무기 장착
    public void EquipWeapon(WeaponSO weapon)
    {
        if (!weaponCounts.ContainsKey(weapon))
        {
            Debug.LogWarning($"장착하려는 무기를 보유하고 있지 않습니다: {weapon.name}");
            return;
        }

        equippedWeapon = weapon;
        Debug.Log($"무기를 장착했습니다: {weapon.name}");

        // 무기 능력치를 플레이어에게 적용하는 로직은 여기 추가 가능
    }

    /// 현재 장착 무기 반환
    public WeaponSO GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    /// 보유 중인 모든 무기 목록 반환
    public List<WeaponSO> GetAllWeapons()
    {
        return new List<WeaponSO>(weaponCounts.Keys);
    }

    /// 무기 보유 여부 확인
    public bool HasWeapon(WeaponSO weapon)
    {
        return weaponCounts.ContainsKey(weapon);
    }

    /// 특정 무기의 보유 개수 반환
    public int GetWeaponCount(WeaponSO weapon)
    {
        return weaponCounts.TryGetValue(weapon, out int count) ? count : 0;
    }
}
