using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WeaponInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Transform weaponListContainer;
    [SerializeField] private GameObject weaponButtonPrefab;

    private WeaponInventory inventory;

    public void Initialize(WeaponInventory inv)
    {
        inventory = inv;
        RefreshUI();
        uiPanel.SetActive(false);
    }

    public void ToggleUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(!uiPanel.activeSelf);
        }
    }

    public void RefreshUI()
    {
        foreach (Transform child in weaponListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (WeaponSO weapon in inventory.GetAllWeapons())
        {
            GameObject btnObj = Instantiate(weaponButtonPrefab, weaponListContainer);
            btnObj.GetComponentInChildren<TMP_Text>().text = $"{weapon.name} x{inventory.GetWeaponCount(weapon)}";
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                inventory.EquipWeapon(weapon);
                ToggleUI();
            });
        }
    }
}
