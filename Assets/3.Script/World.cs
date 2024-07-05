using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;

}


[System.Serializable]

public class BlockType
{
    public string blockName;
    public bool isSoild;

    [Header("Texture IDs")]
    public int topFaceTextureID;
    public int frontFaceTextureID;
    public int backFaceTextureID;
    public int leftFaceTextureID;
    public int rightFaceTextureID;
    public int bottomFaceTextureID;

    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case VoxelData.topFace: return topFaceTextureID;
            case VoxelData.frontFace: return frontFaceTextureID;
            case VoxelData.backFace: return backFaceTextureID;
            case VoxelData.leftFace: return leftFaceTextureID;
            case VoxelData.rightFace: return rightFaceTextureID;
            case VoxelData.bottomFace: return bottomFaceTextureID;

            default:
                throw new IndexOutOfRangeException($"Face Index must be in 0 ~ 5, but input : {faceIndex}");
        }

    }
}

