using UnityEngine;

public class WeaponDropManager : MonoBehaviour
{
    public WeaponSO weaponSO;
    public WeaponInventory weaponInventory;

    private void Awake()
    {
        if (weaponInventory == null)
        {
            weaponInventory = FindObjectOfType<WeaponInventory>();
            if (weaponInventory == null)
            {
                Debug.LogWarning("WeaponInventory�� ã�� �� �����ϴ�.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (weaponSO == null) return;

        weaponInventory?.AddWeapon(weaponSO);
        Debug.Log($"{weaponSO.weaponName} ���� �Ϸ�");

        Destroy(gameObject); // ���� �� ������Ʈ ����
    }
}
