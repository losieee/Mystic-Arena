using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject aiPrefab; // AI ������ ����

    private readonly string[] aiSpawnPoints = {
        "RespawnPoint_02",
        "RespawnPoint_03",
        "RespawnPoint_04"
    };

    void Start()
    {
        // �÷��̾�� �׻� RespawnPoint_01
        if (ButtonManager.Instance != null && ButtonManager.Instance.selectedPrefab != null)
        {
            GameObject playerPoint = GameObject.Find("RespawnPoint_01");
            if (playerPoint != null)
            {
                Instantiate(ButtonManager.Instance.selectedPrefab,
                            playerPoint.transform.position,
                            playerPoint.transform.rotation);
                Debug.Log("�÷��̾� ��ȯ �Ϸ� @ RespawnPoint_01");
            }
        }

        // AI�� �ִ� 3�����, ���� ����Ʈ�� 1��
        int aiCount = PlayerPrefs.GetInt("AICount", 0);
        Debug.Log($"AI ��ȯ ��: {aiCount}");

        for (int i = 0; i < aiCount && i < aiSpawnPoints.Length; i++)
        {
            GameObject aiPoint = GameObject.Find(aiSpawnPoints[i]);

            if (aiPoint != null && aiPrefab != null)
            {
                Instantiate(aiPrefab,
                            aiPoint.transform.position,
                            aiPoint.transform.rotation);
                Debug.Log($"AI {i + 1} ��ȯ �Ϸ� @ {aiSpawnPoints[i]}");
            }
            else
            {
                Debug.LogWarning($"���� ��ġ '{aiSpawnPoints[i]}' �Ǵ� �������� �����ϴ�.");
            }
        }
    }
}