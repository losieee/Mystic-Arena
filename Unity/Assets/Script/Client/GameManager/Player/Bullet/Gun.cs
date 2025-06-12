using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;    // �Ѿ� ������
    public Transform firePoint;        // �ѱ� ��ġ
    public float bulletForce = 500f;   // �Ѿ� �߻� ��

    private bool canShoot = true;      // �߻� ���� ����

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Shoot();
            canShoot = false;
            StartCoroutine(ResetShoot());
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("�Ѿ˿� Rigidbody�� �����ϴ�!");
        }
    }

    IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(0.1f);  // 0.1�� ��Ÿ�� ���� ����
        canShoot = true;
    }
}
