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
            Debug.LogError("[Portal] GameManager�� ã�� �� �����ϴ�!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log("[Portal] Player ��Ż �浹 ����");

        if (gameManager == null)
        {
            Debug.LogError("[Portal] GameManager�� ���� - ó�� �ߴ�");
            return;
        }

        if (IsStageClear())
        {
            Debug.Log("[Portal] ���� ���� �� ���� ������ �̵�");

            isTriggered = true;
            gameManager.LoadNextStage();
        }
        else
        {
            Debug.Log("[Portal] ���� ����� �� ��Ż ��� �Ұ� (isStageClear=false)");
        }
    }

    private bool IsStageClear()
    {
        return gameManager != null && gameManager.isStageClear;
    }
}
