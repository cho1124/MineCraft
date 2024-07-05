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
        // 아틀라스 내의 텍스쳐 가로, 세로 개수
        (int w, int h) = (VoxelData.textureAtlasWidth, VoxelData.textureAtlasHeight);

        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        AddTextureUV(x, y);
    }

    // 텍스쳐 아틀라스 내에서(x, y) 위치의 텍스쳐 UV를 uvs 리스트에 추가
    private void AddTextureUV(int x, int y)
    {
        //if (x < 0 || y < 0 || x >= VoxelData.TextureAtlasWidth || y >= VoxelData.TextureAtlasHeight)
        //    throw new IndexOutOfRangeException($"텍스쳐 아틀라스의 범위를 벗어났습니다 : [x = {x}, y = {y}]");

        float nw = VoxelData.NormalizedTextureAtlasWidth;
        float nh = VoxelData.NormalizedTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        // 해당 텍스쳐의 uv를 LB-LT-RB-RT 순서로 추가
        uvs.Add(new Vector2(uvX, uvY));
        uvs.Add(new Vector2(uvX, uvY + nh));
        uvs.Add(new Vector2(uvX + nw, uvY));
        uvs.Add(new Vector2(uvX + nw, uvY + nh));
    }



    void CreatMesh()
    {
        // 6방향의 면 그리기


        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        // 블록은 정육면체
        for (int p = 0; p < 6; p++)
        {
            // face check(면이 바라보는 방향으로 +1 이동하여 확인) 했을때
            // soild(빈공간이 아닌)가 아닌경우에만 큐브의 면이 그려지도록....
            // -> 청크의 외곽 부분만 면이 그려지고, 내부에는 면이 그려지지 않도록 최적화 

            if (CheckVoxel(pos) && !CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                byte blockID = GetBlockID(pos);

                // 각 면의 삼각형 2개 그리기
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
