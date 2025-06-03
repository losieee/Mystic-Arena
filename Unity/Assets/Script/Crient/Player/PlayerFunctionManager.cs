using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFunctionManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject spawnPoint;

    public void Awake()
    {
        spawnPoint = GameObject.Find("playerPoint");

        if (!spawnPoint)
        {
            spawnPoint = FindAnyObjectByType<GameObject>();
        }

        transform.position = spawnPoint.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    { 
  
        if (other.gameObject.CompareTag("Potal"))
        {
            transform.position = spawnPoint.transform.position;
            SceneManager.LoadScene("GameSceneRoom02");
        }
    }
}
