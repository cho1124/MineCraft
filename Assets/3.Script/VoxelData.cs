using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
    // 블럭 6면 방향
    public const int backFace = 0;
    public const int frontFace = 1;
    public const int topFace = 2;
    public const int bottomFace = 3;
    public const int leftFace = 4;
    public const int rightFace = 5;



    // VoxelData.cs

    public static readonly int ChunkWidth = 5;
    public static readonly int ChunkHeight = 5;

    public static readonly int textureAtlasSizeInBlocks = 4;
    public static float normalizedBlockTextureSize
    {
        get { return 1f / (float)textureAtlasSizeInBlocks; }
    }

    //// 연습용 텍스쳐 아틀라스
    //public static readonly int textureAtlasWidth = 4;
    //public static readonly int textureAtlasHeight = 4;


    //// 텍스쳐 아틀라스 내에서 각 행, 열마다 텍스쳐가 갖는 크기 비율
    //public static float NormalizedTextureAtlasWidth => 1f / textureAtlasWidth;
    //
    //public static float NormalizedTextureAtlasHeight => 1f / textureAtlasHeight;

    public static readonly Vector3[] voxelVerts = new Vector3[8] {

        // front
        new Vector3(0.0f,0.0f,0.0f), // LB 0
        new Vector3(1.0f,0.0f,0.0f), // RB 1
        new Vector3(1.0f,1.0f,0.0f), // RT 2
        new Vector3(0.0f,1.0f,0.0f), // LT 3

        // Back
        new Vector3(0.0f,0.0f,1.0f), // LB 4
        new Vector3(1.0f,0.0f,1.0f), // Rb 5
        new Vector3(1.0f,1.0f,1.0f), // RT 6
        new Vector3(0.0f,1.0f,1.0f), // LT 7


    };


    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0.0f, 0.0f, -1.0f), 
        new Vector3(0.0f, 0.0f, 1.0f), 
        new Vector3(0.0f, 1.0f, 0.0f), 
        new Vector3(0.0f, -1.0f, 0.0f), 
        new Vector3(-1.0f, 0.0f, 0.0f), 
        new Vector3(1.0f, 0.0f, 0.0f)

    };


    public static readonly int[,] voxelTris = new int[6, 4]
    {

        //back
        //
        // 0 - 3
        // | \ |
        // 1 - 2
        {0,3,1,2 },


        // front
        //
        // 5 - 6
        // | \ |
        // 4 - 7
        {5,6,4, 7 },

        // top
        //
        // 3 - 7
        // | \ |
        // 2 - 6 
        {3,7,2,6 },


        // bottom
        //
        // 1 - 5
        // | \ |
        // 0 - 4
        {1,5,0,4 },
        

        // left
        //
        // 4 - 7
        // | \ |
        // 0 - 3
        {4,7,0,3 },
        
        // right
        //
        // 1 - 2
        // | \ |
        // 5 - 6
        {1,2,5,6 }
    };


    // 텍스트 넣을 위치
    public static readonly Vector2[] voxelUvs = new Vector2[4]
{
        new Vector2(0.0f, 0.0f), // LB
        new Vector2(0.0f, 1.0f), // LT
        new Vector2(1.0f, 0.0f), // RB
        new Vector2(1.0f, 1.0f), // RT
     };





}
