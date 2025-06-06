using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAttackType
{
    sword,
    Gun,
}

[CreateAssetMenu(fileName = "Player", menuName = "Database/Player")]
public class PlayerSO : ScriptableObject
{
    public int playerSpeed = 5;
    public float playerCurrHp = 200f;
    public float playerMaxHp = 200f;
    public int playerAttack = 10;
    public PlayerAttackType playerAttackType;
}
