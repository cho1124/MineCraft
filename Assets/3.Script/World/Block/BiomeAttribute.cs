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


    [Header("Trees")]
    public float treeZoneScale = 1.3f;

    [Range(0.1f, 1f)]
    public float treeZoneThreshold = 0.6f;
    public float treePlacementScale = 15f;
    [Range(0.1f, 1f)]
    public float treePlacementThreshold = 0.2f;

    public int maxTreeHeight = 12;
    public int minTreeHeight = 5;


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