using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BiomAttributes", menuName ="Minecraft/Biome Attribute") ]
public class BiomeAttribute : ScriptableObject
{
    public string biomeName;

    public int solidGroundHeight;
    public int terrainHeight;

    // 노이즈에 들어갈꺼
    public float terrainScale;


    public Lode[] lodes;
}


[System.Serializable]
public class Lode 
{

    public string nodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
    public bool isSolid;

}