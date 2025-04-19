using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public GameObject aiPrefab;
    private int aiSpawnCount = 0;

    private readonly string[] aiSpawnPoints = {
        "RespawnPoint_02",
        "RespawnPoint_03",
        "RespawnPoint_04"
    };

    public void SpawnAI()
    {
        if (aiSpawnCount >= aiSpawnPoints.Length)
        {
            Debug.Log("AI 캐릭터는 최대 3명까지만 추가할 수 있습니다.");
            return;
        }

        string pointName = aiSpawnPoints[aiSpawnCount];
        GameObject spawnPoint = GameObject.Find(pointName);

        if (spawnPoint != null && aiPrefab != null)
        {
            Instantiate(aiPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            aiSpawnCount++;
        }
        else
        {
            Debug.LogWarning("AI 스폰 위치 또는 프리팹이 없습니다.");
        }
    }
}