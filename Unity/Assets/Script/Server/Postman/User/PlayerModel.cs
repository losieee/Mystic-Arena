using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public string playerName;
    public int player_count;
 
    public List<PlanetModel> Planets;
    public PlayerModel(string name)
    {
        this.playerName = name;
        this.player_count = 0;
    }

    public void CollectResources()
    {
        player_count += 0;              // ÃßÈÄ Á¡¼ö È¹µæ½Ã »ç¿ë
    }
}
