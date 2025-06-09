using System.Collections.Generic;
using UnityEngine;

public class SceneSequenceManager : MonoBehaviour
{
    public static SceneSequenceManager Instance;

    public List<string> sceneSequence = new List<string>
    {
        "Stage_1", "Stage_2", "Stage_3", "Stage_4", "Stage_5", "Stage_6", "Stage_7", "Stage_8", "Stage_9"
    };

    public int currentSceneIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
