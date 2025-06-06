using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField]
    private List<WeaponSO> weaponSOList; // 에디터에서 테스트용 무기 리스트

    private Dictionary<WeaponSO, int> weaponCounts = new Dictionary<WeaponSO, int>();

    private WeaponSO equippedWeapon;

    [Header("UI 및 무기 핸들링")]
    [SerializeField] private WeaponInventoryUI inventoryUI;
    [SerializeField] private PlayerWeaponHandler weaponHandler;

    public IReadOnlyDictionary<WeaponSO, int> WeaponCounts => weaponCounts;

    private void Start()
    {
        foreach (var weapon in weaponSOList)
        {
            AddWeapon(weapon);
        }

        if (inventoryUI != null)
            inventoryUI.Initialize(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryUI?.ToggleUI();
        }
    }

    public void AddWeapon(WeaponSO weapon)
    {
        if (weapon == null) return;

        if (weaponCounts.ContainsKey(weapon))
            weaponCounts[weapon]++;
        else
            weaponCounts[weapon] = 1;

        inventoryUI?.RefreshUI();
    }

    public void EquipWeapon(WeaponSO weapon)
    {
        if (!weaponCounts.ContainsKey(weapon))
        {
            Debug.LogWarning("보유하지 않은 무기입니다.");
            return;
        }

        equippedWeapon = weapon;
        weaponHandler?.SetWeapon(weapon); // 실제 무기 장착 처리
        Debug.Log($"{weapon.weaponName} 장착 완료");
    }

    public WeaponSO GetEquippedWeapon() => equippedWeapon;
    public List<WeaponSO> GetAllWeapons() => new List<WeaponSO>(weaponCounts.Keys);
    public bool HasWeapon(WeaponSO weapon) => weaponCounts.ContainsKey(weapon);
    public int GetWeaponCount(WeaponSO weapon) => weaponCounts.TryGetValue(weapon, out int count) ? count : 0;
}
