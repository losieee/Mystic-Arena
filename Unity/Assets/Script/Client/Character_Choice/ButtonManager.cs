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
    private float timeLimit = 10f; // ���ѽð� 10��
    private bool isTimeOver = false; // �ð� �ʰ� ����
    public TMP_Text timerText; // UI�� ǥ���� Ÿ�̸� �ؽ�Ʈ

    public GameObject selectedPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� �Ѿ�� �ı� �� ��
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

        // �ʱ�ȭ�� ���� �ε�� �� ����
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

        // ���� ī�޶󿡼� �۸�ġ ������Ʈ ã�Ƽ� ���� �� ��Ȱ��ȭ
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
        // �� ������ ���
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
        selectedButton = null;    // ���õ� ��ư �ʱ�ȭ
        isTimeOver = false;       // �ð� �ʰ� ���� �ʱ�ȭ
        timeLimit = 10f;          // Ÿ�̸� �ʱ�ȭ
        UpdateTimerUI();          // Ÿ�̸� UI ������Ʈ
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
                TimeOver();  // �� ���� �߿�! 0���� �� �� ��ȯ
            }
            else
            {
                UpdateTimerUI();
            }
        }
    }

    // Ÿ�̸Ӹ� 1�ʸ��� ������Ʈ�ϴ� �Լ�
    void UpdateTimer()
    {
        if (timeLimit > 0)
        {
            timeLimit -= 1f;
        }
        else
        {
            CancelInvoke("UpdateTimer"); // Ÿ�̸Ӱ� ������ �� �̻� ȣ������ ����
            TimeOver(); // Ÿ�̸� ���� �� TimeOver ȣ��
        }
    }

    // UI Ÿ�̸� ������Ʈ
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = timeLimit.ToString("F0"); // ������ ǥ��
        }
    }

    // ��ư�� ������ ��� �� �̵����� �ʰ� ���ø� ����
    public bool CanSelect()
    {
        return selectedButton == null && !isTimeOver;
    }

    // ��ư�� ���õǾ��� �� ȣ��
    public void SetSelectedButton(ButtonEffect button)
    {
        selectedButton = button;
        DisableOtherButtons();
    }

    // �ð��� �� ������, ��ư�� ���ȴ��� ���ο� ���� �� �̵�
    private void TimeOver()
    {
        isTimeOver = true;

        string targetScene = (selectedButton != null) ? "SampleScene" : "Main_Lobby";

        // Transition ȿ���� ���� �� �� ��ȯ
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

        // ȿ���� ���
        float soundDuration = 2f;
        if (transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound);
            soundDuration = transitionSound.length;
        }

        // ȿ���� ���� ������ ���
        yield return new WaitForSeconds(soundDuration);

        // �� ��ȯ
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

    // �ٸ� ��ư�� ��Ȱ��ȭ
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