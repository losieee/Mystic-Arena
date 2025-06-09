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
            Debug.Log($"[GameManager] ���� ��({currentScene})�� ������ �ʾƼ� GameManager ������Ʈ�� �ı��մϴ�.");
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
            Debug.LogWarning($"[GameManager] ���̺� �����Ͱ� ���� ���������Դϴ�: {stageName}");
            return;
        }

        currentWave = 0;
        isStageClear = false;
        remainingTime = waveTable[stageName].limitTime;
        currentEnemyCount = waveTable[stageName].waveEnemyCounts[currentWave];

        Debug.Log($"[GameManager] === �������� �ܰ�: {stageIndex + 1} ===");
        Debug.Log($"[GameManager] {stageName} �������� ���� - ���ѽð� {remainingTime}�� / ���̺� {waveTable[stageName].waveEnemyCounts.Count}��");
        Debug.Log($"[GameManager] ù ���̺� ���� - ���� ��: {currentEnemyCount}");

        SpawnMonsters(currentEnemyCount);
    }

    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (allowedScenes.Contains(currentScene))
        {
            remainingTime -= Time.deltaTime;

            // �׽�Ʈ Ű
            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("[GameManager] �׽�Ʈ Ű(K) �Է� �� NextWave() ȣ��");
                NextWave();
            }

            if (remainingTime <= 0f)
            {
                Debug.Log($"[GameManager] �������� ���ѽð� �ʰ� - ���� ���� ó�� ����");
            }
        }
    }

    public void NextWave()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
        {
            Debug.Log($"[GameManager] ��� ���̺� �Ϸ�!");
            isStageClear = true;
            return;
        }

        currentWave++;
        currentEnemyCount = waveTable[currentScene].waveEnemyCounts[currentWave];

        Debug.Log($"[GameManager] ���̺� {currentWave + 1} ���� - ���� ��: {currentEnemyCount}");
        SpawnMonsters(currentEnemyCount);
    }

    private void SpawnMonsters(int count)
    {
        if (monsterPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[GameManager] ���� ������ �Ǵ� ��������Ʈ�� �������� �ʾҽ��ϴ�.");
            return;
        }

        aliveMonsterCount = count;

        Debug.Log($"[GameManager] ���� {count}���� ���� ����");

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject monster = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);

            // ���Ϳ� GameManager ���� (MeteorDamage���� ��� ����)
            MeteorDamage md = monster.GetComponent<MeteorDamage>();
            if (md != null)
            {
                md.gameManager = this;
            }
        }

        Debug.Log($"[GameManager] ���� {count}���� ���� �Ϸ�");
    }

    public void OnMonsterKilled()
    {
        aliveMonsterCount--;
        Debug.Log($"[GameManager] ���� ��� �� ���� ��: {aliveMonsterCount}");

        if (aliveMonsterCount <= 0)
        {
            Debug.Log("[GameManager] ���̺� Ŭ����!");
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

        Debug.Log($"[GameManager] ���� ���������� �̵�: {nextSceneName} (���� �������� �ܰ�: {stageIndex + 1})");

        if (nextSceneName == "Stage_9" && stageIndex >= 8)
        {
            if (FadeManager.Instance != null)
                FadeManager.Instance.LoadSceneWithFade("GameClearScene");
            else
                SceneManager.LoadScene("GameClearScene");

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

            Debug.Log($"[GameManager] �� �ε� �Ϸ��: {scene.name} �� StartStage() �ڵ� ȣ�� / stageIndex={stageIndex + 1}");

            StartStage(scene.name);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
