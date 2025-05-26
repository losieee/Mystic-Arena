//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class Damage : MonoBehaviour
//{
//    void Update()
//    {
//        transform.Rotate(0,90 * Time.deltaTime,0);
//    }
//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            KnightMove playerMove = other.GetComponentInParent<KnightMove>();

//            if (playerMove != null)
//            {
//                playerMove.Damage(30);
//            }
//        }
//    }
//}
