using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;    // 총알 프리팹
    public Transform firePoint;        // 총구 위치
    public float bulletForce = 500f;   // 총알 발사 힘

    private bool canShoot = true;      // 발사 가능 상태

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
            Debug.LogWarning("총알에 Rigidbody가 없습니다!");
        }
    }

    IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(0.1f);  // 0.1초 쿨타임 조절 가능
        canShoot = true;
    }
}
