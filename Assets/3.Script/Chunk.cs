using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    // Chunk.cs


    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world;

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        PopulateVoxelMap();
        CreateMeshData();
        CreatMesh();

    }


    void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {

                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));

                }
            }

        }
    }


    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;


        return world.blockTypes[voxelMap[x, y, z]].isSoild;
    }


    void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {

                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    //voxelMap[x, y, z] = 1; // world.GetBlockType(new Vector3(x, y, z));
 
                }
            }

        }
    }

    private void AddTextureUV(int textureID)
    {
        // ��Ʋ�� ���� �ؽ��� ����, ���� ����
        (int w, int h) = (VoxelData.textureAtlasWidth, VoxelData.textureAtlasHeight);

        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        AddTextureUV(x, y);
    }

    // �ؽ��� ��Ʋ�� ������(x, y) ��ġ�� �ؽ��� UV�� uvs ����Ʈ�� �߰�
    private void AddTextureUV(int x, int y)
    {
        //if (x < 0 || y < 0 || x >= VoxelData.TextureAtlasWidth || y >= VoxelData.TextureAtlasHeight)
        //    throw new IndexOutOfRangeException($"�ؽ��� ��Ʋ���� ������ ������ϴ� : [x = {x}, y = {y}]");

        float nw = VoxelData.NormalizedTextureAtlasWidth;
        float nh = VoxelData.NormalizedTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        // �ش� �ؽ����� uv�� LB-LT-RB-RT ������ �߰�
        uvs.Add(new Vector2(uvX, uvY));
        uvs.Add(new Vector2(uvX, uvY + nh));
        uvs.Add(new Vector2(uvX + nw, uvY));
        uvs.Add(new Vector2(uvX + nw, uvY + nh));
    }



    void CreatMesh()
    {
        // 6������ �� �׸���


        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        // ����� ������ü
        for (int p = 0; p < 6; p++)
        {
            // face check(���� �ٶ󺸴� �������� +1 �̵��Ͽ� Ȯ��) ������
            // soild(������� �ƴ�)�� �ƴѰ�쿡�� ť���� ���� �׷�������....
            // -> ûũ�� �ܰ� �κи� ���� �׷�����, ���ο��� ���� �׷����� �ʵ��� ����ȭ 

            if (CheckVoxel(pos) && !CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                byte blockID = GetBlockID(pos);

                // �� ���� �ﰢ�� 2�� �׸���
                //int triangleIndex = VoxelData.voxelTris[p, i];

                for(int i = 0; i<4; i++)
                {

                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                //vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                //vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                //vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);
                }
                //uvs.Add(VoxelData.voxelUvs[0]);
                //uvs.Add(VoxelData.voxelUvs[1]);
                //uvs.Add(VoxelData.voxelUvs[2]);
                //uvs.Add(VoxelData.voxelUvs[3]);
                //AddTextureUV(2);
                AddTextureUV(world.blockTypes[blockID].GetTextureID(p));
                //triangles.Add(vertexIndex);

                //vertexIndex++;

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);


                vertexIndex += 4;
            }
        }

    }

    private byte GetBlockID(in Vector3 pos)
    {
        return voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
    }


    // Chunk.cs

    //private void CreateMeshData()
    //{
    //    for (int y = 0; y < VoxelData.ChunkHeight; y++)
    //    {
    //        for (int x = 0; x < VoxelData.ChunkWidth; x++)
    //        {
    //            for (int z = 0; z < VoxelData.ChunkWidth; z++)
    //            {
    //                AddVoxelDataToChunk(new Vector3(x, y, z));
    //            }
    //        }
    //    }
    //}
    //


}
