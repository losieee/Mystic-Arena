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
    public AudioClip potalIn;

    [Header("UI Elements")]
    public TextMeshProUGUI functionText;

    [Header("Portal Objects")]
    public GameObject beforePortal;
    public GameObject glowPortal;

    private bool isPlayerNearby = false;
    private bool isPortalActivated = false;
    private bool isSceneTransitioning = false;

    private AudioSource audioSource;
    private int portalCloseCount = 0;
    private const int portalCloseThreshold = 3;
    private const int requiredCloseCount = 3;

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
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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
            StartCoroutine(ActivatePortalAndMoveToNextScene());
        }

        CheckAndOpenPortal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gameManager != null && gameManager.isStageClear && portalCloseCount >= requiredCloseCount)
        {
            isPlayerNearby = true;
            functionText.text = "F�� ����\n���� ���������� �̵�";
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
        if (functionText != null) functionText.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        if (!isSceneTransitioning && gameManager != null)
        {
            isSceneTransitioning = true;
            if (potalIn != null)
            {
                audioSource.clip = potalIn;
                audioSource.volume = 0.1f;
                audioSource.Play();
            }
            gameManager.LoadNextStage();
        }
    }

    public void AddPortalCloseCount()
    {
        portalCloseCount++;
        Debug.Log($"[FunctionPoint] ��Ż ���� ��: {portalCloseCount}");

        CheckAndOpenPortal();
    }

    private void InitializePortalState()
    {
        isPortalActivated = false;
        isPlayerNearby = false;
        isSceneTransitioning = false;

        portalCloseCount = 0;

        if (beforePortal != null) beforePortal.SetActive(true);
        if (glowPortal != null) glowPortal.SetActive(false);

        if (functionText != null)
            functionText.gameObject.SetActive(false);
    }
    private void CheckAndOpenPortal()
    {
        if (gameManager != null && gameManager.isStageClear && portalCloseCount >= portalCloseThreshold)
        {
            if (glowPortal != null && !glowPortal.activeSelf)
            {
                glowPortal.SetActive(true);
                Debug.Log("[FunctionPoint] ��� ���� ���� �� glowPortal Ȱ��ȭ");
            }

            if (beforePortal != null && beforePortal.activeSelf)
            {
                beforePortal.SetActive(false);
            }
        }
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