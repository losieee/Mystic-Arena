using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private GameObject currentWeaponObj;
    [SerializeField] private Transform handTransform; // 무기 장착 위치

    public void SetWeapon(WeaponSO weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("weapon is null");
            return;
        }

        if (weapon.weaponPrefab == null)
        {
            Debug.LogWarning("weaponPrefab is null in WeaponSO");
            return;
        }

        if (handTransform == null)
        {
            Debug.LogWarning("handTransform is not assigned");
            return;
        }

        if (currentWeaponObj != null)
            Destroy(currentWeaponObj);

        // weapon 자체가 아니라 weapon.weaponPrefab을 Instantiate해야 함
        currentWeaponObj = Instantiate(weapon.weaponPrefab, handTransform);
        currentWeaponObj.transform.localPosition = Vector3.zero;
        currentWeaponObj.transform.localRotation = Quaternion.identity;
    }

}
