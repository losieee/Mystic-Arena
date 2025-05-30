using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Integer : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
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
            Destroy(gameObject);
        }
    }
}
