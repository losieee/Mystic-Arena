using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSequenceManager : MonoBehaviour
{
    public static SceneSequenceManager Instance;

    public List<string> sceneSequence = new List<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9"
    };

    public int currentSceneIndex = 0;

    private readonly HashSet<string> allowedScenes = new HashSet<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9"
    };

    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (!allowedScenes.Contains(currentScene))
        {
            Debug.Log($"[SceneSequenceManager] 현재 씬({currentScene})은 허용되지 않아 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[SceneSequenceManager] 초기화 완료");
    }

    public void AdvanceToNextScene()
    {
        if (currentSceneIndex + 1 < sceneSequence.Count)
        {
            currentSceneIndex++;
            Debug.Log($"[SceneSequenceManager] currentSceneIndex 증가됨 → {currentSceneIndex}");
        }
        else
        {
            Debug.Log("[SceneSequenceManager] 모든 스테이지 완료됨 → 마지막 Stage 유지");
        }
    }

    public string GetCurrentScene()
    {
        return sceneSequence[currentSceneIndex];
    }

    public string PeekNextScene()
    {
        if (currentSceneIndex + 1 < sceneSequence.Count)
        {
            return sceneSequence[currentSceneIndex + 1];
        }
        else
        {
            return null; // 마지막 스테이지 도달
        }
    }
}
