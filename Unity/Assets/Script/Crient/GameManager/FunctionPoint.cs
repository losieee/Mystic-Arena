using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq; // FirstOrDefault 사용

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

        // 최초 한번만 초기화 (OnEnable에서 중복 제거)
        StartCoroutine(DelayedFindPortalObjects());
    }

    // OnEnable에서는 DelayedFindPortalObjects 호출 제거 (GameManager가 직접 호출)
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
            Debug.Log("F 키 입력 감지 → ActivatePortal() 호출 시도");
            ActivatePortal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("플레이어 감지 (OnTriggerEnter)");

        if (gameManager != null)
        {
            Debug.Log($"isStageClear 상태 확인: {gameManager.isStageClear}");

            if (gameManager.isStageClear)
            {
                isPlayerNearby = true;
                functionText.text = "F  정화하기";
                functionText.gameObject.SetActive(true);

                Debug.Log("플레이어가 포탈 앞에 있습니다. F 키 입력 대기중.");
            }
            else
            {
                Debug.Log("스테이지 클리어 상태가 아님 → 포탈 인터랙션 불가");
            }
        }
        else
        {
            Debug.LogError("GameManager 참조가 없습니다!");
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
            Debug.LogError("포탈 오브젝트가 연결되지 않았습니다!");
            return;
        }

        Debug.Log("포탈이 정화됩니다... 오브젝트 상태 변경 시도");

        beforePortal.SetActive(false);
        glowPortal.SetActive(true);
        mainPortal.SetActive(true);
        functionText.gameObject.SetActive(false);

        Debug.Log("포탈 정화 완료 - 오브젝트 상태 변경 완료");
    }

    private void InitializePortalState()
    {
        // 씬 시작 시 포탈 상태 초기화
        isPortalActivated = false;
        isPlayerNearby = false;

        if (beforePortal != null) beforePortal.SetActive(true);
        if (glowPortal != null) glowPortal.SetActive(false);
        if (mainPortal != null) mainPortal.SetActive(false);

        if (functionText != null)
        {
            functionText.gameObject.SetActive(false);
        }

        Debug.Log("포탈 상태 초기화 완료");
    }

    private IEnumerator DelayedFindPortalObjects()
    {
        yield return new WaitForSeconds(0.1f); // 씬 로드 후 한프레임 대기

        FindPortalObjectsByName();
        InitializePortalState();
    }

    private void FindPortalObjectsByName()
    {
        beforePortal = FindInactiveObjectByName("NextPortalBlue");
        glowPortal = FindInactiveObjectByName("PortalGrin");
        mainPortal = FindInactiveObjectByName("Potal");

        if (beforePortal == null) Debug.LogWarning("BeforePortal (NextPortalBlue) 오브젝트를 찾을 수 없습니다.");
        if (glowPortal == null) Debug.LogWarning("GlowPortal (PortalGrin) 오브젝트를 찾을 수 없습니다.");
        if (mainPortal == null) Debug.LogWarning("MainPortal (Potal) 오브젝트를 찾을 수 없습니다.");

        Debug.Log("포탈 오브젝트 찾기 완료 (비활성화 포함)");
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        return Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == name);
    }

    // ? 외부에서 호출용 공개 메서드
    public void InitializePortalFromOutside()
    {
        Debug.Log("외부 호출 → 포탈 상태 초기화 요청됨");

        FindPortalObjectsByName();
        InitializePortalState();
    }
}
