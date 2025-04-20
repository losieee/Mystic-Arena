using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject aiPrefab; // AI 프리팹 연결

    private readonly string[] aiSpawnPoints = {
        "RespawnPoint_02",
        "RespawnPoint_03",
        "RespawnPoint_04"
    };

    void Start()
    {
        // 플레이어는 항상 RespawnPoint_01
        if (ButtonManager.Instance != null && ButtonManager.Instance.selectedPrefab != null)
        {
            GameObject playerPoint = GameObject.Find("RespawnPoint_01");
            if (playerPoint != null)
            {
                Instantiate(ButtonManager.Instance.selectedPrefab,
                            playerPoint.transform.position,
                            playerPoint.transform.rotation);
                Debug.Log("플레이어 소환 완료 @ RespawnPoint_01");
            }
        }

        // AI는 최대 3명까지, 각각 포인트에 1명만
        int aiCount = PlayerPrefs.GetInt("AICount", 0);
        Debug.Log($"AI 소환 수: {aiCount}");

        for (int i = 0; i < aiCount && i < aiSpawnPoints.Length; i++)
        {
            GameObject aiPoint = GameObject.Find(aiSpawnPoints[i]);

            if (aiPoint != null && aiPrefab != null)
            {
                Instantiate(aiPrefab,
                            aiPoint.transform.position,
                            aiPoint.transform.rotation);
                Debug.Log($"AI {i + 1} 소환 완료 @ {aiSpawnPoints[i]}");
            }
            else
            {
                Debug.LogWarning($"스폰 위치 '{aiSpawnPoints[i]}' 또는 프리팹이 없습니다.");
            }
        }
    }
}