using static BossWaveSpawner;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float forcePower;
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 direction, float force, Monster1 targetMonster)
    {
        forcePower = force;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * forcePower, ForceMode.Impulse);
    }

}
