using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public string playerName;
    public int metal;
    public int crystal;
    public int deuterium;
    public List<PlanetModel> Planets;
    public PlayerModel(string name)
    {
        this.playerName = name;
        this.metal = 0;
        this.crystal = 0;
        this.deuterium = 0;
    }

    public void CollectResources()
    {
        metal += 10;
        crystal += 5;
        deuterium += 2;
    }
}
