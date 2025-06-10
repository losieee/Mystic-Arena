using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public EnemySO EnemySO;
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;  // Animator 변수 추가

    public float chaseRange = 20f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Animator 컴포넌트 할당
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // 추적 범위 내에 있으면 달리기 시작
            if (distance <= chaseRange)
            {
                agent.SetDestination(player.position);
                //  animator.SetBool("isRunning", true);  // 달리기 애니메이션 실행
            }
            else
            {
                agent.SetDestination(transform.position); // 멈춤
                animator.SetBool("isRunning", false); // 대기 애니메이션 실행
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerHitbox")
        {
            Fight_Demo playerComponent = other.GetComponentInParent<Fight_Demo>();
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(EnemySO.monsterAttack);
            }
        }
    }


    private void Die()
    {
        if(EnemySO.monstercurrHp <= 0)
        {
            gameObject.SetActive(false);
           // animator.SetBool("isDIe", true);
        }
    }
}
