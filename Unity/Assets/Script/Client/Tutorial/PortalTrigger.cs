using UnityEngine;
using System.Collections;
using TMPro;

public class PortalTrigger : MonoBehaviour
{
    public AudioClip purifiedWork;
    public AudioClip portalClose;
    private bool playerInRange = false;
    private GameObject player;
    private Fight_Demo playerController;
    private bool playerInitialized = false;

    private bool destroyAfterShrink = false;
    private bool isProcessing = false;

    private TextMeshProUGUI portalText;
    private AudioSource audioSource;

    public int potalCloseCount;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            playerInRange = true;

            player = other.GetComponentInParent<Fight_Demo>().gameObject;

            if (portalText == null)
            {
                Transform potalTextTransform = player.transform.Find("Canvas/PotalText");

                if (potalTextTransform != null)
                {
                    portalText = potalTextTransform.GetComponent<TextMeshProUGUI>();
                }
            }

            // 텍스트 표시
            if (portalText != null)
            {
                portalText.gameObject.SetActive(true);
            }

            // PlayerController 연결
            playerController = other.GetComponentInParent<Fight_Demo>();

            if (playerController != null)
            {
                playerInitialized = true;
            }
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            playerInRange = false;

            // 텍스트 숨기기
            if (portalText != null)
            {
                portalText.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        // F 키 입력 처리
        if (playerInitialized && playerInRange && !isProcessing && Input.GetKeyDown(KeyCode.F))
        {
            potalCloseCount++;
            if (portalText != null)
            {
                portalText.gameObject.SetActive(false);
            }

            LookAtPortal();

            playerController.animator.SetTrigger("isWorking");
            playerController.canMove = false;
            playerController.isWorking = true;

            // purifiedWork 사운드 출력
            if (purifiedWork != null)
            {
                StartCoroutine(PlayPurifiedWorkTwice());
            }

            StartCoroutine(ShrinkPortal());
        }
        // Player 초기화 안 됐으면 → 한번만 찾기
        if (!playerInitialized)
        {
            player = GameObject.FindWithTag("PlayerHitbox");
            if (player != null)
            {
                playerController = player.GetComponentInParent<Fight_Demo>();

                if (playerController != null)
                {
                    playerInitialized = true;
                }
            }
        }
    }
    private IEnumerator PlayPurifiedWorkTwice()
    {
        audioSource.pitch = 0.8f;
        audioSource.volume = 0.1f;

        audioSource.PlayOneShot(purifiedWork);

        // purifiedWork 길이만큼 대기
        yield return new WaitForSeconds(2.5f);

        // 다시 재생
        audioSource.PlayOneShot(purifiedWork);
    }

    private IEnumerator ShrinkPortal()
    {
        isProcessing = true;

        yield return new WaitForSeconds(0.05f);

        AnimatorStateInfo stateInfo = playerController.animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Working"))
        {
            yield return null;
            stateInfo = playerController.animator.GetCurrentAnimatorStateInfo(0);
        }

        float animDuration = stateInfo.length;

        GameObject portalObject = transform.root.gameObject;
        Vector3 originalScale = portalObject.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float elapsed = 0f;

        // portalClose 사운드 재생 여부 플래그
        bool portalClosePlayed = false;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            Vector3 newScale = Vector3.Lerp(originalScale, targetScale, t);
            portalObject.transform.localScale = newScale;

            // 크기 x값 기준으로 0.8 이하 순간 → portalClose 출력
            if (!portalClosePlayed && newScale.x <= 0.5f)
            {
                portalClosePlayed = true;

                if (portalClose != null)
                {
                    audioSource.volume = 0.5f;
                    audioSource.PlayOneShot(portalClose);
                }
            }

            yield return null;
        }

        portalObject.transform.localScale = targetScale;

        playerController.isWorking = false;
        playerController.canMove = true;

        if (destroyAfterShrink)
        {
            Destroy(portalObject);
        }

        isProcessing = false;

        FunctionPoint fp = FindObjectOfType<FunctionPoint>();
        if (fp != null)
        {
            fp.AddPortalCloseCount(); // 카운트 증가 및 포탈 열림 여부 체크
        }
    }
    private void LookAtPortal()
    {
        GameObject portalObject = transform.root.gameObject;

        // 방향 계산
        Vector3 directionToPortal = portalObject.transform.position - playerController.transform.position;
        directionToPortal.y = 0f;

        if (directionToPortal.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPortal);
            playerController.transform.rotation = targetRotation;
        }
    }
}
