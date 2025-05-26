//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AIRespawn : MonoBehaviour
//{
//    public GameObject aiPrefab;
//    public Transform respawnPoint;

//    public void SpawnAI(float delay)
//    {
//        StartCoroutine(SpawnAIAfterDelay(delay));
//    }

//    private IEnumerator SpawnAIAfterDelay(float delay)
//    {
//        yield return new WaitForSeconds(delay);
//        if (aiPrefab != null && respawnPoint != null)
//        {
//            GameObject ai = Instantiate(aiPrefab, respawnPoint.position, respawnPoint.rotation);

//            var fov = FindObjectOfType<FieldOfView>();
//            if (fov != null) fov.FindVisibleTargets();
//        }
//    }
//}
