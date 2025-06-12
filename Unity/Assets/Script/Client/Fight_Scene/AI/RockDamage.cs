using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            Fight_Demo playerComponent = other.GetComponentInParent<Fight_Demo>();
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(15f);

                Destroy(gameObject);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Rock" || collision.gameObject.tag == "vicinityEnemy")
        {
            Destroy(gameObject);
        }
    }
}
