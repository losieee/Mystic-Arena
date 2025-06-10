using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeteorDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerHitbox")
        {
            Fight_Demo playerComponent = other.GetComponentInParent<Fight_Demo>();
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(70f);
            }
        }
    }

}