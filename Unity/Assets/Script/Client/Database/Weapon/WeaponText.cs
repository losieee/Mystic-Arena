using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponText : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponentInParent<Fight_Demo>();
            player.WeaponText();
        }
    }
}
