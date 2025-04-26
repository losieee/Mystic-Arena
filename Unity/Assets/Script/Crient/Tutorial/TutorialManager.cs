using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Kino;

public class TutorialManager : MonoBehaviour
{
    public enum ImagePosition { G, Q, W, E, Cool, Heal }
    [System.Serializable]
    public class ExplanationData
    {
        public Sprite image;
        public string text;
        public ImagePosition imagePosition;
    }

    [Header("Dialogue Settings")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public AudioClip typingSound;
    public float typingSpeed = 0.05f;

    [Header("Explanation Settings")]
    public GameObject imageExplanationPanel;
    public Image imageG, imageQ, imageW, imageE, imageCool, imageHeal;
    public TextMeshProUGUI explanationText_G, explanationText_Q, explanationText_W,
                            explanationText_E, explanationText_Cool, explanationText_Heal;
    public AudioClip skillAudio;
    public List<ExplanationData> explanations = new List<ExplanationData>();
    public RectTransform explanationTextBox;

    [Header("Input Blocker")]
    public GameObject tutorialInputBlocker;

    [Header("Linked Character")]
    public Tutorial_Knight_Move knightMove;

    [Header("Glitch")]
    public AudioClip startSound;
    public DigitalGlitch glitchEffect;
    public AnalogGlitch analogglitch;
    private AudioSource audioSource;

    private bool isTyping = false;
    private bool isTypingExplanation = false;
    private bool isExplaining = false;
    public int dialogueIndex = 0;
    private int currentExplanationIndex = 0;
    private bool isInputBlocked = false;
    private bool waitingForHeal = false;
    private bool healed = false;
    private bool isRespawning = false;

    private string[] dialogues = {
        "안녕하세요 영웅이여!",
        "이곳은 여러 차원이 모이는 전장. 미스틱 아레나입니다!",
        "지금부터 전투를 위한 기본 정보를 안내하겠습니다.",
        "우선 스킬 설명입니다.",
        "피가 없을 때를 가정하여\n오브젝트를 클릭하여 체력을 회복해보세요!",
        "좋아요! 체력이 회복되었습니다!",
        "하지만 조심하세요. 이 회복은 한 목숨당 한 번뿐입니다.",
        "자, 이제 적의 진영으로 가서 적을 처치해봅시다!",
        "이런! 적 진영 안으로 들어가셨군요!",
        "적 진영 안으로 들어가게 되면 죽게됩니다! 조심하세요!",
        "이제 모든 안내를 마쳤으니 돌아갈 시간입니다.",
        "행운을 빌겠습니다!"
    };
    void Start()
    {
        Debug.Log(isTypingExplanation);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        BlockInput(true);
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeDialogue());

        if (explanations.Count >= 6)
        {
            explanations[0].text = "G는 차원이동(대쉬) 스킬입니다.\n사용 시 일정 거리를 빠르게 이동할 수 있으며\n사용 중에는 데미지를 받지 않습니다.";
            explanations[1].text = "Q는 공격 스킬입니다.\n캐릭터별 특성에 맞춰 다양한 효과를 냅니다.";
            explanations[2].text = "W는 시야 스킬입니다.\n캐릭터 주변의 시야를 밝혀줍니다.";
            explanations[3].text = "E는 버프 스킬입니다.\n캐릭터에게 이로운 효과를 부여합니다.";
            explanations[4].text = "스킬 쿨타임이 끝나게 되면\n스킬 이미지 주변이 빛나게 됩니다.";
            explanations[5].text = "전투 중 피가 닳게 되었을 때\n체력을 회복하는 수단입니다.\n목숨 당 한 번이므로 신중하게 활용하셔야 합니다!";
        }
    }
    void Update()
    {
        // 설명 중일 때 클릭 감지
        if (isExplaining && Input.GetMouseButtonDown(0))
        {
            currentExplanationIndex++;
            if (currentExplanationIndex < explanations.Count)
            {
                StartCoroutine(ShowExplanationSequence());
            }
            else
            {
                isExplaining = false;
                imageExplanationPanel.SetActive(false);
                dialoguePanel.SetActive(true);

                // 설명 끝난 후 자동 대사 출력
                dialogueIndex = 4;
                StartCoroutine(ShowHealInstruction());
            }
        }

        // 일반 대사 클릭 처리
        if (dialoguePanel.activeSelf && !isTyping && Input.GetMouseButtonDown(0))
        {
            dialogueIndex++;
            if (dialogueIndex == 4)
            {
                // 스킬 설명 시작
                dialoguePanel.SetActive(false);
                imageExplanationPanel.SetActive(true);
                isExplaining = true;
                currentExplanationIndex = 0;
                StartCoroutine(ShowExplanationSequence());
            }
            else if (dialogueIndex < dialogues.Length)
            {
                StartCoroutine(TypeDialogue());
            }
            else
            {
                // 모든 대화가 끝났으면 씬 이동
                LoadNextScene();
            }
        }
        // 인덱스 7번 대사가 나왔으면
        if (dialogueIndex == 7 && !isTyping && dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(false); // 대화 패널 숨기기
            BlockInput(false); // 입력 허용 (움직일 수 있게)
        }

        // 리스폰 중이고, 대화 패널이 닫혀 있다면 다음 대사 시작
        if (isRespawning && !dialoguePanel.activeSelf)
        {
            BlockInput(true); // 입력 다시 막기
            dialoguePanel.SetActive(true);
            dialogueIndex = 8; // 다음 대사 인덱스
            StartCoroutine(TypeDialogue());
            isRespawning = false; // 리스폰 상태 초기화
        }
    }

    IEnumerator ShowHealInstruction()
    {
        yield return StartCoroutine(TypeDialogue());

        waitingForHeal = true;
        BlockInput(false); // 입력 허용 (움직일 수 있게)

        knightMove.EnableMovement();

        // 사용자가 클릭할 때까지 대기
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        dialoguePanel.SetActive(false);
        knightMove.SetHealthToLow();
    }

    IEnumerator TypeDialogue()
    {
        isTyping = true;
        dialogueText.text = "";
        int charCount = 0;

        foreach (char letter in dialogues[dialogueIndex].ToCharArray())
        {
            dialogueText.text += letter;
            charCount++;

            if (typingSound != null && audioSource != null && charCount % 4 == 0)
                audioSource.PlayOneShot(typingSound);

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // 타이핑 끝나면 사운드 완전 정리
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    IEnumerator ShowExplanationSequence()
    {
        isTypingExplanation = true;

        var explanation = explanations[currentExplanationIndex];

        imageG.gameObject.SetActive(false);
        imageQ.gameObject.SetActive(false);
        imageW.gameObject.SetActive(false);
        imageE.gameObject.SetActive(false);
        imageCool.gameObject.SetActive(false);
        imageHeal.gameObject.SetActive(false);

        explanationText_G.gameObject.SetActive(false);
        explanationText_Q.gameObject.SetActive(false);
        explanationText_W.gameObject.SetActive(false);
        explanationText_E.gameObject.SetActive(false);
        explanationText_Cool.gameObject.SetActive(false);
        explanationText_Heal.gameObject.SetActive(false);

        Image targetImage = null;
        TextMeshProUGUI targetText = null;

        switch (explanation.imagePosition)
        {
            case ImagePosition.G: targetImage = imageG; targetText = explanationText_G; break;
            case ImagePosition.Q: targetImage = imageQ; targetText = explanationText_Q; break;
            case ImagePosition.W: targetImage = imageW; targetText = explanationText_W; break;
            case ImagePosition.E: targetImage = imageE; targetText = explanationText_E; break;
            case ImagePosition.Cool: targetImage = imageCool; targetText = explanationText_Cool; break;
            case ImagePosition.Heal: targetImage = imageHeal; targetText = explanationText_Heal; break;
        }

        if (targetImage != null)
        {
            targetImage.sprite = explanation.image;
            targetImage.color = Color.white;
            targetImage.gameObject.SetActive(true);
        }

        if (targetText != null)
        {
            targetText.gameObject.SetActive(true);
            targetText.text = explanation.text;
        }

        audioSource?.PlayOneShot(skillAudio);

        isTypingExplanation = false;
        yield return null;
    }

    public void BlockInput(bool block)
    {
        isInputBlocked = block;
        if (tutorialInputBlocker != null)
            tutorialInputBlocker.SetActive(block);
    }

    public bool IsInputBlocked()
    {
        return isInputBlocked;
    }
    public bool IsWaitingForHeal()
    {
        return waitingForHeal && !healed;
    }

    public void OnHealed()
    {
        if (healed) return;

        healed = true;
        waitingForHeal = false;

        BlockInput(true);
        dialoguePanel.SetActive(true);
        dialogueIndex = 5; // "좋아요! 체력이 회복되었습니다!"
        StartCoroutine(TypeDialogue());
    }
    public void OnCharacterDeath()
    {
        waitingForHeal = false; // 힐 대기 상태 종료
    }
    public void OnCharacterRespawn()
    {
        isRespawning = true;
    }
    private void LoadNextScene()
    {
        glitchEffect.intensity = 0f;
        analogglitch.scanLineJitter = 0f;
        analogglitch.verticalJump = 0f;
        analogglitch.horizontalShake = 0f;
        analogglitch.colorDrift = 0f;

        StartCoroutine(ToLobbyGlitchAndSound());
    }
    private IEnumerator ToLobbyGlitchAndSound()
    {
        yield return new WaitForSeconds(1f); // 1초 대기

        // 글리치 켜기
        if (glitchEffect != null && analogglitch != null)
        {
            glitchEffect.enabled = true;
            analogglitch.enabled = true;
            StartCoroutine(IncreaseGlitch());
        }

        // 효과음 재생
        if (startSound != null)
        {
            audioSource.PlayOneShot(startSound);
            yield return new WaitForSeconds(startSound.length);
        }

        SceneManager.LoadScene("Main_Lobby");
    }
    private IEnumerator IncreaseGlitch()
    {
        float duration = startSound != null ? startSound.length : 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Analog 글리치 점점 증가
            analogglitch.scanLineJitter = Mathf.Lerp(0.1f, 1.0f, t);
            analogglitch.verticalJump = Mathf.Lerp(0.05f, 0.5f, t);
            analogglitch.horizontalShake = Mathf.Lerp(0.1f, 0.8f, t);
            analogglitch.colorDrift = Mathf.Lerp(0.1f, 1.0f, t);

            // Digital 글리치 점점 증가
            glitchEffect.intensity = Mathf.Lerp(0.0f, 1.0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}