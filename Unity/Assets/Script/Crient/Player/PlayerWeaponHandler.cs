using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private GameObject currentWeaponObj;

    public void SetWeapon(WeaponSO weapon)
    {
        // ���� ���� ���� �� ó��
        if (currentWeaponObj != null)
            Destroy(currentWeaponObj);

        // ���� �������� �����ϰ� �� ��ġ�� ���̱� (��)
        currentWeaponObj = Instantiate(weapon.weaponPrefab, transform);

        // �߰� ���� ���� ����
    }
}
