using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerHitbox")
        {
            Fight_Demo player = other.GetComponentInParent<Fight_Demo>();
            if (player != null)
            {
                player.TakeDamage(120f);
            }
        }
    }
}
