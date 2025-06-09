using UnityEngine;

public class WeaponDropManager : MonoBehaviour
{
    public WeaponSO weaponSO;
    public WeaponInventory weaponInventory;

    private void Awake()
    {
        if (weaponInventory == null)
            weaponInventory = FindObjectOfType<WeaponInventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (weaponSO == null) return;

        weaponInventory.AddWeapon(weaponSO);  // WeaponSO Ÿ�� ���� ����
        Debug.Log($"{weaponSO.weaponName} ���� �Ϸ�");

        Destroy(gameObject);
    }
}
