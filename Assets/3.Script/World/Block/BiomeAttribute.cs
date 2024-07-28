using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BiomAttributes", menuName ="Minecraft/Biome Attribute") ]
public class BiomeAttribute : ScriptableObject
{
    [Header("Biome Settings")]
    public string biomeName;
    public int offset;
    public float scale;

    public int terrainHeight;

    // 노이즈에 들어갈꺼
    public float terrainScale;

    
    [Header("Biome Blocks")]
    public byte surfaceBlock;
    public byte subSurfaceBlock;



    [Header("Major Flora")]
    public int majorFloraIndex;
    public float majorFloraZoneScale = 1.3f;

    [Range(0.1f, 1f)]
    public float majorFloraZoneThreshold = 0.6f;
    public float majorFloraPlacementScale = 15f;
    [Range(0.1f, 1f)]
    public float majorFloraPlacementThreshold = 0.2f;
    public bool placeMajorFlora = true;

    public int maxHeight = 12;
    public int minHeight = 5;


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