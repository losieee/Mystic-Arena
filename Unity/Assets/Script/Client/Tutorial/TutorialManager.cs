using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public Animator animator;  // 플레이어 Animator 연결 (Inspector에서 연결!)

    private Fight_Demo playerController;
    private bool playerInRange = false;
    private GameObject currentPotal;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // 플레이어 미리 찾기
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<Fight_Demo>();
            Debug.Log("TutorialManager: playerController 설정됨: " + playerController.name);
        }
        else
        {
            Debug.LogWarning("TutorialManager: Player 태그를 가진 오브젝트가 없습니다!");
        }
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (playerController != null && currentPotal != null)
            {
                // 안전하게 둘 다 있을 때만 실행
                playerController.animator.SetTrigger("isWorking");
                playerController.canMove = false;
                playerController.isWorking = true;

                StartCoroutine(ShrinkPotalDuringAnimation(currentPotal));
            }
            else
            {
                Debug.LogWarning("F키 눌렀지만 playerController 또는 currentPotal이 null입니다!");
            }
        }
    }

    public void SetCurrentPotal(GameObject potal)
    {
        currentPotal = potal;
        Debug.Log("TutorialManager: currentPotal 설정됨: " + currentPotal.name);

        playerInRange = true;
    }

    public void ClearCurrentPotal(GameObject potal)
    {
        if (currentPotal == potal)
        {
            currentPotal = null;
            Debug.Log("TutorialManager: currentPotal 해제됨");

            playerInRange = false;
        }
    }


    private IEnumerator ShrinkPotalDuringAnimation(GameObject potalTarget)
    {
        if (potalTarget == null)
        {
            Debug.LogWarning("ShrinkPotalDuringAnimation - potalTarget == null!");
            yield break;
        }

        // playerController null 체크
        if (playerController == null)
        {
            Debug.LogWarning("ShrinkPotalDuringAnimation - playerController == null!");
            yield break;
        }

        // playerController.animator null 체크
        if (playerController.animator == null)
        {
            Debug.LogWarning("ShrinkPotalDuringAnimation - playerController.animator == null!");
            yield break;
        }

        yield return new WaitForSeconds(0.05f);  // Animator 상태 동기화 대기

        AnimatorStateInfo stateInfo = playerController.animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Working"))
        {
            yield return null;
            stateInfo = playerController.animator.GetCurrentAnimatorStateInfo(0);
        }

        float duration = stateInfo.length;
        Debug.Log("Working 애니메이션 길이: " + duration + "초");

        Vector3 originalScale = potalTarget.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        Debug.Log("Portal 초기 스케일: " + originalScale);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            potalTarget.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        potalTarget.transform.localScale = targetScale;
        Debug.Log("Portal 최종 스케일: " + potalTarget.transform.localScale);
    }

}
