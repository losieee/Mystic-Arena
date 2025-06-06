using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegerTouch : MonoBehaviour
{
    public GameObject touchEffect;
    public AudioClip pickupSound;
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
                    boss.TakeDamage(2000f);
                }
            }

            if (touchEffect != null)
            {
                Vector3 spawnPos = other.transform.position;
                Quaternion spawnRot = Quaternion.identity;

                GameObject effect = Instantiate(touchEffect, spawnPos, spawnRot);

                if (pickupSound != null)
                {
                    AudioSource audioSource = GetComponent<AudioSource>();
                    if (audioSource == null)
                        audioSource = gameObject.AddComponent<AudioSource>();

                    audioSource.PlayOneShot(pickupSound, 0.1f);
                }

                Destroy(effect, 1f);
            }

            Destroy(other.gameObject);
        }
    }
}
