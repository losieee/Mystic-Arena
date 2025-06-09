using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private GameObject currentWeaponObj;
    [SerializeField] private Transform handTransform; // ���� ���� ��ġ

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

        // weapon ��ü�� �ƴ϶� weapon.weaponPrefab�� Instantiate�ؾ� ��
        currentWeaponObj = Instantiate(weapon.weaponPrefab, handTransform);
        currentWeaponObj.transform.localPosition = Vector3.zero;
        currentWeaponObj.transform.localRotation = Quaternion.identity;
    }

}
