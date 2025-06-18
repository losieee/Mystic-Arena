using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEditor;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [System.Serializable]
    public class WaveData
    {
        public int limitTime;
        public List<int> waveEnemyCounts;
    }

    public Dictionary<string, WaveData> waveTable = new Dictionary<string, WaveData>();

    private Dictionary<string, List<string>> stageStartDialogues = new()
    {
        { "Stage_1", new List<string> {
            "���ù��� ��\n\n���Ⱑ.. ������ �տ��̱�",
            "���ù��� ��\n\n...!! ���� ���ʹ�! � �������� óġ�ؾ߰ھ�"
        }},
        { "Stage_2", new List<string> {
            "���ù��� ��\n\n���Ⱑ�� �����ϰ� ����� ���ڱ�.",
            "���ù��� ��\n\n�̰� �ܼ��� ħ���� �ƴϴ�. ���� ��ü�� �������� �־�.",
            "���� ���� ��ű�\n\n�� 'ħ�ĵ� ��' ���� ���̴�ġ�ڱ���. ��� ��ȭ��Ű����."
        }},
        { "Stage_3", new List<string> {
            "���� ���� ��ű�\n\n...����� ����. ��ȭ �ӵ��� ���� ���� �ӵ��� ������ ���ϰ� �ֽ��ϴ�.",
            "���� ���� ��ű�\n\n���� ��ü�� �������� �ֽ��ϴ�. ���� ��� ��ȭ�� �����ϼ���. ���ù��� ��."
        }},
        { "Stage_4", new List<string> {
            "���ù��� ��\n\n...��ȭ�� �� �˾Ҵ� ���� ƴ�� �ٽ� �������־�.",
            "���ù��� ��\n\n���� �̻��ϱ�, �������� �ٸ� ��ô�� ��������."
        }},
        { "Stage_5", new List<string> {
            "���ù��� ��\n\n...������ ��ȭ�� �ŵ��Ҽ��� ����� ������ �� �ΰ����� �ƴϾ�. �̰��� ���� ������ ������ �ִ°��ΰ�.",
            "���ù��� ��\n\n�� ���̿� �ٴٸ� �̻�, ���� ���� ����."
        }},
        { "Stage_6", new List<string> {
            "���ù��� ��\n\n�������� ��� ��ȭ�ϰ� ������.. ������ ħ���� ������ �ʴ´�. ������ �� ��� ������ �־�.",
            "���ù��� ��\n\n�Դٰ�... �� ��ô��.. �� �ٸ� ���籺.",
            "���ù��� ��\n\n..���� ���� ���Ѻ��� �ִ�."
        }},
        { "Stage_7", new List<string> {
            "���ù��� ��\n\n��ȭ�� ��ӵǰ� ������..",
            "���ù��� ��\n\n������ ������ ������ �ֽ��ϰ� �ִ� ���� ������ ���"
        }},
        { "Stage_8", new List<string> {
            "���ù��� ��\n\n..���� ������ �ʴ±�.",
            "���ù��� ��\n\n�����̶� �̷� �ǰ�... �������� ���� �ӿ��� ���߾� �Ѵ�."
        }},
        { "Stage_9", new List<string> {
            "���ù��� ��\n\n����... �ٸ� ���������� ���� �޶�.",
            "���ù��� ��\n\n������Ͱ� ��ȭ���� ���� ���Ƿ� ���� �� �־�",
            "���ù��� ��\n\n����, ������ ���ϴ°Ŵ�."
        }},
    };

    
    private Dictionary<int, string> stageClearUnlockObjectNames = new()
    {
        { 2, "UnlockObject_3" }, // Stage_3
        { 4, "UnlockObject_5" }, // Stage_5
        { 6, "UnlockObject_7" }  // Stage_7
    };

    public Dictionary<int, string> StageClearUnlockObjectNames => stageClearUnlockObjectNames;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip[] bgmClips;

    public int currentWave = 0;
    public float remainingTime = 0f;
    public bool isStageClear = false;
    public int stageIndex = 1;
    public Fight_Demo fight_Demo;
    public EnemySO enemySO;
    private string lastStartedScene = "";
    private bool hasBossIntroLoaded = false;
    private bool isDialoguePlaying = false;
    public SceneSequenceManager sceneSequenceManager;
    // ---------------------------------------------------------------------------------
    public bool isFullWave = false;
    // ---------------------------------------------------------------------------------
    // 30�� ������ ���� ���� �߰�
    private float waveTimer = 0f;
    private float waveInterval = 30f;  // 30�ʸ��� ���̺� ����

    private readonly HashSet<string> allowedScenes = new HashSet<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9", "BossIntro"
    };

    [Header("Monster Spawning")]
    public GameObject[] monsterPrefabs;
    private Transform[] spawnPoints;
    private GameObject[] currentStageMonsters;
    public GameObject SkillImage;
    public int aliveMonsterCount = 0;
    private bool isStageStarted = false;
    private bool isPlayerDeadByTime = false;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public Image purificationGauge;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    private int purificationValue = 0;
    private const int maxPurification = 100;
    private Coroutine dialogueCoroutine;

    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        sceneSequenceManager = FindAnyObjectByType<SceneSequenceManager>();
        if (!allowedScenes.Contains(currentScene))
        {
            Debug.Log($"[GameManager] ���� ��({currentScene})�� ������ �ʾƼ� GameManager ������Ʈ�� �ı��մϴ�.");
            Destroy(gameObject);
            return;
        }

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitWaveTable();
            isStageStarted = false;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (fight_Demo == null)
            fight_Demo = FindObjectOfType<Fight_Demo>();
        if (enemySO == null)
            enemySO = Resources.Load<EnemySO>("Enmey");
    }


    private void InitWaveTable()
    {
        waveTable["Stage_1"] = new WaveData { limitTime = 3600, waveEnemyCounts = new List<int> { 3 } };
        waveTable["Stage_2"] = new WaveData { limitTime = 120, waveEnemyCounts = new List<int> { 7 } };
        waveTable["Stage_3"] = new WaveData { limitTime = 180, waveEnemyCounts = new List<int> { 8, 10 } };
        waveTable["Stage_4"] = new WaveData { limitTime = 240, waveEnemyCounts = new List<int> { 8, 10, 12 } };
        waveTable["Stage_5"] = new WaveData { limitTime = 300, waveEnemyCounts = new List<int> { 10, 12, 14 } };
        waveTable["Stage_6"] = new WaveData { limitTime = 360, waveEnemyCounts = new List<int> { 12, 14, 16 } };
        waveTable["Stage_7"] = new WaveData { limitTime = 420, waveEnemyCounts = new List<int> { 14, 16, 18, 20 } };
        waveTable["Stage_8"] = new WaveData { limitTime = 480, waveEnemyCounts = new List<int> { 14, 16, 18, 22 } };
        waveTable["Stage_9"] = new WaveData { limitTime = 540, waveEnemyCounts = new List<int> { 16, 18, 20, 22, 25 } };
    }

    public void StartStage(string stageName)
    {
        if (fight_Demo != null)
        {
            fight_Demo.RevivePlayer();           
            fight_Demo.UpdateHPUI();
            fight_Demo.LockPlayerControl();
        }
       // ---------------------------------------------------------------------------------
        currentWave = 0;
        isStageClear = false;
        remainingTime = waveTable[stageName].limitTime;
        aliveMonsterCount = waveTable[stageName].waveEnemyCounts[currentWave];

        // ���̺갡 1�����̸� isFullWave = true�� ����
        isFullWave = (waveTable[stageName].waveEnemyCounts.Count == 1);
        // ---------------------------------------------------------------------------------

        if (isStageStarted && lastStartedScene == stageName)
        {
            Debug.Log($"[GameManager] StartStage({stageName}) �ߺ� ȣ�� ������.");
            return;
        }

        lastStartedScene = stageName;
        isStageStarted = true;

        if (!waveTable.ContainsKey(stageName))
        {
            Debug.LogWarning($"[GameManager] ���̺� �����Ͱ� ���� ���������Դϴ�: {stageName}");
            return;
        }

        currentWave = 0;
        isStageClear = false;
        remainingTime = waveTable[stageName].limitTime;
        aliveMonsterCount = waveTable[stageName].waveEnemyCounts[currentWave];

        SetMonstersForStage(stageIndex);
        FindSpawnPoints();

        Debug.Log($"[GameManager] === �������� �ܰ�: {stageIndex + 1} ===");
        Debug.Log($"[GameManager] {stageName} �������� ���� - ���ѽð� {remainingTime / 60:F0}�� {remainingTime % 60:F0}�� / ���̺� {waveTable[stageName].waveEnemyCounts.Count}��");
        Debug.Log($"[GameManager] ù ���̺� ���� - ���� ��: {aliveMonsterCount}");

        SpawnMonsters(aliveMonsterCount);
        PlayStageBGM(stageIndex);

        if (stageStartDialogues.TryGetValue(stageName, out List<string> dialogues))
        {
            if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);

            foreach (SkillHandler handler in SkillImage.GetComponentsInChildren<SkillHandler>(true))
            {
                handler.ResetCooldownUI();
            }
            SkillImage.SetActive(false);

            dialogueCoroutine = StartCoroutine(ShowDialogueSequence(dialogues));
        }

        if (fight_Demo != null && fight_Demo.agent != null)
        {
            fight_Demo.agent.ResetPath();
            fight_Demo.agent.velocity = Vector3.zero;
            fight_Demo.isMove = false;
        }
    }
    private IEnumerator ClearDialogueAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dialogueText != null)
            dialogueText.text = "";
    }

    private void PlayStageBGM(int index)
    {
        if (bgmClips == null || bgmClips.Length == 0 || bgmSource == null)
        {
            Debug.LogWarning("[GameManager] BGM ��� ����: Ŭ�� �Ǵ� AudioSource�� �����ϴ�.");
            return;
        }

        if (index >= 0 && index < bgmClips.Length && bgmClips[index] != null)
        {
            bgmSource.volume = 0.06f;
            bgmSource.clip = bgmClips[index];
            bgmSource.loop = true;
            bgmSource.Play();
            Debug.Log($"[GameManager] Stage {index + 1} BGM ��� ����");
        }
        else
        {
            Debug.LogWarning($"[GameManager] Stage {index + 1}�� �´� BGM Ŭ���� �����ϴ�.");
        }
    }

    private void FindSpawnPoints()
    {
        GameObject spawnerGroup = GameObject.Find("SpawnPoints");
        if (spawnerGroup != null)
        {
            var allPoints = spawnerGroup.GetComponentsInChildren<Transform>();
            spawnPoints = allPoints.Length > 1 ? allPoints[1..] : new Transform[0];
        }
        else
        {
            Debug.LogError("[GameManager] 'SpawnPoints' ������Ʈ�� ������ ã�� �� �����ϴ�.");
            spawnPoints = new Transform[0];
        }
    }

    private void Update()
    {
        if (!allowedScenes.Contains(SceneManager.GetActiveScene().name))
            return;

        string currentScene = SceneManager.GetActiveScene().name;

        // ��ȭ ���̸� �Է� ���
        if (isDialoguePlaying)
        {
            fight_Demo.SetInputLock(true);
            return;
        }
        else
        {
            fight_Demo.SetInputLock(false);
        }

        // ���̺� ������ ������������ �ٸ��� ����
        waveInterval = (currentScene == "Stage_7" || currentScene == "Stage_8" || currentScene == "Stage_9") ? 50f : 30f;

        remainingTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        if (timerText != null)
            timerText.text = $"{minutes:D2}:{seconds:D2}";

        if (!isStageClear && waveTable.ContainsKey(currentScene) &&
            waveTable[currentScene].waveEnemyCounts.Count > 1)
        {
            waveTimer += Time.deltaTime;

            if (waveTimer >= waveInterval)
            {
                waveTimer = 0f;
                Debug.Log($"[GameManager] {waveInterval}�� ��� �� NextWave() �ڵ� ȣ��");
                NextWave();
            }
        }

        // �׽�Ʈ Ű: 3 �� Stage_3 ���� �̵�
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            stageIndex = 2;
            sceneSequenceManager.currentSceneIndex = 2;
            Debug.Log("[GameManager] �׽�Ʈ Ű(3) �Է� �� Stage_3�� �̵�");
            if (FadeManager.Instance != null)
                FadeManager.Instance.LoadSceneWithFade("Stage_3");
            else
                SceneManager.LoadScene("Stage_3");


            purificationGauge.fillAmount = 0.2f;
        }


        // �׽�Ʈ Ű: 9 �� Stage_9 ���� �̵�
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            stageIndex = 8;
            sceneSequenceManager.currentSceneIndex = 8;
            Debug.Log("[GameManager] �׽�Ʈ Ű(9) �Է� �� Stage_9�� �̵�");
            if (FadeManager.Instance != null)
                FadeManager.Instance.LoadSceneWithFade("Stage_9");
            else
                SceneManager.LoadScene("Stage_9");


            purificationGauge.fillAmount = 0.8f;
        }

        // �׽�Ʈ Ű: �����̽� �� �������� Ŭ���� ���·� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isStageClear = true;
        }

        // ���� ��Ʈ�η� ����
        if (!hasBossIntroLoaded && Input.GetKeyDown(KeyCode.Alpha9) &&
            currentScene == "Stage_9" && isStageClear)
        {
            hasBossIntroLoaded = true;
            Debug.Log("���� ��Ʈ�� �� �ε� �õ�!");

            if (FadeManager.Instance != null)
            {
                Debug.Log("���̵� �Ŵ��� ���� BossIntro �ε�");
                FadeManager.Instance.LoadSceneWithFade("BossIntro");
                StartCoroutine(DestroyLater());
            }
            else
            {
                Debug.Log("���� BossIntro �ε�");
                SceneManager.LoadScene("BossIntro");
            }
        }

        if (!isPlayerDeadByTime && remainingTime <= 0f)
        {
            fight_Demo.Dead();
            timerText.gameObject.SetActive(false);
            isPlayerDeadByTime = true;
        }

    }
    private IEnumerator DestroyLater()
    {
        yield return new WaitForSeconds(1f); // FadeOut �ð� ���
        Destroy(gameObject);
    }

    public void NextWave()
    {
        // ---------------------------------------------------------------------------------
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
        {
            Debug.Log("[GameManager] ��� ���̺� �Ϸ�!");
            isFullWave = true; // ������ ���̺� �Ϸ� ǥ��
            return;
        }

        currentWave++;
        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
            isFullWave = true;
        aliveMonsterCount += waveTable[currentScene].waveEnemyCounts[currentWave];
        // ---------------------------------------------------------------------------------
        Debug.Log($"[GameManager] ���̺� {currentWave + 1} ���� - ���� ��: {aliveMonsterCount}");
        SpawnMonsters(aliveMonsterCount);
    }

    private void SpawnMonsters(int count)
    {
        if (currentStageMonsters == null || currentStageMonsters.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[GameManager] ���� ������ �Ǵ� ��������Ʈ�� �������� �ʾҽ��ϴ�.");
            return;
        }

        aliveMonsterCount = count;

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject prefabToSpawn = currentStageMonsters[Random.Range(0, currentStageMonsters.Length)];
            Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
        }

        Debug.Log($"[GameManager] ���� {count}���� ���� �Ϸ�");
    }

    public void OnMonsterKilled()
    {
        aliveMonsterCount--;
        Debug.Log($"[GameManager] ���� ��� �� ���� ��: {aliveMonsterCount}");

        if (aliveMonsterCount > 0)
            return;
    
        DieCcount();
    }


    public void LoadNextStage()
    {
        if (SceneSequenceManager.Instance == null)
        {
            Debug.LogError("[GameManager] SceneSequenceManager.Instance �� �������� �ʽ��ϴ�!");
            return;
        }

        SceneSequenceManager.Instance.AdvanceToNextScene();
        string nextSceneName = SceneSequenceManager.Instance.GetCurrentScene();
        stageIndex = SceneSequenceManager.Instance.currentSceneIndex;

        if (purificationGauge != null)
        {
            purificationValue = Mathf.Min(purificationValue + 10, maxPurification);
            purificationGauge.fillAmount = purificationValue / 100f;
        }

        Debug.Log($"[GameManager] ���� ���������� �̵�: {nextSceneName} (���� �������� �ܰ�: {stageIndex + 1})");

        if (nextSceneName == "Stage_9" && stageIndex >= 8)
        {
            gameObject.SetActive(false);
            SceneManager.LoadScene("BossIntro");
            Debug.Log($"[GameManager] GameManager �ı��� (���� stageIndex: {stageIndex + 1})");
            Destroy(gameObject);
            return;
        }

        if (FadeManager.Instance != null)
            FadeManager.Instance.LoadSceneWithFade(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (allowedScenes.Contains(scene.name) && scene.name != "BossIntro")
        {
            stageIndex = SceneSequenceManager.Instance.currentSceneIndex;
            isStageStarted = false;
            StartStage(scene.name);
        }
    }

    private void DieCcount()
    {
        if (aliveMonsterCount <= 0 && isFullWave)
        {
            Debug.Log("dd");
            isStageClear = true;

        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void SetMonstersForStage(int stageIndex)
    {
        List<GameObject> monstersToSpawn = new List<GameObject>();

        if (stageIndex == 0)
            monstersToSpawn.Add(monsterPrefabs[0]);
        else if (stageIndex == 1)
            monstersToSpawn.Add(monsterPrefabs[0]);
        else if (stageIndex == 2)
            monstersToSpawn.Add(monsterPrefabs[0]);
        else if (stageIndex == 3)
            monstersToSpawn.AddRange(new[] { monsterPrefabs[0], monsterPrefabs[1] });
        else if (stageIndex == 4)
            monstersToSpawn.AddRange(new[] { monsterPrefabs[0], monsterPrefabs[1] });
        else if (stageIndex == 5)
            monstersToSpawn.Add(monsterPrefabs[2]);
        else if (stageIndex == 6)
            monstersToSpawn.AddRange(new[] { monsterPrefabs[2], monsterPrefabs[3] });
        else if (stageIndex == 7)
            monstersToSpawn.AddRange(new[] { monsterPrefabs[2], monsterPrefabs[3] });
        else if (stageIndex == 8)
            monstersToSpawn.AddRange(new[] { monsterPrefabs[0], monsterPrefabs[2], monsterPrefabs[3] });
        else
            monstersToSpawn.AddRange(monsterPrefabs);

        currentStageMonsters = monstersToSpawn.ToArray();
    }
    private IEnumerator ShowDialogueSequence(List<string> lines, float delayPerLine = 2.5f, float charInterval = 0.05f)
    {
        isDialoguePlaying = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);


        foreach (string line in lines)
        {
            if (dialogueText != null)
            {
                dialogueText.text = "";
                foreach (char c in line)
                {
                    dialogueText.text += c;
                    yield return new WaitForSeconds(charInterval);
                }
            }

            yield return new WaitForSeconds(delayPerLine);
        }

        if (dialogueText != null)
            dialogueText.text = "";

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isDialoguePlaying = false;
        waveTimer = 0f; // ��ȭ �������Ƿ� 30�� ī��Ʈ ����

        if (SkillImage != null)
            SkillImage.SetActive(true);

        if (fight_Demo != null)
        {
            fight_Demo.UnlockPlayerControl();
        }
    }
    public bool IsDialoguePlaying()
    {
        return isDialoguePlaying;
    }
}
