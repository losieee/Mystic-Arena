using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public PlayerSO playerSO;

    private void Update()
    {
        // ü�� ����
        playerSO.playerMaxHp = 200f;
        playerSO.playerCurrHp = playerSO.playerMaxHp;

    }
}
