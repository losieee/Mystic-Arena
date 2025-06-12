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
    }

    private void FindSpawnPoints()
    {
        GameObject spawnerGroup = GameObject.Find("SpawnPoints");
        if (spawnerGroup != null)
        {
            var allPoints = spawnerGroup.GetComponentsInChildren<Transform>();
            spawnPoints = allPoints.Length > 1 ? allPoints[1..] : new Transform[0];
            Debug.Log($"[GameManager] ���� ����Ʈ {spawnPoints.Length}�� �ε��.");
        }
        else
        {
            Debug.LogError("[GameManager] 'SpawnPoints' ������Ʈ�� ������ ã�� �� �����ϴ�.");
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
                Debug.Log("[GameManager] �׽�Ʈ Ű(K) �Է� �� NextWave() ȣ��");
                NextWave();
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Debug.Log("[GameManager] �׽�Ʈ Ű(9) �Է� �� Stage_9�� �̵�");
                stageIndex = 8;
                if (FadeManager.Instance != null)
                    FadeManager.Instance.LoadSceneWithFade("Stage_9");
                else
                    SceneManager.LoadScene("Stage_9");
            }

            if (Input.GetKeyDown(KeyCode.F) && currentScene == "Stage_9" && isStageClear)
            {
                Debug.Log("[GameManager] Stage_9 Ŭ���� �� FŰ �Է� �� BossIntro�� �̵�");
                if (FadeManager.Instance != null)
                    FadeManager.Instance.LoadSceneWithFade("BossIntro");
                else
                    SceneManager.LoadScene("BossIntro");
                Destroy(gameObject, 3);
                return;
            }

            if (remainingTime <= 0f)
            {
                Debug.Log("[GameManager] �������� ���ѽð� �ʰ� - ���� ���� ó�� ����");
            }
        }
    }

    public void NextWave()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
        {
            Debug.Log("[GameManager] ��� ���̺� �Ϸ�!");
            isStageClear = true;
            return;
        }

        currentWave++;
        aliveMonsterCount = waveTable[currentScene].waveEnemyCounts[currentWave];

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
        Debug.Log($"[GameManager] ���� {count}���� ���� ����");

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

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
        {
            Debug.Log("[GameManager] ��� ���̺� �Ϸ� �� �������� Ŭ����!");
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
            Debug.LogError("[GameManager] SceneSequenceManager.Instance �� �������� �ʽ��ϴ�!");
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

        Debug.Log($"[GameManager] ���� ���������� �̵�: {nextSceneName} (���� �������� �ܰ�: {stageIndex + 1})");

        if (nextSceneName == "Stage_9" && stageIndex >= 8)
        {
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
        if (allowedScenes.Contains(scene.name))
        {
            stageIndex = SceneSequenceManager.Instance.currentSceneIndex;
            isStageStarted = false;
            Debug.Log($"[GameManager] �� �ε� �Ϸ��: {scene.name} �� StartStage() �ڵ� ȣ�� / stageIndex={stageIndex + 1}");
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