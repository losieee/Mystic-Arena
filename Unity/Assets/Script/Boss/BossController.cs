using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public float maxHP = 10000;
    public float currentHP;

    public Image hpBarImage;
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
