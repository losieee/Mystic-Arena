using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFunctionManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject spawnPoint;

    private bool isTransitioningScene = false; // 추가!

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
            Debug.LogWarning("playerPoint를 찾을 수 없습니다.");
        }

        // 씬 로드 후에는 다시 포탈 활성화 가능
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
            Debug.Log("포탈에 충돌");
            if (isTransitioningScene)
            {
                Debug.Log("[PlayerFunctionManager] 씬 전환 중 → 포탈 무시");
                return;
            }

            Debug.Log("포탈과 충돌 → 다음 씬 이동 시작");

            isTransitioningScene = true; // 잠금 처리!

            if (GameManager.instance != null)
            {
                GameManager.instance.LoadNextStage();
            }
            else
            {
                Debug.LogWarning("GameManager 인스턴스를 찾을 수 없습니다.");
            }
        }
    }
}
