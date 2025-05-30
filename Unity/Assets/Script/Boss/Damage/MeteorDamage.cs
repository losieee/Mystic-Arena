using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Fight_Demo player = other.GetComponentInParent<Fight_Demo>();
            if (player != null)
            {
                player.TakeDamage(10f);
            }
        }
    }
}
