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
            // ������ �� �����϶� �� ���
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name); // �� �̸����� �ٽ� ��
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (gameManager != null)
                Destroy(gameManager);

            SceneManager.LoadScene("BossCut");
        }
    }
}
