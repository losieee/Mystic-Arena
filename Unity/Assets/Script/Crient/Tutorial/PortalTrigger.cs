using UnityEngine;
using System.Collections;
using TMPro;

public class PortalTrigger : MonoBehaviour
{
    private bool playerInRange = false;
    private GameObject player;
    private Fight_Demo playerController;
    private bool playerInitialized = false;

    private bool destroyAfterShrink = false;
    private bool isProcessing = false;

    private TextMeshProUGUI portalText;

    void Start()
    {
        
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
            if (portalText != null)
            {
                portalText.gameObject.SetActive(false);
            }

            LookAtPortal();

            playerController.animator.SetTrigger("isWorking");
            playerController.canMove = false;
            playerController.isWorking = true;

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

    private IEnumerator ShrinkPortal()
    {
        isProcessing = true;

        // 애니메이션 상태 기다리기
        yield return new WaitForSeconds(0.05f);

        AnimatorStateInfo stateInfo = playerController.animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Working"))
        {
            yield return null;
            stateInfo = playerController.animator.GetCurrentAnimatorStateInfo(0);
        }

        float animDuration = stateInfo.length;

        // 애니메이션 재생되는 동안 shrink 진행
        GameObject portalObject = transform.root.gameObject;
        Vector3 originalScale = portalObject.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            portalObject.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
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
