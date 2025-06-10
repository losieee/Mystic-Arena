using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public EnemySO EnemySO;
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;  // Animator ���� �߰�

    public float chaseRange = 20f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Animator ������Ʈ �Ҵ�
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

            // ���� ���� ���� ������ �޸��� ����
            if (distance <= chaseRange)
            {
                agent.SetDestination(player.position);
                //  animator.SetBool("isRunning", true);  // �޸��� �ִϸ��̼� ����
            }
            else
            {
                agent.SetDestination(transform.position); // ����
                animator.SetBool("isRunning", false); // ��� �ִϸ��̼� ����
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
