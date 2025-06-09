using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeteorDamage : MonoBehaviour
{
    public EnemySO EnemySO;
    private NavMeshAgent agent;
    private Transform player;

    public float chaseRange = 20f;
    public float maxHP = 100f;
    private float currentHP;

    public GameManager gameManager;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        gameManager = GameManager.instance;

        currentHP = maxHP;
    }

    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= chaseRange)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.SetDestination(transform.position); // ¸ØÃã
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
                playerComponent.TakeDamage(70f);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"[{gameObject.name}] ÇÇ°Ý: {damage} ¡æ ³²Àº HP: {currentHP}");

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[{gameObject.name}] »ç¸Á Ã³¸®");

        if (gameManager != null)
        {
            gameManager.OnMonsterKilled();
        }

        Destroy(gameObject);
    }
}
