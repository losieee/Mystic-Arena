using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFunctionManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject spawnPoint;

    private bool isTransitioningScene = false; // �߰�!

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        spawnPoint = GameObject.Find("playerPoint");

        if (spawnPoint != null)
        {
            StartCoroutine(MoveToSpawnWithDelay(0.5f));
        }
        else
        {
            Debug.LogWarning("playerPoint�� ã�� �� �����ϴ�.");
        }

        // �� �ε� �Ŀ��� �ٽ� ��Ż Ȱ��ȭ ����
        isTransitioningScene = false;
    }

    private System.Collections.IEnumerator MoveToSpawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.position = spawnPoint.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            Debug.Log("��Ż�� �浹");
            if (isTransitioningScene)
            {
                Debug.Log("[PlayerFunctionManager] �� ��ȯ �� �� ��Ż ����");
                return;
            }

            Debug.Log("��Ż�� �浹 �� ���� �� �̵� ����");

            isTransitioningScene = true; // ��� ó��!

            if (GameManager.instance != null)
            {
                GameManager.instance.LoadNextStage();
            }
            else
            {
                Debug.LogWarning("GameManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            }
        }
    }
}
