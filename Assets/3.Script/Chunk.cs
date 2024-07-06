using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    int vertexIndex = 0;

    // 정점 -> 여러 버텍스를 모아 폴리곤 -> 메쉬데이터
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    // 2d 텍스쳐를 3d 모델로 매핑되도록...
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

    // Chunk 내의 각 위치에 어떤 블록이 있을지...
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


    // 빈공간인지 아닌지 판단 -> 그래야 보이는 면만 그리니까...
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
        // 블록은 6면체

        for (int p = 0; p < 6; p++)
        {

            // face check(면이 바라보는 방향으로 +1 이동하여 확인) 했을때
            // soild(빈공간이 아닌)가 아닌경우에만 큐브의 면이 그려지도록....
            // -> 청크의 외곽 부분만 면이 그려지고, 내부에는 면이 그려지지 않도록 최적화 

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

        // 법선벡터 재계산
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }

    void AddTexture(int textureID)
    {

        float y = textureID / VoxelData.textureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);

        // 텍스쳐 아틀라스 정규화
        x *= VoxelData.normalizedBlockTextureSize;
        y *= VoxelData.normalizedBlockTextureSize;

        y = 1f - y - VoxelData.normalizedBlockTextureSize;
        

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.normalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y + VoxelData.normalizedBlockTextureSize));


    }

}