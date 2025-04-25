using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public enum ImagePosition
    {
        G, Q, W, E, Cool, Heal
    }

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
    public Image imageG;
    public Image imageQ;
    public Image imageW;
    public Image imageE;
    public Image imageCool;
    public Image imageHeal;

    public TextMeshProUGUI explanationText_G;
    public TextMeshProUGUI explanationText_Q;
    public TextMeshProUGUI explanationText_W;
    public TextMeshProUGUI explanationText_E;
    public TextMeshProUGUI explanationText_Cool;
    public TextMeshProUGUI explanationText_Heal;

    public AudioClip skillAudio;

    public List<ExplanationData> explanations = new List<ExplanationData>();
    public RectTransform explanationTextBox;

    [Header("Input Blocker")]
    public GameObject tutorialInputBlocker; // Inspector 창에서 할당

    private AudioSource audioSource;
    private bool isTyping = false;
    private bool isTypingExplanation = false;
    private bool dialogueEnded = false;
    private bool isExplaining = false;
    private float inactivityTimer = 0f;
    private float waitTime = 3f;
    private int dialogueIndex = 0;
    private int currentExplanationIndex = 0;
    private bool isInputBlocked = false; // 입력 차단 상태

    private string[] dialogues = {
        "안녕하세요 영웅이여!",
        "이곳은 여러 차원이 모이는 전장. 미스틱 아레나입니다!",
        "지금부터 전투를 위한 기본 정보를 안내하겠습니다.",
        "우선 스킬 설명입니다."
    };

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // 설명 텍스트 임시 세팅
        if (explanations.Count >= 6) // explanations 리스트 크기를 6 이상으로 확인
        {
            explanations[0].text = "G는 차원이동(대쉬) 스킬입니다.\n사용시 일정 거리를 빠르게 이동할 수 있으며\n사용중에는 데미지를 받지 않습니다.";
            explanations[1].text = "Q는 공격 스킬입니다.\n캐릭터별 특성에 맞춰 다양한 효과를 냅니다.";
            explanations[2].text = "W는 시야 스킬입니다.\n캐릭터 주변의 시야를 밝혀줍니다.";
            explanations[3].text = "E는 버프 스킬입니다.\n캐릭터에게 이로운 효과를 부여합니다.";
            explanations[4].text = "스킬 쿨타임이 끝나게 되면\n스킬 이미지 주변이 빛나게 됩니다.";
            explanations[5].text = "전투 중 피가 닳게 되었을 때 체력을 회복하는 수단입니다.\n목숨 당 한번이므로 신중하게 활용하셔야 합니다!";
        }

        dialoguePanel.SetActive(true);
        BlockInput(true); // 다이얼로그 시작 시 입력 막기
        StartCoroutine(TypeDialogue());
    }

    void Update()
    {
        // 다이얼로그 패널이 활성화되어 있을 때 클릭 감지
        if (dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = dialogues[dialogueIndex];
                isTyping = false;
            }
            else
            {
                dialogueIndex++;
                if (dialogueIndex < dialogues.Length)
                {
                    StartCoroutine(TypeDialogue());
                }
                else
                {
                    dialoguePanel.SetActive(false);
                    dialogueEnded = true;
                    inactivityTimer = 0f;

                    imageExplanationPanel.SetActive(true);
                    isExplaining = true;
                    currentExplanationIndex = 0;
                    BlockInput(true); // 설명 시작 시 입력 막기
                    StartCoroutine(ShowExplanationSequence());
                }
            }
        }

        // 설명 패널이 활성화되어 있을 때 클릭 감지
        if (isExplaining && Input.GetMouseButtonDown(0))
        {
            if (isTypingExplanation)
            {
                StopAllCoroutines();
                ShowFullExplanationText();
                isTypingExplanation = false;
            }
            else
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
                    BlockInput(false); // 설명 종료 시 입력 허용
                }
            }
        }

        // 다이얼로그와 설명이 모두 끝났을 때
        if (dialogueEnded && !isExplaining)
        {
            if (Input.anyKeyDown)
            {
                inactivityTimer = 0f;
            }
            else
            {
                inactivityTimer += Time.deltaTime;
                if (inactivityTimer >= waitTime)
                {
                    dialogueEnded = false;
                }
            }
        }
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

            if (typingSound != null && audioSource != null && charCount % 3 == 0)
            {
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    IEnumerator ShowExplanationSequence()
    {
        isTypingExplanation = true;

        var explanation = explanations[currentExplanationIndex];

        // 모든 이미지/텍스트 비활성화
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
            case ImagePosition.G:
                targetImage = imageG;
                targetText = explanationText_G;
                audioSource.PlayOneShot(skillAudio);
                break;
            case ImagePosition.Q:
                targetImage = imageQ;
                targetText = explanationText_Q;
                audioSource.PlayOneShot(skillAudio);
                break;
            case ImagePosition.W:
                targetImage = imageW;
                targetText = explanationText_W;
                audioSource.PlayOneShot(skillAudio);
                break;
            case ImagePosition.E:
                targetImage = imageE;
                targetText = explanationText_E;
                audioSource.PlayOneShot(skillAudio);
                break;
            case ImagePosition.Cool:
                targetImage = imageCool;
                targetText = explanationText_Cool;
                audioSource.PlayOneShot(skillAudio);
                break;
            case ImagePosition.Heal:
                targetImage = imageHeal;
                targetText = explanationText_Heal;
                audioSource.PlayOneShot(skillAudio);
                break;
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
            targetText.text = explanation.text;  // 타이핑 효과 없이 즉시 출력
        }

        yield return null;

        isTypingExplanation = false;
    }

    void ShowFullExplanationText()
    {
        var explanation = explanations[currentExplanationIndex];
        switch (explanation.imagePosition)
        {
            case ImagePosition.G:
                explanationText_G.text = explanation.text;
                break;
            case ImagePosition.Q:
                explanationText_Q.text = explanation.text;
                break;
            case ImagePosition.W:
                explanationText_W.text = explanation.text;
                break;
            case ImagePosition.E:
                explanationText_E.text = explanation.text;
                break;
            case ImagePosition.Cool:
                explanationText_Cool.text = explanation.text;
                break;
        }
    }

    public void BlockInput(bool block)
    {
        isInputBlocked = block;
        if (tutorialInputBlocker != null)
        {
            tutorialInputBlocker.SetActive(block);
        }
    }
    public bool IsInputBlocked()
    {
        return isInputBlocked;
    }
}