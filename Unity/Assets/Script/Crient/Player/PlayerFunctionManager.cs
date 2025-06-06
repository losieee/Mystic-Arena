using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFunctionManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject spawnPoint;

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
        // ���ο� ������ ���� ����Ʈ ��Ž��
        spawnPoint = GameObject.Find("playerPoint");

        if (spawnPoint != null)
        {
            StartCoroutine(MoveToSpawnWithDelay(0.5f)); // 0.5�� �� �̵� (���̵� Ÿ�̹� ���߱�)
        }
        else
        {
            Debug.LogWarning("playerPoint�� ã�� �� �����ϴ�.");
        }
    }

    private System.Collections.IEnumerator MoveToSpawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.position = spawnPoint.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potal"))
        {
            string nextScene = SceneSequenceManager.Instance?.GetNextScene();

            if (!string.IsNullOrEmpty(nextScene))
            {
                if (FadeManager.Instance != null)
                {
                    FadeManager.Instance.LoadSceneWithFade(nextScene);
                }
                else
                {
                    SceneManager.LoadScene(nextScene);
                }
            }
            else
            {
                Debug.LogWarning("���� �� �̸��� ã�� �� �����ϴ�.");
            }
        }
    }
}
