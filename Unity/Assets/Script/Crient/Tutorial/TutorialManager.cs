//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using TMPro;
//using System.Collections.Generic;
//using UnityEngine.SceneManagement;
//using Kino;
//using UnityEditor;

//public class TutorialManager : MonoBehaviour
//{
//    public enum ImagePosition { G, Q, W, E, Cool, Heal }
//    [System.Serializable]
//    public class ExplanationData
//    {
//        public Sprite image;
//        public string text;
//        public ImagePosition imagePosition;
//    }

//    // �⺻ ��� �г�
//    [Header("Dialogue Settings")]
//    public GameObject dialoguePanel;
//    public TextMeshProUGUI dialogueText;
//    public AudioClip typingSound;
//    public float typingSpeed = 0.05f;

//    // �̹��� ���� �г�
//    [Header("Explanation Settings")]
//    public GameObject imageExplanationPanel;
//    public Image imageG, imageQ, imageW, imageE, imageCool, imageHeal;
//    public TextMeshProUGUI explanationText_G, explanationText_Q, explanationText_W,
//                            explanationText_E, explanationText_Cool, explanationText_Heal;
//    public AudioClip skillAudio;
//    public List<ExplanationData> explanations = new List<ExplanationData>();
//    public RectTransform explanationTextBox;

//    // Ʃ�丮�� �� ĳ���� ������ ����
//    [Header("Input Blocker")]
//    public GameObject tutorialInputBlocker;

//    // ĳ���� ��ũ��Ʈ ����
//    [Header("Linked Character")]
//    public Tutorial_Knight_Move knightMove;

//    // �۸�ġ ȿ��
//    [Header("Glitch")]
//    public AudioClip startSound;
//    public DigitalGlitch glitchEffect;
//    public AnalogGlitch analogglitch;
//    private AudioSource audioSource;

//    private bool isTyping = false;           // ���� Ÿ���� ������
//    private bool isTypingExplanation = false;   // ���� Ÿ���� ������
//    private bool isExplaining = false;       // ���� ������
//    public int dialogueIndex = 0;               // ���� ��ȭ �ε���
//    private int currentExplanationIndex = 0; // ���� ���� �ε���
//    private bool isInputBlocked = false;        // �Է� ���� ����
//    private bool waitingForHeal = false;     // �� ��ٸ��� ������
//    private bool healed = false;                // ȸ�� �Ϸ��ߴ���
//    private bool isRespawning = false;       // ������ ���������
//    private bool sceneLoading = false;          // �� �Ѿ�� ������
//    private bool isForceRespawning = false;  // ���� �������� �ߴ���(Ʃ�丮�� �� ���� �Ѿ�� ��)

//    // Ʃ�丮�� ����
//    private string[] dialogues = {
//        "�ȳ��ϼ��� �����̿�!",
//        "�̰��� ���� ������ ���̴� ����. �̽�ƽ �Ʒ����Դϴ�!",
//        "���ݺ��� ������ ���� �⺻ ������ �ȳ��ϰڽ��ϴ�.",
//        "�켱 ��ų �����Դϴ�.",
//        "�ǰ� ���� ���� �����Ͽ�\n������Ʈ�� Ŭ���Ͽ� ü���� ȸ���غ�����!",
//        "���ƿ�! ü���� ȸ���Ǿ����ϴ�!",
//        "������ �����ϼ���. �� ȸ���� �� ����� �� �����Դϴ�.",
//        "��, ���� ������ �����ϼ���!",
//        "������ �����ø� ������ �����⸦ ���� �����ø� �˴ϴ�."
//    };
//    void Start()
//    {
//        Debug.Log(isTypingExplanation); //����� ���� ����
//        audioSource = gameObject.AddComponent<AudioSource>();
//        audioSource.playOnAwake = false;

//        // �� �����ڸ��� Ʃ�丮�� ����
//        BlockInput(true);
//        dialoguePanel.SetActive(true);
//        StartCoroutine(TypeDialogue());

//        // �̹��� ���� �гο��� ���� �����
//        if (explanations.Count >= 6)
//        {
//            explanations[0].text = "G�� �����̵�(�뽬) ��ų�Դϴ�.\n��� �� ���� �Ÿ��� ������ �̵��� �� ������\n��� �߿��� �������� ���� �ʽ��ϴ�.";
//            explanations[1].text = "Q�� ���� ��ų�Դϴ�.\nĳ���ͺ� Ư���� ���� �پ��� ȿ���� ���ϴ�.";
//            explanations[2].text = "W�� �þ� ��ų�Դϴ�.\nĳ���� �ֺ��� �þ߸� �����ݴϴ�.";
//            explanations[3].text = "E�� ���� ��ų�Դϴ�.\nĳ���Ϳ��� �̷ο� ȿ���� �ο��մϴ�.";
//            explanations[4].text = "��ų ��Ÿ���� ������ �Ǹ�\n��ų �̹��� �ֺ��� ������ �˴ϴ�.";
//            explanations[5].text = "���� �� �ǰ� ��� �Ǿ��� ��\nü���� ȸ���ϴ� �����Դϴ�.\n��� �� �� ���̹Ƿ� �����ϰ� Ȱ���ϼž� �մϴ�!";
//        }
//    }
//    void Update()
//    {
//        // �ε��� 4�� ���� ���¿��� �� �ȹް� ������ ������ ��
//        if (waitingForHeal && !healed)
//        {
//            float distance = Vector3.Distance(knightMove.transform.position, knightMove.respawnPoint.position);
//            if (distance > 5f) // ������ �̻� �־�����
//            {
//                ForceRespawn(); // ���� ������
//            }
//        }

//        // ���� ���� �� Ŭ�� ����
//        if (isExplaining && Input.GetMouseButtonDown(0))
//        {
//            currentExplanationIndex++;
//            if (currentExplanationIndex < explanations.Count)
//            {
//                StartCoroutine(ShowExplanationSequence());
//            }
//            else
//            {
//                isExplaining = false;
//                imageExplanationPanel.SetActive(false);
//                dialoguePanel.SetActive(true);

//                dialogueIndex = 4;
//                StartCoroutine(ShowHealInstruction());
//            }
//        }

//        // �ε��� 8�� ���� Ŭ���ϸ� �г� ����
//        if (dialogueIndex == 8 && !isTyping && dialoguePanel.activeSelf)
//        {
//            if (Input.GetMouseButtonDown(0))
//            {
//                dialoguePanel.SetActive(false);
//                BlockInput(false);
//            }

//            return;
//        }

//        // �Ϲ� ��� Ŭ�� ó�� (7�� �ƴ� ����)
//        if (dialoguePanel.activeSelf && !isTyping && Input.GetMouseButtonDown(0))
//        {
//            // ���� ������ �ȳ� ���̸� �гθ� ���� ��
//            if (isForceRespawning)
//            {
//                dialoguePanel.SetActive(false);
//                isForceRespawning = false;
//                return;
//            }

//            dialogueIndex++;

//            // �ε��� 4 ��� �߰� �Ѿ�� ȸ�� �ؾ߸� ���� �ε��� ��
//            if (dialogueIndex == 4)
//            {
//                dialoguePanel.SetActive(false);
//                imageExplanationPanel.SetActive(true);
//                isExplaining = true;
//                currentExplanationIndex = 0;
//                StartCoroutine(ShowExplanationSequence());
//            }
//            // ���� ���� ��簡 ������ ���� ��� ���
//            else if (dialogueIndex < dialogues.Length)
//            {
//                StartCoroutine(TypeDialogue());
//            }
//            else
//            {
//                // ��簡 ���� �������� �� �̵�
//                //if (!sceneLoading)
//                //{
//                //    sceneLoading = true;
//                //    dialoguePanel.SetActive(false);
//                //    BlockInput(false);
//                //    //LoadNextScene();
//                //}
//            }
//        }




//        // ������ ���� ó��
//        if (isRespawning && !dialoguePanel.activeSelf)
//        {
//            //BlockInput(true);
//            //dialoguePanel.SetActive(true);
//            //dialogueIndex = 8;
//            //StartCoroutine(TypeDialogue());
//            isRespawning = false;
//        }
//    }
//    public void GoToLobby()
//    {
//        LoadNextScene();
//    }
//    private void ForceRespawn()
//    {
//        // ��ġ�� ������ ����Ʈ�� ���� �̵�
//        knightMove.transform.SetPositionAndRotation(knightMove.respawnPoint.position, knightMove.respawnPoint.rotation);

//        knightMove.StopAgentImmediately(); // ������ ����

//        // �� ��ٸ��� ���� ����
//        if (!dialoguePanel.activeSelf)
//        {
//            dialoguePanel.SetActive(true);
//        }

//        dialogueText.text = "�� ���� �ް� ���ž� �մϴ�!";

//        isForceRespawning = true;
//    }

//    // ü�� ȸ�� �ϸ� ���� ��� �ߴ� �Լ�
//    IEnumerator ShowHealInstruction()
//    {
//        yield return StartCoroutine(TypeDialogue());

//        // Ÿ���� ������ �Ҹ� ���� ����
//        if (audioSource.isPlaying)
//            audioSource.Stop();

//        waitingForHeal = true;
//        BlockInput(false); // �Է� ��� (������ �� �ְ�)

//        knightMove.EnableMovement();

//        // ����ڰ� Ŭ���� ������ ���
//        while (!Input.GetMouseButtonDown(0))
//        {
//            yield return null;
//        }

//        dialoguePanel.SetActive(false);
//        knightMove.SetHealthToLow();
//    }

//    // ��� Ÿ���� ���
//    IEnumerator TypeDialogue()
//    {
//        isTyping = true;
//        dialogueText.text = "";
//        int charCount = 0;

//        foreach (char letter in dialogues[dialogueIndex].ToCharArray())
//        {
//            dialogueText.text += letter;
//            charCount++;

//            if (typingSound != null && audioSource != null && charCount % 4 == 0)
//                audioSource.PlayOneShot(typingSound);

//            yield return new WaitForSeconds(typingSpeed);
//        }

//        isTyping = false;

//        // Ÿ���� ������ ���� ���� ����
//        if (audioSource.isPlaying)
//            audioSource.Stop();
//    }

//    // �̹��� + ���� ���(��ų, ��)
//    IEnumerator ShowExplanationSequence()
//    {
//        isTypingExplanation = true;

//        var explanation = explanations[currentExplanationIndex];

//        imageG.gameObject.SetActive(false);
//        imageQ.gameObject.SetActive(false);
//        imageW.gameObject.SetActive(false);
//        imageE.gameObject.SetActive(false);
//        imageCool.gameObject.SetActive(false);
//        imageHeal.gameObject.SetActive(false);

//        explanationText_G.gameObject.SetActive(false);
//        explanationText_Q.gameObject.SetActive(false);
//        explanationText_W.gameObject.SetActive(false);
//        explanationText_E.gameObject.SetActive(false);
//        explanationText_Cool.gameObject.SetActive(false);
//        explanationText_Heal.gameObject.SetActive(false);

//        Image targetImage = null;
//        TextMeshProUGUI targetText = null;

//        switch (explanation.imagePosition)
//        {
//            case ImagePosition.G: targetImage = imageG; targetText = explanationText_G; break;
//            case ImagePosition.Q: targetImage = imageQ; targetText = explanationText_Q; break;
//            case ImagePosition.W: targetImage = imageW; targetText = explanationText_W; break;
//            case ImagePosition.E: targetImage = imageE; targetText = explanationText_E; break;
//            case ImagePosition.Cool: targetImage = imageCool; targetText = explanationText_Cool; break;
//            case ImagePosition.Heal: targetImage = imageHeal; targetText = explanationText_Heal; break;
//        }

//        if (targetImage != null)
//        {
//            targetImage.sprite = explanation.image;
//            targetImage.color = Color.white;
//            targetImage.gameObject.SetActive(true);
//        }

//        if (targetText != null)
//        {
//            targetText.gameObject.SetActive(true);
//            targetText.text = explanation.text;
//        }

//        audioSource?.PlayOneShot(skillAudio);

//        isTypingExplanation = false;
//        yield return null;
//    }

//    // ��簡 ������ ���� �� ĳ���� �������̰�
//    public void BlockInput(bool block)
//    {
//        isInputBlocked = block;
//        if (tutorialInputBlocker != null)
//            tutorialInputBlocker.SetActive(block);
//    }

//    // ĳ���� ��ũ��Ʈ�� ���� �Ǿ����� (�Է� ����)
//    public bool IsInputBlocked()
//    {
//        return isInputBlocked;
//    }
//    public bool IsWaitingForHeal()
//    {
//        return waitingForHeal && !healed;
//    }
//    public void OnCharacterDeath()
//    {
//        waitingForHeal = false; // �� ��� ���� ����
//    }
//    public void OnCharacterRespawn()
//    {
//        isRespawning = true;
//    }

//    // ���� �ߴٸ� ������ ���
//    public void OnHealed()
//    {
//        if (healed) return;

//        healed = true;
//        waitingForHeal = false;

//        isForceRespawning = false;

//        BlockInput(true);
//        dialoguePanel.SetActive(true);
//        dialogueIndex = 5; // "���ƿ�! ü���� ȸ���Ǿ����ϴ�!"
//        StartCoroutine(TypeDialogue());
//    }

//    // �۸�ġ �����鼭 �� ��ȯ
//    private void LoadNextScene()
//    {
//        glitchEffect.intensity = 0f;
//        analogglitch.scanLineJitter = 0f;
//        analogglitch.verticalJump = 0f;
//        analogglitch.horizontalShake = 0f;
//        analogglitch.colorDrift = 0f;

//        StartCoroutine(ToLobbyGlitchAndSound());
//    }
//    private IEnumerator ToLobbyGlitchAndSound()
//    {
//        yield return new WaitForSeconds(1f); // 1�� ���

//        // �۸�ġ �ѱ�
//        if (glitchEffect != null && analogglitch != null)
//        {
//            glitchEffect.enabled = true;
//            analogglitch.enabled = true;
//            StartCoroutine(IncreaseGlitch());
//        }

//        // ȿ���� ���
//        if (startSound != null)
//        {
//            audioSource.PlayOneShot(startSound);
//            yield return new WaitForSeconds(startSound.length);
//        }

//        SceneManager.LoadScene("Main_Lobby");
//    }
//    private IEnumerator IncreaseGlitch()
//    {
//        float duration = startSound != null ? startSound.length : 2f;
//        float elapsed = 0f;

//        while (elapsed < duration)
//        {
//            float t = elapsed / duration;

//            // Analog �۸�ġ ���� ����
//            analogglitch.scanLineJitter = Mathf.Lerp(0.1f, 1.0f, t);
//            analogglitch.verticalJump = Mathf.Lerp(0.05f, 0.5f, t);
//            analogglitch.horizontalShake = Mathf.Lerp(0.1f, 0.8f, t);
//            analogglitch.colorDrift = Mathf.Lerp(0.1f, 1.0f, t);

//            // Digital �۸�ġ ���� ����
//            glitchEffect.intensity = Mathf.Lerp(0.0f, 1.0f, t);

//            elapsed += Time.deltaTime;
//            yield return null;
//        }
//    }
//}