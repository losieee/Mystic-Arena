using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegerTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Integer"))
        {
            GameObject bossObj = GameObject.FindGameObjectWithTag("Boss");
            if (bossObj != null)
            {
                BossController boss = bossObj.GetComponent<BossController>();
                if (boss != null)
                {
                    boss.TakeDamage(200f);
                }
            }
            Destroy(other.gameObject);
        }
    }
}
