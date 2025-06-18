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
            "선택받은 자\n\n여기가.. 차원의 균열이군",
            "선택받은 자\n\n...!! 저기 몬스터다! 어서 움직여서 처치해야겠어"
        }},
        { "Stage_2", new List<string> {
            "선택받은 자\n\n공기가… 끈적하고 기분이 나쁘군.",
            "선택받은 자\n\n이건 단순한 침입이 아니다. 차원 자체가 무너지고 있어.",
            "연합 본부 통신기\n\n곧 '침식된 자' 들이 들이닥치겠군요. 모두 정화시키세요."
        }},
        { "Stage_3", new List<string> {
            "연합 본부 통신기\n\n...여기는 본부. 정화 속도가 몬스터 출현 속도를 따라가지 못하고 있습니다.",
            "연합 본부 통신기\n\n차원 자체가 무너지고 있습니다. 지금 즉시 정화에 착수하세요. 선택받은 자."
        }},
        { "Stage_4", new List<string> {
            "선택받은 자\n\n...정화된 줄 알았던 차원 틈이 다시 열리고있어.",
            "선택받은 자\n\n뭔가 이상하군, 이전과는 다른 기척이 느껴진다."
        }},
        { "Stage_5", new List<string> {
            "선택받은 자\n\n...전투와 정화를 거듭할수록 어색한 점들이 한 두가지가 아니야. 이곳에 무언가 감춰진 진실이 있는것인가.",
            "선택받은 자\n\n이 깊이에 다다른 이상, 멈출 수는 없어."
        }},
        { "Stage_6", new List<string> {
            "선택받은 자\n\n차원문을 계속 정화하고 있지만.. 차원의 침식이 멈추지 않는다. 오히려 더 깊게 스며들고 있어.",
            "선택받은 자\n\n게다가... 이 기척은.. 또 다른 존재군.",
            "선택받은 자\n\n..무언가 나를 지켜보고 있다."
        }},
        { "Stage_7", new List<string> {
            "선택받은 자\n\n정화는 계속되고 있지만..",
            "선택받은 자\n\n차원의 저편이 이쪽을 주시하고 있는 듯한 느낌이 들어"
        }},
        { "Stage_8", new List<string> {
            "선택받은 자\n\n..끝이 보이지 않는군.",
            "선택받은 자\n\n절망이란 이런 건가... 무너지는 균형 속에서 버텨야 한다."
        }},
        { "Stage_9", new List<string> {
            "선택받은 자\n\n여긴... 다른 차원문과는 무언가 달라.",
            "선택받은 자\n\n공기부터가 정화되지 않은 악의로 가득 차 있어",
            "선택받은 자\n\n가자, 세상을 구하는거다."
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
    // 30초 간격을 위한 변수 추가
    private float waveTimer = 0f;
    private float waveInterval = 30f;  // 30초마다 웨이브 진행

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
            Debug.Log($"[GameManager] 현재 씬({currentScene})은 허용되지 않아서 GameManager 오브젝트를 파괴합니다.");
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

        // 웨이브가 1개뿐이면 isFullWave = true로 설정
        isFullWave = (waveTable[stageName].waveEnemyCounts.Count == 1);
        // ---------------------------------------------------------------------------------

        if (isStageStarted && lastStartedScene == stageName)
        {
            Debug.Log($"[GameManager] StartStage({stageName}) 중복 호출 방지됨.");
            return;
        }

        lastStartedScene = stageName;
        isStageStarted = true;

        if (!waveTable.ContainsKey(stageName))
        {
            Debug.LogWarning($"[GameManager] 웨이브 데이터가 없는 스테이지입니다: {stageName}");
            return;
        }

        currentWave = 0;
        isStageClear = false;
        remainingTime = waveTable[stageName].limitTime;
        aliveMonsterCount = waveTable[stageName].waveEnemyCounts[currentWave];

        SetMonstersForStage(stageIndex);
        FindSpawnPoints();

        Debug.Log($"[GameManager] === 스테이지 단계: {stageIndex + 1} ===");
        Debug.Log($"[GameManager] {stageName} 스테이지 시작 - 제한시간 {remainingTime / 60:F0}분 {remainingTime % 60:F0}초 / 웨이브 {waveTable[stageName].waveEnemyCounts.Count}개");
        Debug.Log($"[GameManager] 첫 웨이브 시작 - 몬스터 수: {aliveMonsterCount}");

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
            Debug.LogWarning("[GameManager] BGM 재생 실패: 클립 또는 AudioSource가 없습니다.");
            return;
        }

        if (index >= 0 && index < bgmClips.Length && bgmClips[index] != null)
        {
            bgmSource.volume = 0.06f;
            bgmSource.clip = bgmClips[index];
            bgmSource.loop = true;
            bgmSource.Play();
            Debug.Log($"[GameManager] Stage {index + 1} BGM 재생 시작");
        }
        else
        {
            Debug.LogWarning($"[GameManager] Stage {index + 1}에 맞는 BGM 클립이 없습니다.");
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
            Debug.LogError("[GameManager] 'SpawnPoints' 오브젝트를 씬에서 찾을 수 없습니다.");
            spawnPoints = new Transform[0];
        }
    }

    private void Update()
    {
        if (!allowedScenes.Contains(SceneManager.GetActiveScene().name))
            return;

        string currentScene = SceneManager.GetActiveScene().name;

        // 대화 중이면 입력 잠금
        if (isDialoguePlaying)
        {
            fight_Demo.SetInputLock(true);
            return;
        }
        else
        {
            fight_Demo.SetInputLock(false);
        }

        // 웨이브 간격을 스테이지별로 다르게 설정
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
                Debug.Log($"[GameManager] {waveInterval}초 경과 → NextWave() 자동 호출");
                NextWave();
            }
        }

        // 테스트 키: 3 → Stage_3 강제 이동
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            stageIndex = 2;
            sceneSequenceManager.currentSceneIndex = 2;
            Debug.Log("[GameManager] 테스트 키(3) 입력 → Stage_3로 이동");
            if (FadeManager.Instance != null)
                FadeManager.Instance.LoadSceneWithFade("Stage_3");
            else
                SceneManager.LoadScene("Stage_3");


            purificationGauge.fillAmount = 0.2f;
        }


        // 테스트 키: 9 → Stage_9 강제 이동
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            stageIndex = 8;
            sceneSequenceManager.currentSceneIndex = 8;
            Debug.Log("[GameManager] 테스트 키(9) 입력 → Stage_9로 이동");
            if (FadeManager.Instance != null)
                FadeManager.Instance.LoadSceneWithFade("Stage_9");
            else
                SceneManager.LoadScene("Stage_9");


            purificationGauge.fillAmount = 0.8f;
        }

        // 테스트 키: 스페이스 → 스테이지 클리어 상태로 변경
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isStageClear = true;
        }

        // 보스 인트로로 진입
        if (!hasBossIntroLoaded && Input.GetKeyDown(KeyCode.Alpha9) &&
            currentScene == "Stage_9" && isStageClear)
        {
            hasBossIntroLoaded = true;
            Debug.Log("보스 인트로 씬 로드 시도!");

            if (FadeManager.Instance != null)
            {
                Debug.Log("페이드 매니저 통해 BossIntro 로드");
                FadeManager.Instance.LoadSceneWithFade("BossIntro");
                StartCoroutine(DestroyLater());
            }
            else
            {
                Debug.Log("직접 BossIntro 로드");
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
        yield return new WaitForSeconds(1f); // FadeOut 시간 고려
        Destroy(gameObject);
    }

    public void NextWave()
    {
        // ---------------------------------------------------------------------------------
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
        {
            Debug.Log("[GameManager] 모든 웨이브 완료!");
            isFullWave = true; // 마지막 웨이브 완료 표시
            return;
        }

        currentWave++;
        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
            isFullWave = true;
        aliveMonsterCount += waveTable[currentScene].waveEnemyCounts[currentWave];
        // ---------------------------------------------------------------------------------
        Debug.Log($"[GameManager] 웨이브 {currentWave + 1} 시작 - 몬스터 수: {aliveMonsterCount}");
        SpawnMonsters(aliveMonsterCount);
    }

    private void SpawnMonsters(int count)
    {
        if (currentStageMonsters == null || currentStageMonsters.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[GameManager] 몬스터 프리팹 또는 스폰포인트가 설정되지 않았습니다.");
            return;
        }

        aliveMonsterCount = count;

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject prefabToSpawn = currentStageMonsters[Random.Range(0, currentStageMonsters.Length)];
            Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
        }

        Debug.Log($"[GameManager] 몬스터 {count}마리 스폰 완료");
    }

    public void OnMonsterKilled()
    {
        aliveMonsterCount--;
        Debug.Log($"[GameManager] 몬스터 사망 → 남은 수: {aliveMonsterCount}");

        if (aliveMonsterCount > 0)
            return;
    
        DieCcount();
    }


    public void LoadNextStage()
    {
        if (SceneSequenceManager.Instance == null)
        {
            Debug.LogError("[GameManager] SceneSequenceManager.Instance 가 존재하지 않습니다!");
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

        Debug.Log($"[GameManager] 다음 스테이지로 이동: {nextSceneName} (현재 스테이지 단계: {stageIndex + 1})");

        if (nextSceneName == "Stage_9" && stageIndex >= 8)
        {
            gameObject.SetActive(false);
            SceneManager.LoadScene("BossIntro");
            Debug.Log($"[GameManager] GameManager 파괴됨 (최종 stageIndex: {stageIndex + 1})");
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
        waveTimer = 0f; // 대화 끝났으므로 30초 카운트 시작

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
