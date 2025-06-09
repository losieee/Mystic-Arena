using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
    public int currentEnemyCount = 0;
    public float remainingTime = 0f;
    public bool isStageClear = false;
    public int stageIndex = 1;

    private readonly HashSet<string> allowedScenes = new HashSet<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9"
    };

    [Header("Monster Spawning")]
    public GameObject monsterPrefab;
    public Transform[] spawnPoints;
    private int aliveMonsterCount = 0;

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
            StartStage(currentScene);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitWaveTable()
    {
        waveTable["Stage_1"] = new WaveData { limitTime = 9999, waveEnemyCounts = new List<int> { 3 } };
        waveTable["Stage_2"] = new WaveData { limitTime = 180, waveEnemyCounts = new List<int> { 7 } };
        waveTable["Stage_3"] = new WaveData { limitTime = 210, waveEnemyCounts = new List<int> { 8, 10 } };
        waveTable["Stage_4"] = new WaveData { limitTime = 220, waveEnemyCounts = new List<int> { 8, 10, 12 } };
        waveTable["Stage_5"] = new WaveData { limitTime = 250, waveEnemyCounts = new List<int> { 10, 12, 14 } };
        waveTable["Stage_6"] = new WaveData { limitTime = 260, waveEnemyCounts = new List<int> { 12, 14, 16 } };
        waveTable["Stage_7"] = new WaveData { limitTime = 280, waveEnemyCounts = new List<int> { 14, 16, 18, 20 } };
        waveTable["Stage_8"] = new WaveData { limitTime = 360, waveEnemyCounts = new List<int> { 14, 16, 18, 22 } };
        waveTable["Stage_9"] = new WaveData { limitTime = 420, waveEnemyCounts = new List<int> { 16, 18, 20, 22, 25 } };
    }

    public void StartStage(string stageName)
    {
        if (!waveTable.ContainsKey(stageName))
        {
            Debug.LogWarning($"[GameManager] 웨이브 데이터가 없는 스테이지입니다: {stageName}");
            return;
        }

        currentWave = 0;
        isStageClear = false;
        remainingTime = waveTable[stageName].limitTime;
        currentEnemyCount = waveTable[stageName].waveEnemyCounts[currentWave];

        Debug.Log($"[GameManager] === 스테이지 단계: {stageIndex + 1} ===");
        Debug.Log($"[GameManager] {stageName} 스테이지 시작 - 제한시간 {remainingTime}초 / 웨이브 {waveTable[stageName].waveEnemyCounts.Count}개");
        Debug.Log($"[GameManager] 첫 웨이브 시작 - 몬스터 수: {currentEnemyCount}");

        SpawnMonsters(currentEnemyCount);
    }

    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (allowedScenes.Contains(currentScene))
        {
            remainingTime -= Time.deltaTime;

            // 테스트 키
            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("[GameManager] 테스트 키(K) 입력 → NextWave() 호출");
                NextWave();
            }

            if (remainingTime <= 0f)
            {
                Debug.Log($"[GameManager] 스테이지 제한시간 초과 - 게임 오버 처리 가능");
            }
        }
    }

    public void NextWave()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
        {
            Debug.Log($"[GameManager] 모든 웨이브 완료!");
            isStageClear = true;
            return;
        }

        currentWave++;
        currentEnemyCount = waveTable[currentScene].waveEnemyCounts[currentWave];

        Debug.Log($"[GameManager] 웨이브 {currentWave + 1} 시작 - 몬스터 수: {currentEnemyCount}");
        SpawnMonsters(currentEnemyCount);
    }

    private void SpawnMonsters(int count)
    {
        if (monsterPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[GameManager] 몬스터 프리팹 또는 스폰포인트가 설정되지 않았습니다.");
            return;
        }

        aliveMonsterCount = count;

        Debug.Log($"[GameManager] 몬스터 {count}마리 스폰 시작");

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject monster = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);

            // 몬스터에 GameManager 연결 (MeteorDamage에서 사용 가능)
            MeteorDamage md = monster.GetComponent<MeteorDamage>();
            if (md != null)
            {
                md.gameManager = this;
            }
        }

        Debug.Log($"[GameManager] 몬스터 {count}마리 스폰 완료");
    }

    public void OnMonsterKilled()
    {
        aliveMonsterCount--;
        Debug.Log($"[GameManager] 몬스터 사망 → 남은 수: {aliveMonsterCount}");

        if (aliveMonsterCount <= 0)
        {
            Debug.Log("[GameManager] 웨이브 클리어!");
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

        Debug.Log($"[GameManager] 다음 스테이지로 이동: {nextSceneName} (현재 스테이지 단계: {stageIndex + 1})");

        if (nextSceneName == "Stage_9" && stageIndex >= 8)
        {
            if (FadeManager.Instance != null)
                FadeManager.Instance.LoadSceneWithFade("GameClearScene");
            else
                SceneManager.LoadScene("GameClearScene");

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

            Debug.Log($"[GameManager] 씬 로드 완료됨: {scene.name} → StartStage() 자동 호출 / stageIndex={stageIndex + 1}");

            StartStage(scene.name);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
