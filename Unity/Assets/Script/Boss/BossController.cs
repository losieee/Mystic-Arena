using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public float maxHP = 10000;
    public float currentHP;
    public GameObject dead;
    public Vector3 deadSpawnPosition;
    public Vector3 deadSpawnRotation;

    public Image hpBarImage;

    private bool isDead = false;
    public static bool portalClosed = false;
    private GameObject deadInstance;
    void Start()
    {
        currentHP = maxHP;
        UpdateHPUI();
    }
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHPUI();
    }
    private void UpdateHPUI()
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = currentHP / maxHP;
        }
    }

    private void Update()
    {
        if (currentHP <= 0 && !isDead)
        {
            isDead = true;
            Fight_Demo.isBossDead = true;

            if (dead != null)
            {
                deadInstance = Instantiate(dead, deadSpawnPosition, Quaternion.Euler(deadSpawnRotation));
            }
        }

        if (isDead)
        {
            transform.position += Vector3.down * Time.deltaTime * 1.5f;

            if (transform.position.y <= -10f && !portalClosed)
            {
                portalClosed = true;

                if (deadInstance != null && deadInstance.GetComponent<DeadShrink>() == null)
                {
                    DeadShrink shrink = deadInstance.AddComponent<DeadShrink>();
                    shrink.duration = 3f;
                }
            }

            if (transform.position.y <= -11f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
