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
        // 새로운 씬에서 스폰 포인트 재탐색
        spawnPoint = GameObject.Find("playerPoint");

        if (spawnPoint != null)
        {
            StartCoroutine(MoveToSpawnWithDelay(0.5f)); // 0.5초 후 이동 (페이드 타이밍 맞추기)
        }
        else
        {
            Debug.LogWarning("playerPoint를 찾을 수 없습니다.");
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
                Debug.LogWarning("다음 씬 이름을 찾을 수 없습니다.");
            }
        }
    }
}
