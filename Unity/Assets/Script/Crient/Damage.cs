using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0,90 * Time.deltaTime,0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove playerMove = other.GetComponentInParent<PlayerMove>();

            if (playerMove != null)
            {
                playerMove.Damage(30);
                Debug.Log("현재 체력: " + playerMove.curHealth);
            }
        }
    }
}
