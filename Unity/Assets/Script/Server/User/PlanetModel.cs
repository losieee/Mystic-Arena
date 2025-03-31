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
    public int metal;
    public int crystal;
    public int deuterium;
    public List<PlanetModel> Planets;


    public PlanetModel(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.metal = 0;         // 추후 변경
        this.crystal = 0;
        this.deuterium = 0;
    }


}
