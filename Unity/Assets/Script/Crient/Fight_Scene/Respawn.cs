using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    void Start()
    {
        if (ButtonManager.Instance != null && ButtonManager.Instance.selectedPrefab != null)
        {
            GameObject spawnPoint = GameObject.Find("RespawnPoint_01"); // 이름으로 찾기

            Vector3 spawnPos = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
            Quaternion spawnRot = spawnPoint != null ? spawnPoint.transform.rotation : Quaternion.identity;

            Instantiate(ButtonManager.Instance.selectedPrefab, spawnPos, spawnRot);
        }
    }
}
