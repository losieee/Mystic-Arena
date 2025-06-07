using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public Animator animator;  // �÷��̾� Animator ���� (Inspector���� ����!)

    private Fight_Demo playerController;
    private bool playerInRange = false;
    private GameObject currentPotal;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // �÷��̾� �̸� ã��
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<Fight_Demo>();
            Debug.Log("TutorialManager: playerController ������: " + playerController.name);
        }
        else
        {
            Debug.LogWarning("TutorialManager: Player �±׸� ���� ������Ʈ�� �����ϴ�!");
        }
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (playerController != null && currentPotal != null)
            {
                // �����ϰ� �� �� ���� ���� ����
                playerController.animator.SetTrigger("isWorking");
                playerController.canMove = false;
                playerController.isWorking = true;

                StartCoroutine(ShrinkPotalDuringAnimation(currentPotal));
            }
            else
            {
                Debug.LogWarning("FŰ �������� playerController �Ǵ� currentPotal�� null�Դϴ�!");
            }
        }
    }

    public void SetCurrentPotal(GameObject potal)
    {
        currentPotal = potal;
        Debug.Log("TutorialManager: currentPotal ������: " + currentPotal.name);

        playerInRange = true;
    }

    public void ClearCurrentPotal(GameObject potal)
    {
        if (currentPotal == potal)
        {
            currentPotal = null;
            Debug.Log("TutorialManager: currentPotal ������");

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

        // playerController null üũ
        if (playerController == null)
        {
            Debug.LogWarning("ShrinkPotalDuringAnimation - playerController == null!");
            yield break;
        }

        // playerController.animator null üũ
        if (playerController.animator == null)
        {
            Debug.LogWarning("ShrinkPotalDuringAnimation - playerController.animator == null!");
            yield break;
        }

        yield return new WaitForSeconds(0.05f);  // Animator ���� ����ȭ ���

        AnimatorStateInfo stateInfo = playerController.animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Working"))
        {
            yield return null;
            stateInfo = playerController.animator.GetCurrentAnimatorStateInfo(0);
        }

        float duration = stateInfo.length;
        Debug.Log("Working �ִϸ��̼� ����: " + duration + "��");

        Vector3 originalScale = potalTarget.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        Debug.Log("Portal �ʱ� ������: " + originalScale);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            potalTarget.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        potalTarget.transform.localScale = targetScale;
        Debug.Log("Portal ���� ������: " + potalTarget.transform.localScale);
    }

}
