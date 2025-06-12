using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PortalBlock : MonoBehaviour
{
    private GameObject player;
    private TextMeshProUGUI portalText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            player = other.GetComponentInParent<Fight_Demo>().gameObject;

            if (portalText == null)
            {
                Transform potalTextTransform = player.transform.Find("Canvas/PotalBlockText");

                if (potalTextTransform != null)
                {
                    portalText = potalTextTransform.GetComponent<TextMeshProUGUI>();
                }
            }

            // 텍스트 표시
            if (portalText != null)
            {
                portalText.gameObject.SetActive(true);
            }
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            // 텍스트 숨기기
            if (portalText != null)
            {
                portalText.gameObject.SetActive(false);
            }
        }
    }
}
