using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public GameObject sword01;  // µð¸à¼Ç ºí·¹ÀÌµå 
    public GameObject sword02;  // »ç°øµµ
    public GameObject gun01;
    public GameObject gun02;

    public bool isSword01 = true;
    public bool isSword02;
    public bool isGun01;
    public bool isGun02;

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


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("sword02"))
        {
            isSword02 = true;
            Debug.Log("°Ë È¹µæ");
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("gun01"))
        {
            isGun01 = true;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("gun02"))
        {
            isGun02 = true;
            Destroy(other.gameObject);
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
