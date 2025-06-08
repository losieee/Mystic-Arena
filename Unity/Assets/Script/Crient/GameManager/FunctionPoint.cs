using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class FunctionPoint : MonoBehaviour
{
    public GameManager gameManager;

    [Header("UI Elements")]
    public GameObject functionUI;
    public Text functionText;

    [Header("Portal Objects")]
    public GameObject beforePortal;
    public GameObject glowPortal;
    public GameObject mainPortal;

    private bool isPlayerNearby = false;
    private bool isPortalActivated = false;

    private void Start()
    {
        if (!gameManager)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    private void Update()
    {
        // ��Ż �տ��� F Ű �Է� �� ��Ż Ȱ��ȭ
        if (isPlayerNearby && !isPortalActivated && Input.GetKeyDown(KeyCode.F))
        {
            ActivatePortal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ��Ż ��� ���� ����: currentWave == 0 (Stage 1) �̰ų� StageClear ������ ��
        if (gameManager.currentWave == 0 || gameManager.isStageClear)
        {
            isPlayerNearby = true;
            functionText.text = "F  ��ȭ�ϱ�";
            functionUI.SetActive(true);

            Debug.Log("�÷��̾ ��Ż �տ� �ֽ��ϴ�.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = false;
        functionUI.SetActive(false);
    }

    private void ActivatePortal()
    {
        isPortalActivated = true;

        beforePortal.SetActive(false);
        glowPortal.SetActive(true);
        mainPortal.SetActive(true);

        Debug.Log("��Ż�� ��ȭ�Ǿ����ϴ�.");

        // �ɼ�: ��Ż ��ȭ �� �� �̵� ��ɵ� ���ϸ� �߰� ���� (FadeManager ��� ��)
    }
}
