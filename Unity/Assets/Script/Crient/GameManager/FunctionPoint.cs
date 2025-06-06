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
        // 포탈 앞에서 F 키 입력 시 포탈 활성화
        if (isPlayerNearby && !isPortalActivated && Input.GetKeyDown(KeyCode.F))
        {
            ActivatePortal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if(gameManager.WabeCount == 0)
        {
            isPlayerNearby = true;
            functionText.text = "F  정화하기";
            functionUI.SetActive(true);

            Debug.Log("플레이어가 포탈 앞에 있습니다.");
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

        Debug.Log("포탈이 정화되었습니다.");
    }
}
