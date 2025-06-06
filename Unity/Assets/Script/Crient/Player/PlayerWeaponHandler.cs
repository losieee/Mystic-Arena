using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private GameObject currentWeaponObj;

    public void SetWeapon(WeaponSO weapon)
    {
        // 기존 무기 제거 등 처리
        if (currentWeaponObj != null)
            Destroy(currentWeaponObj);

        // 무기 프리팹을 생성하고 손 위치에 붙이기 (예)
        currentWeaponObj = Instantiate(weapon.weaponPrefab, transform);

        // 추가 무기 세팅 로직
    }
}
