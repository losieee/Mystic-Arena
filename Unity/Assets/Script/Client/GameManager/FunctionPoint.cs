using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq; // FirstOrDefault ���

public class FunctionPoint : MonoBehaviour
{
    public GameManager gameManager;

    [Header("UI Elements")]
    public TextMeshProUGUI functionText;

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

        // ���� �ѹ��� �ʱ�ȭ (OnEnable���� �ߺ� ����)
        StartCoroutine(DelayedFindPortalObjects());
    }

    // OnEnable������ DelayedFindPortalObjects ȣ�� ���� (GameManager�� ���� ȣ��)
    /*
    private void OnEnable()
    {
        StartCoroutine(DelayedFindPortalObjects());
    }
    */

    private void Update()
    {
        if (isPlayerNearby && !isPortalActivated && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F Ű �Է� ���� �� ActivatePortal() ȣ�� �õ�");
            ActivatePortal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("�÷��̾� ���� (OnTriggerEnter)");

        if (gameManager != null)
        {
            Debug.Log($"isStageClear ���� Ȯ��: {gameManager.isStageClear}");

            if (gameManager.isStageClear)
            {
                isPlayerNearby = true;
                functionText.text = "F  ��ȭ�ϱ�";
                functionText.gameObject.SetActive(true);

                Debug.Log("�÷��̾ ��Ż �տ� �ֽ��ϴ�. F Ű �Է� �����.");
            }
            else
            {
                Debug.Log("�������� Ŭ���� ���°� �ƴ� �� ��Ż ���ͷ��� �Ұ�");
            }
        }
        else
        {
            Debug.LogError("GameManager ������ �����ϴ�!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = false;
        functionText.gameObject.SetActive(false);
    }

    private void ActivatePortal()
    {
        isPortalActivated = true;

        if (beforePortal == null || glowPortal == null || mainPortal == null)
        {
            Debug.LogError("��Ż ������Ʈ�� ������� �ʾҽ��ϴ�!");
            return;
        }

        Debug.Log("��Ż�� ��ȭ�˴ϴ�... ������Ʈ ���� ���� �õ�");

        beforePortal.SetActive(false);
        glowPortal.SetActive(true);
        mainPortal.SetActive(true);
        functionText.gameObject.SetActive(false);

        Debug.Log("��Ż ��ȭ �Ϸ� - ������Ʈ ���� ���� �Ϸ�");
    }

    private void InitializePortalState()
    {
        // �� ���� �� ��Ż ���� �ʱ�ȭ
        isPortalActivated = false;
        isPlayerNearby = false;

        if (beforePortal != null) beforePortal.SetActive(true);
        if (glowPortal != null) glowPortal.SetActive(false);
        if (mainPortal != null) mainPortal.SetActive(false);

        if (functionText != null)
        {
            functionText.gameObject.SetActive(false);
        }

        Debug.Log("��Ż ���� �ʱ�ȭ �Ϸ�");
    }

    private IEnumerator DelayedFindPortalObjects()
    {
        yield return new WaitForSeconds(0.1f); // �� �ε� �� �������� ���

        FindPortalObjectsByName();
        InitializePortalState();
    }

    private void FindPortalObjectsByName()
    {
        beforePortal = FindInactiveObjectByName("NextPortalBlue");
        glowPortal = FindInactiveObjectByName("PortalGrin");
        mainPortal = FindInactiveObjectByName("Potal");

        if (beforePortal == null) Debug.LogWarning("BeforePortal (NextPortalBlue) ������Ʈ�� ã�� �� �����ϴ�.");
        if (glowPortal == null) Debug.LogWarning("GlowPortal (PortalGrin) ������Ʈ�� ã�� �� �����ϴ�.");
        if (mainPortal == null) Debug.LogWarning("MainPortal (Potal) ������Ʈ�� ã�� �� �����ϴ�.");

        Debug.Log("��Ż ������Ʈ ã�� �Ϸ� (��Ȱ��ȭ ����)");
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == name);
    }

    // ? �ܺο��� ȣ��� ���� �޼���
    public void InitializePortalFromOutside()
    {
        Debug.Log("�ܺ� ȣ�� �� ��Ż ���� �ʱ�ȭ ��û��");

        FindPortalObjectsByName();
        InitializePortalState();
    }
}
