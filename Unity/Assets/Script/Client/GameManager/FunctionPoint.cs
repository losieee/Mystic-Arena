using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class FunctionPoint : MonoBehaviour
{
    public Fight_Demo Fight_Demo;
    public PlayerSO playerSO;
    public GameManager gameManager;

    [Header("UI Elements")]
    public TextMeshProUGUI functionText;

    [Header("Portal Objects")]
    public GameObject beforePortal;
    public GameObject glowPortal;
    public GameObject mainPortal;

    private bool isPlayerNearby = false;
    private bool isPortalActivated = false;
    private bool isSceneTransitioning = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!gameManager)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedFindPortalObjects());
    }

    private void Update()
    {
        if (isPlayerNearby && !isPortalActivated && Input.GetKeyDown(KeyCode.F))
        {
            Fight_Demo.player_currHp = playerSO.playerCurrHp;
            Debug.Log("F Ű �Է� ���� �� ��Ż ��ȭ �� �� ��ȯ �õ�");
            StartCoroutine(ActivatePortalAndMoveToNextScene());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gameManager != null && gameManager.isStageClear)
        {
            isPlayerNearby = true;
            functionText.text = "F  ��ȭ�ϱ�";
            functionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = false;
        functionText.gameObject.SetActive(false);
    }

    private IEnumerator ActivatePortalAndMoveToNextScene()
    {
        isPortalActivated = true;

        // ��Ż ��ȭ ����Ʈ
        if (beforePortal != null) beforePortal.SetActive(false);
        if (glowPortal != null) glowPortal.SetActive(true);
        if (mainPortal != null) mainPortal.SetActive(true);
        if (functionText != null) functionText.gameObject.SetActive(false);

        Debug.Log("��Ż ��ȭ �Ϸ� - �� ��ȯ ��� ��");

        yield return new WaitForSeconds(1.5f);

        if (!isSceneTransitioning && gameManager != null)
        {
            isSceneTransitioning = true;
            gameManager.LoadNextStage(); // GameManager ���� ���� �� �ε�
        }
    }

    private void InitializePortalState()
    {
        isPortalActivated = false;
        isPlayerNearby = false;
        isSceneTransitioning = false;

        if (beforePortal != null) beforePortal.SetActive(true);
        if (glowPortal != null) glowPortal.SetActive(false);
        if (mainPortal != null) mainPortal.SetActive(false);

        if (functionText != null)
            functionText.gameObject.SetActive(false);
    }

    private IEnumerator DelayedFindPortalObjects()
    {
        yield return new WaitForSeconds(0.1f);
        FindPortalObjectsByName();
        InitializePortalState();
    }

    private void FindPortalObjectsByName()
    {
        beforePortal = FindInactiveObjectByName("NextPortalBlue");
        glowPortal = FindInactiveObjectByName("PortalGrin");

        if (beforePortal == null) Debug.LogWarning("NextPortalBlue ã�� �� ����");
        if (glowPortal == null) Debug.LogWarning("PortalGrin ã�� �� ����");
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == name);
    }

    public void InitializePortalFromOutside()
    {
        Debug.Log("�ܺ� ȣ��� ��Ż �ʱ�ȭ");
        FindPortalObjectsByName();
        InitializePortalState();
    }
}
