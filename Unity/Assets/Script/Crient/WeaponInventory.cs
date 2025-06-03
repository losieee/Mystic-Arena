using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;


public class WeaponInventory : MonoBehaviour
{
    public WeaponData weaponData;
    public int sword01Count = 0;
    public int sword02Count = 0;
    public int Gun01Count = 0;
    public int Gun02Count = 0;

    private void Iventory()
    {
        switch (weaponData.weaponType)
        {
            case WeaponType.Sword01:
                sword01Count++;
                break;
            case WeaponType.Sword02:
                sword02Count++;
                break;
            case WeaponType.Gun01:
                Gun01Count++;
                break;
            case WeaponType.Gun02:
                Gun02Count++;
                break;
        }
    }


}
