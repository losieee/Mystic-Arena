using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrivateScene : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    public static PrivateScene Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameManager manager = FindAnyObjectByType<GameManager>();
        if (manager != null)
        {
            gameManager = manager.gameObject;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (gameManager != null)
                Destroy(gameManager);

            SceneManager.LoadScene("BossIntro");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (gameManager != null)
                Destroy(gameManager);
            // 보스가 안 움직일때 만 사용
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name); // 씬 이름으로 다시 로
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (gameManager != null)
                Destroy(gameManager);

            SceneManager.LoadScene("BossCut");
        }
    }
}
