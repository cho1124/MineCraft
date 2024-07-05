using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{

    // VoxelData.cs

    public static readonly int ChunkWidth = 5;
    public static readonly int ChunkHeight = 5;

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


    public static readonly int[,] voxelTris = new int[6, 6]
    {

        //back
        //
        // 0 - 3
        // | \ |
        // 1 - 2
        {0,3,1,1,3,2 },


        // front
        //
        // 5 - 6
        // | \ |
        // 4 - 7
        {5,6,4,4,6,7 },

        // top
        //
        // 3 - 7
        // | \ |
        // 2 - 6 
        {3,7,2,2,7,6 },


        // bottom
        //
        // 1 - 5
        // | \ |
        // 0 - 4
        {1,5,0,0,5,4 },
        

        // left
        //
        // 4 - 7
        // | \ |
        // 0 - 3
        {4,7,0,0,7,3 },
        
        // right
        //
        // 1 - 2
        // | \ |
        // 5 - 6
        {1,2,5,5,2,6 }
    };


    // 텍스트 넣을 위치
    public static readonly Vector2[] voxelUvs = new Vector2[6]
{
        new Vector2(0.0f, 0.0f), // LB
        new Vector2(0.0f, 1.0f), // LT
        new Vector2(1.0f, 0.0f), // RB

        new Vector2(1.0f, 0.0f), // RB
        new Vector2(0.0f, 1.0f), // LT
        new Vector2(1.0f, 1.0f), // RT
     };
    
  



}
