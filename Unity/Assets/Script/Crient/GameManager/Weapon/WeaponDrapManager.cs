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

        weaponInventory.AddWeapon(weaponSO);  // WeaponSO 타입 변수 전달
        Debug.Log($"{weaponSO.weaponName} 습득 완료");

        Destroy(gameObject);
    }
}
