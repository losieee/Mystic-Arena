using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;


[Serializable]
public class PlanetModel
{
    public int id;
    public string name;
    public int player_count;

    public List<PlanetModel> Planets;


    public PlanetModel(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.player_count = 0;

    }


}
