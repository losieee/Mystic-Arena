using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

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

    public int currentWave = 0;
    public float remainingTime = 0f;
    public bool isStageClear = false;
    public int stageIndex = 1;

    private string lastStartedScene = "";

    private readonly HashSet<string> allowedScenes = new HashSet<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9"
    };

    [Header("Monster Spawning")]
    public GameObject[] monsterPrefabs;
    private Transform[] spawnPoints;
    private GameObject[] currentStageMonsters;
    public int aliveMonsterCount = 0;
    private bool isStageStarted = false;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public Slider purificationGauge;
    private int purificationValue = 0;
    private const int maxPurification = 100;

    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

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
    }

    private void FindSpawnPoints()
    {
        GameObject spawnerGroup = GameObject.Find("SpawnPoints");
        if (spawnerGroup != null)
        {
            var allPoints = spawnerGroup.GetComponentsInChildren<Transform>();
            spawnPoints = allPoints.Length > 1 ? allPoints[1..] : new Transform[0];
            Debug.Log($"[GameManager] 스폰 포인트 {spawnPoints.Length}개 로드됨.");
        }
        else
        {
            Debug.LogError("[GameManager] 'SpawnPoints' 오브젝트를 씬에서 찾을 수 없습니다.");
            spawnPoints = new Transform[0];
        }
    }

    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (allowedScenes.Contains(currentScene))
        {
            remainingTime -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            if (timerText != null)
                timerText.text = $"{minutes:D2}:{seconds:D2}";

            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("[GameManager] 테스트 키(K) 입력 → NextWave() 호출");
                NextWave();
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Debug.Log("[GameManager] 테스트 키(9) 입력 → Stage_9로 이동");
                stageIndex = 8;
                if (FadeManager.Instance != null)
                    FadeManager.Instance.LoadSceneWithFade("Stage_9");
                else
                    SceneManager.LoadScene("Stage_9");
            }

            if (Input.GetKeyDown(KeyCode.F) && currentScene == "Stage_9" && isStageClear)
            {
                Debug.Log("[GameManager] Stage_9 클리어 후 F키 입력 → BossIntro로 이동");
                if (FadeManager.Instance != null)
                    FadeManager.Instance.LoadSceneWithFade("BossIntro");
                else
                    SceneManager.LoadScene("BossIntro");
                Destroy(gameObject, 3);
                return;
            }

            if (remainingTime <= 0f)
            {
                Debug.Log("[GameManager] 스테이지 제한시간 초과 - 게임 오버 처리 가능");
            }
        }
    }

    public void NextWave()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
        {
            Debug.Log("[GameManager] 모든 웨이브 완료!");
            isStageClear = true;
            return;
        }

        currentWave++;
        aliveMonsterCount = waveTable[currentScene].waveEnemyCounts[currentWave];

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
        Debug.Log($"[GameManager] 몬스터 {count}마리 스폰 시작");

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

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
        {
            Debug.Log("[GameManager] 모든 웨이브 완료 → 스테이지 클리어!");
            isStageClear = true;
        }
        else
        {
            NextWave();
        }
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
            purificationGauge.value = purificationValue / 100f;
        }

        Debug.Log($"[GameManager] 다음 스테이지로 이동: {nextSceneName} (현재 스테이지 단계: {stageIndex + 1})");

        if (nextSceneName == "Stage_9" && stageIndex >= 8)
        {
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
        if (allowedScenes.Contains(scene.name))
        {
            stageIndex = SceneSequenceManager.Instance.currentSceneIndex;
            isStageStarted = false;
            Debug.Log($"[GameManager] 씬 로드 완료됨: {scene.name} → StartStage() 자동 호출 / stageIndex={stageIndex + 1}");
            StartStage(scene.name);
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
}