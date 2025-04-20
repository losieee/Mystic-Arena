using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAttack : MonoBehaviour
{
    public float detectionRadius = 50f; // 감지 반경
    public float moveSpeed = 1f;

    private Transform target;
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        FindClosestTarget();

        if (target != null)
            agent.SetDestination(target.position);
    }

    void FindClosestTarget()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Player"); // 플레이어
        List<GameObject> allTargets = new List<GameObject>(potentialTargets);

        // 추가로 AI들도 감지 대상에 포함
        GameObject[] aiTargets = GameObject.FindGameObjectsWithTag("AI");

        foreach (GameObject ai in aiTargets)
        {
            // 자기 자신은 제외
            if (ai != this.gameObject)
                allTargets.Add(ai);
        }

        float closestDist = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject obj in allTargets)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < detectionRadius && dist < closestDist)
            {
                closestDist = dist;
                closestTarget = obj.transform;
            }
        }

        target = closestTarget;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}