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
                Debug.LogWarning("WeaponInventory를 찾을 수 없습니다.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (weaponSO == null) return;

        weaponInventory?.AddWeapon(weaponSO);
        Debug.Log($"{weaponSO.weaponName} 습득 완료");

        Destroy(gameObject); // 습득 후 오브젝트 제거
    }
}
