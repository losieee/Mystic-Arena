using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    // 보유한 모든 무기 리스트
    private List<WeaponSO> ownedWeapons = new List<WeaponSO>();

    // 현재 장착된 무기 (옵션)
    private WeaponSO equippedWeapon;

    /// 무기를 인벤토리에 추가합니다 (중복 방지 가능)
    public void AddWeapon(WeaponSO weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("추가하려는 무기가 null입니다.");
            return;
        }

        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
            Debug.Log($"무기를 획득했습니다: {weapon.name}");
        }
        else
        {
            Debug.Log($"이미 보유 중인 무기입니다: {weapon.name}");
        }
    }

    /// 현재 장착 무기 설정
    public void EquipWeapon(WeaponSO weapon)
    {
        if (!ownedWeapons.Contains(weapon))
        {
            Debug.LogWarning($"장착하려는 무기를 보유하고 있지 않습니다: {weapon.name}");
            return;
        }

        equippedWeapon = weapon;
        Debug.Log($"무기를 장착했습니다: {weapon.name}");

        // 필요 시, 무기 능력치를 플레이어에게 적용
        // 예: player.SetDamage(weapon.baseDamage);
    }

    /// 현재 장착 무기 반환
    public WeaponSO GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    /// 보유한 모든 무기 반환 (복사본)
    public List<WeaponSO> GetAllWeapons()
    {
        return new List<WeaponSO>(ownedWeapons);
    }


    /// 무기 보유 여부 확인
    public bool HasWeapon(WeaponSO weapon)
    {
        return ownedWeapons.Contains(weapon);
    }
}

