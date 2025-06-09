using UnityEngine;

public class Portal : MonoBehaviour
{
    private GameManager gameManager;
    private bool isTriggered = false;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("[Portal] GameManager를 찾을 수 없습니다!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log("[Portal] Player 포탈 충돌 감지");

        if (gameManager == null)
        {
            Debug.LogError("[Portal] GameManager가 없음 - 처리 중단");
            return;
        }

        if (IsStageClear())
        {
            Debug.Log("[Portal] 조건 만족 → 다음 씬으로 이동");

            isTriggered = true;
            gameManager.LoadNextStage();
        }
        else
        {
            Debug.Log("[Portal] 조건 불충분 → 포탈 통과 불가 (isStageClear=false)");
        }
    }

    private bool IsStageClear()
    {
        return gameManager != null && gameManager.isStageClear;
    }
}
