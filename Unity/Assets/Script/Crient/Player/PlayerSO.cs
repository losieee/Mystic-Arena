using UnityEngine;
//----------------------------------------------------------------------------------
// 초기 플레이어 관련
[CreateAssetMenu(fileName = "Player", menuName = "GameObject/Player")]
public class PlayerSO : ScriptableObject
{
    public int player_Uid;
    public string player_Id;
    public string player_Password;
    public string player_Name;
    public float player_Speed = 5f;
    public float playre_Jump = 5f;
    public int player_Lever = 1;
    public float player_Attack = 10f;
    public float player_CurrHp = 100f;
    public float player_MaxHp = 100f;
    public float player_MinHp = 10f;
    public Rigidbody player_Rigidbody;
    public Animator player_Animator;

}
//----------------------------------------------------------------------------------
// 적 관련 
[CreateAssetMenu(fileName = "Enemy", menuName = "GameObject/Enemy")]
public class EnemySO : ScriptableObject
{
    public int enemy_Id;
    public string enemy_Name;
    public int enemy_Lever = 1;
    public float enemy_Damage = 5f;
    public float enemy_MaxHp = 50f;
    public float enmey_CurrHp = 50f;
    public float enemy_Speed = 2.5f;
    public Animator enemy_Animator;
}
//----------------------------------------------------------------------------------
// Item 관련
[CreateAssetMenu(fileName = "Item", menuName = "GameObject/Item")]
public class ItemSO : ScriptableObject
{
    public int item_Id;
    public string item_Name;
    public int item_Lever = 1;
}

//----------------------------------------------------------------------------------




