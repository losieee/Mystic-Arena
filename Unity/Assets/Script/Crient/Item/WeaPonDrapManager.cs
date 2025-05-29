//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WeaPonDrapManager : MonoBehaviour
//{
//    public WeaponSO weaponSO;

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Player"))
//        {
//            switch (weaponSO.weapon_type)
//            {
//                case weapon_type.Sword:
//                    ItemType_Sword();
//                    break;
//                case weapon_type.Gun:
//                    ItemType_Gun();
//                    break;
//                default:
//                    break;
//            }
//        }
//    }

//    private void Item_Handler()
//    {
//        Destroy(gameObject);


//    }

//    private void ItemType_Sword()
//    {
//        Debug.Log("플레이어가 소드를 획득했습니다.");
//        Item_Handler();
//    }

//    private void ItemType_Gun()
//    {
//        Debug.Log("플레이어가 총을 획득했습니다.");
//        Item_Handler();
//    }

//}
