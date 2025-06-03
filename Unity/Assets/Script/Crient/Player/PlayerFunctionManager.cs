using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFunctionManager : MonoBehaviour
{
    public SceneLoad sceneLoad;
    public GameObject playerPoint;

    private void Start()
    {
        sceneLoad = GetComponent<SceneLoad>();
    }
    private void Awake()
    {
        if (sceneLoad == null)
        {
            sceneLoad = FindObjectOfType<SceneLoad>();
        }

            playerPoint = GameObject.Find("playerPoint");

        if (playerPoint != null)
        {
            transform.position = playerPoint.transform.position;
        }
        else
        {
            Debug.LogWarning("playerPoint ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }
}
