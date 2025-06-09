using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
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
                    GameObject audioObj = new GameObject("TempAudio_PickupSound");
                    audioObj.transform.position = transform.position;

                    AudioSource audioSource = audioObj.AddComponent<AudioSource>();
                    audioSource.clip = pickupSound;
                    audioSource.volume = 0.05f;
                    audioSource.loop = false;
                    audioSource.Play();

                    Destroy(audioObj, 2f);
                }

                Destroy(effect, 1f);
            }

            Destroy(other.gameObject);
        }
    }
}
