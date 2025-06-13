using UnityEngine;

public class BossSceneWeaponInventory : MonoBehaviour
{
    public GameObject sword01;  // 디멘션 블레이드 
    public GameObject sword02;  // 사공도
    public GameObject gun01;
    public GameObject gun02;

    public bool isSword01 = true;
    public bool isSword02 = true;
    public bool isGun01 = true;
    public bool isGun02 = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && isSword01)
        {
            ActivateWeapon(sword01);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && isSword02)
        {
            ActivateWeapon(sword02);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && isGun01)
        {
            ActivateWeapon(gun01);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && isGun02)
        {
            ActivateWeapon(gun02);
        }
    }

    private void ActivateWeapon(GameObject weaponToActivate)
    {
        sword01.SetActive(weaponToActivate == sword01);
        sword02.SetActive(weaponToActivate == sword02);
        gun01.SetActive(weaponToActivate == gun01);
        gun02.SetActive(weaponToActivate == gun02);
    }


}
