using UnityEngine;
using static BossWaveSpawner;

public enum AttackType
{
    Normal_Attack,
    Qkey_Attack,
    Ekey_Attack
}

public class Bullet : MonoBehaviour
{
    public AttackType attackType;
    public PlayerSO playerSO;
    public EnemySO enemySO;
    public float forcePower;
    public float lifeTime = 5f;

    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    // 총알 발사 방향과 힘을 설정하는 함수
    public void Shoot(Vector3 direction, float force)
    {
        forcePower = force;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * forcePower, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            switch (attackType)
            {
                case AttackType.Normal_Attack:
                    NormalAttack();
                    break;
                case AttackType.Qkey_Attack:
                    QkeyAttack();
                    break;
                case AttackType.Ekey_Attack:
                    EkeyAttack();
                    break;
            }

            enemySO.monsterHp -= playerSO.playerAttack;
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            playerSO.playerCurrHp -= enemySO.monsterAttack;
            Destroy(gameObject);
            return;
        }
    }

    public void NormalAttack()
    {
        Debug.Log("일반공격 데미지");
    }

    public void QkeyAttack()
    {
        Debug.Log("Q스킬 공격 데미지");
    }

    public void EkeyAttack()
    {
        Debug.Log("E스킬 공격 데미지");
    }
}
