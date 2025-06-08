using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 웨이브 데이터 구조
    [System.Serializable]
    public class WaveData
    {
        public int limitTime;      // 제한 시간 (초)
        public List<int> waveEnemyCounts; // 각 웨이브별 몬스터 수
    }

    public Dictionary<string, WaveData> waveTable = new Dictionary<string, WaveData>();

    public int currentWave = 0;          // 현재 웨이브 인덱스 (0부터 시작)
    public int currentEnemyCount = 0;    // 현재 웨이브 몬스터 수
    public float stageTimer = 0f;        // 현재 스테이지 타이머

    public bool isStageClear = false;    // 스테이지 클리어 여부

    // 유지할 씬 이름 리스트
    private readonly HashSet<string> allowedScenes = new HashSet<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9"
    };

    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // 유지할 씬이 아니라면 파괴
        if (!allowedScenes.Contains(currentScene))
        {
            Debug.Log($"[GameManager] 현재 씬({currentScene})은 허용되지 않아서 GameManager 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        // 싱글톤 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환해도 유지
            InitWaveTable();               // 웨이브 테이블 초기화
            StartStage(currentScene);      // 현재 씬에 맞게 스테이지 시작
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    private void InitWaveTable()
    {
        // Stage 1 → 웨이브 X → 몬스터 3마리 고정
        waveTable["Stage_1"] = new WaveData { limitTime = 9999, waveEnemyCounts = new List<int> { 3 } };

        // Stage 2~9 웨이브 정보 설정
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
        stageTimer = 0f;
        currentEnemyCount = waveTable[stageName].waveEnemyCounts[currentWave];
        isStageClear = false; // 스테이지 시작 시 초기화

        if (stageName == "Stage_1")
        {
            Debug.Log($"[GameManager] Stage 1 시작 - 웨이브 없음 / 고정 몬스터 수: {currentEnemyCount}");
            SpawnMonsters(currentEnemyCount); // 몬스터 3마리 스폰 호출
        }
        else
        {
            Debug.Log($"[GameManager] {stageName} 스테이지 시작 - 제한시간 {waveTable[stageName].limitTime}초 / 웨이브 {waveTable[stageName].waveEnemyCounts.Count}개");
            Debug.Log($"[GameManager] 첫 웨이브 시작 - 몬스터 수: {currentEnemyCount}");
            SpawnMonsters(currentEnemyCount); // 첫 웨이브 몬스터 스폰
        }
    }

    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (allowedScenes.Contains(currentScene))
        {
            stageTimer += Time.deltaTime;

            // Stage 1 은 웨이브 없음 → 웨이브 진행 X
            if (currentScene != "Stage_1")
            {
                // 수동 테스트용: K 키 → 다음 웨이브 진행
                if (Input.GetKeyDown(KeyCode.K))
                {
                    NextWave();
                }

                // 제한시간 초과 체크
                if (stageTimer > waveTable[currentScene].limitTime)
                {
                    Debug.Log($"[GameManager] 스테이지 제한시간 초과 - 게임 오버 처리 가능");
                }
            }
        }
    }

    public void NextWave()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentWave + 1 >= waveTable[currentScene].waveEnemyCounts.Count)
        {
            Debug.Log($"[GameManager] 모든 웨이브 완료!");
            isStageClear = true; // 스테이지 클리어 처리
            Debug.Log($"[GameManager] Stage Clear 상태로 변경됨");
            return;
        }

        currentWave++;
        currentEnemyCount = waveTable[currentScene].waveEnemyCounts[currentWave];

        Debug.Log($"[GameManager] 웨이브 {currentWave + 1} 시작 - 몬스터 수: {currentEnemyCount}");
        SpawnMonsters(currentEnemyCount); // 다음 웨이브 몬스터 스폰
    }

    // 몬스터 스폰 예시 (추후 SpawnManager와 연동 추천)
    private void SpawnMonsters(int count)
    {
        Debug.Log($"[GameManager] 몬스터 {count}마리 스폰 (SpawnMonsters 호출됨)");
        // 실제로는 SpawnManager.Instance.Spawn(count); 같은 식으로 구현 가능
    }
}
