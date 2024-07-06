using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    int vertexIndex = 0;

    // ���� -> ���� ���ؽ��� ��� ������ -> �޽�������
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    // 2d �ؽ��ĸ� 3d �𵨷� ���εǵ���...
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world;

    void Start()
    {

        world = GameObject.Find("World").GetComponent<World>();

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();

    }

    // Chunk ���� �� ��ġ�� � ����� ������...
    void PopulateVoxelMap()
    {

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {

                    if (y < 1)
                        voxelMap[x, y, z] = 0;
                    else if (y == VoxelData.ChunkHeight - 1)
                        voxelMap[x, y, z] = 3;
                    else
                        voxelMap[x, y, z] = 1;

                }
            }
        }

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


    // ��������� �ƴ��� �Ǵ� -> �׷��� ���̴� �鸸 �׸��ϱ�...
    bool CheckVoxel(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;

        return world.blockTypes[voxelMap[x, y, z]].isSolid;

    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        // ����� 6��ü

        for (int p = 0; p < 6; p++)
        {

            // face check(���� �ٶ󺸴� �������� +1 �̵��Ͽ� Ȯ��) ������
            // soild(������� �ƴ�)�� �ƴѰ�쿡�� ť���� ���� �׷�������....
            // -> ûũ�� �ܰ� �κи� ���� �׷�����, ���ο��� ���� �׷����� �ʵ��� ����ȭ 

            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {

                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

                for(int i = 0; i< 4; i++)

                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                //vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                //vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                //vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blockTypes[blockID].GetTextureID(p));

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

    void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        // �������� ����
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }

    void AddTexture(int textureID)
    {

        float y = textureID / VoxelData.textureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);

        // �ؽ��� ��Ʋ�� ����ȭ
        x *= VoxelData.normalizedBlockTextureSize;
        y *= VoxelData.normalizedBlockTextureSize;

        y = 1f - y - VoxelData.normalizedBlockTextureSize;
        

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.normalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y + VoxelData.normalizedBlockTextureSize));


    }

}