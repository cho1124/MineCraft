using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
    // ����ũ����Ʈ ����(Voxel World)�� �����ϱ����� ����Ƽ�� 3d������Ʈ�� cube, �� ť�� �ȿ� �ִ� box collider�� ���������� ��ġ�ϸ�
    // ��û�� ���귮 ������ ��ǻ�Ͱ� ��Ƽ�� ���Ѵ�
    // �׷��� �ʿ��� �κи� ó���Ѵٴ��� ����ȭ�� �ʿ��ϹǷ�
    // �̹� ������Ʈ�� ����ũ����Ʈ �����ý������� ���� + ����ȭ + RPG ������
    //
    //
    // <<24�� 7�� 7��>> �������� ũ��
    // Voxel Data ���� �ۼ� -> �ۼ��Ȱɷ� Chunk ���� -> Chunk�� ���� �����ϰ�
    // Player�� ������ + ��� ���� ���ġ�� ��
    // 
    // ���� �� ��ũ��Ʈ�� World�� �����ϴ� �������� ������ �����ϴ� ��ũ��Ʈ
    // ť���� ���� �����͸� ���� -> �Ѹ��� �ﰢ�� 2���� ó��
    // (�ﰢ������ ó���ϴ� ������ GPU�� �ﰢ���� ó���ϴµ� ����ȭ �Ǿ��ְ� ������ 3D������ �ﰢ���� ��������
    // ���صǾ� ������ �� (�� GPU ����� �ﰢ������ �����ϴ°� ȿ�����̶�� �Ҹ�)
    // + ���� ����, �浹�˻簡 �ܼ������� �ؽ��� ���ε� �������� ���.... )
    // 
    //
    //
    //
    //
    //
    //
    //

    // �� 6�� ����
    public const int backFace = 0;
    public const int frontFace = 1;
    public const int topFace = 2;
    public const int bottomFace = 3;
    public const int leftFace = 4;
    public const int rightFace = 5;



    // Chunk ������, World������, �ֺ��� �󸶳� chunk�� ���̰� �Ұ���
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
    //       7 �������� 6    
    //     / ��       / ��
    //   3 �������� 2   ��
    //   ��  ��     ��  ��
    //   ��  4����������5  
    //   ��/        ��/
    //   0 �������� 1
    //


    // �� ������ ���� �ﰢ��, �׷����� ����ȭ ������ ���� ��ġ�� �κ��� ��������
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

    // �� ���� �ٱ��� ������ üũ�ϱ� ����...
    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0.0f, 0.0f, -1.0f),  // back    (-z)
        new Vector3(0.0f, 0.0f, +1.0f),  // front   (+z)
        new Vector3(0.0f, +1.0f, 0.0f),  // top     (+y)
        new Vector3(0.0f, -1.0f, 0.0f),  // bottom  (-y)
        new Vector3(-1.0f, 0.0f, 0.0f),  // left    (-x)
        new Vector3(+1.0f, 0.0f, 0.0f)   // right   (+x)

    };


    // �� ���� �ﰢ��
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


    // �ؽ��� �̹����� ��ǥ -> ��� ��Ʋ���� ��ǥ
    public static readonly Vector2[] voxelUvs = new Vector2[4]
{
        new Vector2(0.0f, 0.0f), // LB
        new Vector2(0.0f, 1.0f), // LT
        new Vector2(1.0f, 0.0f), // RB
        new Vector2(1.0f, 1.0f), // RT
     };





}
