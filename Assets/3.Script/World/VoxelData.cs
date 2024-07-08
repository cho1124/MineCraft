using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
    // 마인크래프트 세계(Voxel World)를 구현하기위해 유니티에 3d오브젝트인 cube, 그 큐브 안에 있는 box collider를 무지성으로 배치하면
    // 엄청난 연산량 때문에 컴퓨터가 버티지 못한다
    // 그래서 필요한 부분만 처리한다던가 최적화가 필요하므로
    // 이번 프로젝트는 마인크래프트 복셀시스템으로 구현 + 최적화 + RPG 컨셉임
    //
    //
    // <<24년 7월 7일>> 기준으로 크게
    // Voxel Data 구조 작성 -> 작성된걸로 Chunk 생성 -> Chunk로 월드 생성하고
    // Player는 움직임 + 블록 생성 재배치를 함
    // 
    // 먼저 이 스크립트는 World를 구성하는 복셀들의 구조를 설계하는 스크립트
    // 큐브의 정점 데이터를 정의 -> 한면을 삼각형 2개로 처리
    // (삼각형으로 처리하는 이유는 GPU가 삼각형을 처리하는데 최적화 되어있고 복잡한 3D형상은 삼각형의 집합으로
    // 분해되어 렌더링 됨 (걍 GPU 설계상 삼각형으로 구성하는게 효율적이라는 소리)
    // + 물리 연산, 충돌검사가 단순해지고 텍스쳐 매핑도 쉬워지는 등등.... )
    // 
    //
    //
    //
    //
    //
    //
    //

    // 블럭 6면 방향
    public const int backFace = 0;
    public const int frontFace = 1;
    public const int topFace = 2;
    public const int bottomFace = 3;
    public const int leftFace = 4;
    public const int rightFace = 5;



    // Chunk 사이즈, World사이즈, 주변에 얼마나 chunk를 보이게 할건지
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 128;
    public static readonly int worldSizeInChunks = 100;
    public static readonly int viewDistanceInChunks = 10;


    public static int worldSizeInBlocks
    {
        get { return worldSizeInChunks * ChunkWidth; }
    }


    public static readonly int textureAtlasSizeInBlocks = 4;
    public static float normalizedBlockTextureSize
    {
        get { return 1f / (float)textureAtlasSizeInBlocks; }
    }




    // 
    //       7 ──── 6    
    //     / │       / │
    //   3 ──── 2   │
    //   │  │     │  │
    //   │  4───│─5  
    //   │/        │/
    //   0 ──── 1
    //


    // 각 정점을 이은 삼각형, 그렇지만 최적화 과정을 통해 겹치는 부분은 지워버림
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

    // 각 면이 바깥쪽 면인지 체크하기 위해...
    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0.0f, 0.0f, -1.0f),  // back    (-z)
        new Vector3(0.0f, 0.0f, +1.0f),  // front   (+z)
        new Vector3(0.0f, +1.0f, 0.0f),  // top     (+y)
        new Vector3(0.0f, -1.0f, 0.0f),  // bottom  (-y)
        new Vector3(-1.0f, 0.0f, 0.0f),  // left    (-x)
        new Vector3(+1.0f, 0.0f, 0.0f)   // right   (+x)

    };


    // 각 면의 삼각형
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


    // 텍스쳐 이미지의 좌표 -> 블록 아틀라스의 좌표
    public static readonly Vector2[] voxelUvs = new Vector2[4]
{
        new Vector2(0.0f, 0.0f), // LB
        new Vector2(0.0f, 1.0f), // LT
        new Vector2(1.0f, 0.0f), // RB
        new Vector2(1.0f, 1.0f), // RT
     };





}
