using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField]
    private List<WeaponSO> weaponSOList; // �����Ϳ��� �׽�Ʈ�� ���� ����Ʈ

    private Dictionary<WeaponSO, int> weaponCounts = new Dictionary<WeaponSO, int>();

    private WeaponSO equippedWeapon;

    [Header("UI �� ���� �ڵ鸵")]
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
            Debug.LogWarning("�������� ���� �����Դϴ�.");
            return;
        }

        equippedWeapon = weapon;
        weaponHandler?.SetWeapon(weapon); // ���� ���� ���� ó��
        Debug.Log($"{weapon.weaponName} ���� �Ϸ�");
    }

    public WeaponSO GetEquippedWeapon() => equippedWeapon;
    public List<WeaponSO> GetAllWeapons() => new List<WeaponSO>(weaponCounts.Keys);
    public bool HasWeapon(WeaponSO weapon) => weaponCounts.ContainsKey(weapon);
    public int GetWeaponCount(WeaponSO weapon) => weaponCounts.TryGetValue(weapon, out int count) ? count : 0;
}
