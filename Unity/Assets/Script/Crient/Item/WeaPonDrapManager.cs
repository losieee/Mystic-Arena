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
//        Debug.Log("�÷��̾ �ҵ带 ȹ���߽��ϴ�.");
//        Item_Handler();
//    }

//    private void ItemType_Gun()
//    {
//        Debug.Log("�÷��̾ ���� ȹ���߽��ϴ�.");
//        Item_Handler();
//    }

//}
