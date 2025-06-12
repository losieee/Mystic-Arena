using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Death : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject respawnBackText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("RespawnButton"))
        {
            if (respawnBackText != null)
                respawnBackText.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("RespawnButton"))
        {
            if (respawnBackText != null)
                respawnBackText.SetActive(false);
        }
    }
}
