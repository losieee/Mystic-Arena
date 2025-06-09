using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    public AudioClip transitionSound;
    private AudioSource audioSource;

    public Kino.DigitalGlitch glitchEffect;
    public Kino.AnalogGlitch analogGlitch;
    public static ButtonManager Instance { get; private set; }
    private ButtonEffect selectedButton = null;
    private float timeLimit = 10f; // 제한시간 10초
    private bool isTimeOver = false; // 시간 초과 여부
    public TMP_Text timerText; // UI에 표시할 타이머 텍스트

    public GameObject selectedPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 파괴 안 됨
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        // 초기화는 씬이 로드될 때 진행
        if (SceneManager.GetActiveScene().name == "Main_Lobby")
        {
            InitializeTimer();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Character_Choice")
        {
            StartCoroutine(DelayedInitializeTimer());
        }

        // 메인 카메라에서 글리치 컴포넌트 찾아서 참조 및 비활성화
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            glitchEffect = mainCam.GetComponent<Kino.DigitalGlitch>();
            analogGlitch = mainCam.GetComponent<Kino.AnalogGlitch>();

            if (glitchEffect != null)
                glitchEffect.enabled = false;

            if (analogGlitch != null)
                analogGlitch.enabled = false;
        }
    }

    private IEnumerator DelayedInitializeTimer()
    {
        // 한 프레임 대기
        yield return null;

        var textObj = GameObject.Find("TimerText");
        if (textObj != null)
        {
            timerText = textObj.GetComponent<TMP_Text>();
        }

        InitializeTimer();
    }

    void InitializeTimer()
    {
        selectedButton = null;    // 선택된 버튼 초기화
        isTimeOver = false;       // 시간 초과 여부 초기화
        timeLimit = 10f;          // 타이머 초기화
        UpdateTimerUI();          // 타이머 UI 업데이트
    }

    void Update()
    {
        if (!isTimeOver)
        {
            timeLimit -= Time.deltaTime;
            if (timeLimit <= 0f)
            {
                timeLimit = 0f;
                UpdateTimerUI();
                TimeOver();  // 이 줄이 중요! 0초일 때 씬 전환
            }
            else
            {
                UpdateTimerUI();
            }
        }
    }

    // 타이머를 1초마다 업데이트하는 함수
    void UpdateTimer()
    {
        if (timeLimit > 0)
        {
            timeLimit -= 1f;
        }
        else
        {
            CancelInvoke("UpdateTimer"); // 타이머가 끝나면 더 이상 호출하지 않음
            TimeOver(); // 타이머 종료 후 TimeOver 호출
        }
    }

    // UI 타이머 업데이트
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = timeLimit.ToString("F0"); // 정수로 표시
        }
    }

    // 버튼을 눌러도 즉시 씬 이동하지 않고 선택만 유지
    public bool CanSelect()
    {
        return selectedButton == null && !isTimeOver;
    }

    // 버튼이 선택되었을 때 호출
    public void SetSelectedButton(ButtonEffect button)
    {
        selectedButton = button;
        DisableOtherButtons();
    }

    // 시간이 다 지나면, 버튼이 눌렸는지 여부에 따라 씬 이동
    private void TimeOver()
    {
        isTimeOver = true;

        string targetScene = (selectedButton != null) ? "SampleScene" : "Main_Lobby";

        // Transition 효과가 끝난 후 씬 전환
        StartCoroutine(PlayTransitionAndLoadScene(targetScene));
    }

    private IEnumerator PlayTransitionAndLoadScene(string sceneName)
    {
        if (glitchEffect != null && analogGlitch != null)
        {
            glitchEffect.enabled = true;
            analogGlitch.enabled = true;
            StartCoroutine(IncreaseGlitch());
        }

        // 효과음 재생
        float soundDuration = 2f;
        if (transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound);
            soundDuration = transitionSound.length;
        }

        // 효과음 끝날 때까지 대기
        yield return new WaitForSeconds(soundDuration);

        // 씬 전환
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator IncreaseGlitch()
    {
        float duration = transitionSound != null ? transitionSound.length : 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            analogGlitch.scanLineJitter = Mathf.Lerp(0.1f, 1.0f, t);
            analogGlitch.verticalJump = Mathf.Lerp(0.05f, 0.5f, t);
            analogGlitch.horizontalShake = Mathf.Lerp(0.1f, 0.8f, t);
            analogGlitch.colorDrift = Mathf.Lerp(0.1f, 1.0f, t);

            glitchEffect.intensity = Mathf.Lerp(0.0f, 1.0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // 다른 버튼을 비활성화
    private void DisableOtherButtons()
    {
        ButtonEffect[] allButtons = FindObjectsOfType<ButtonEffect>();
        foreach (ButtonEffect button in allButtons)
        {
            if (button != selectedButton)
            {
                button.DisableButton();
            }
        }
    }
}