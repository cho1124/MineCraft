using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    // Chunk.cs

    private bool[,,] voxelMap = new bool[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    private void Start()
    {


        //for (int i = 0; i <= 3; i++)
        //{
        //    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, i]] + pos);
        //    uvs.Add(VoxelData.voxelUvs[i]);
        //}
        //
        //// 2. Triangle의 버텍스 인덱스 6개 추가
        //triangles.Add(vertexIndex);
        //triangles.Add(vertexIndex + 1);
        //triangles.Add(vertexIndex + 2);
        //
        //triangles.Add(vertexIndex + 2);
        //triangles.Add(vertexIndex + 1);
        //triangles.Add(vertexIndex + 3);
        //
        //vertexIndex += 4;
        for(int y =0; y<VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {

                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    AddVoxelDataToChunk(new Vector3(x,y,z));

                }
            }

        }

        CreatMesh();



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
        for (int p = 0; p < 6; p++)
        {

            for (int i = 0; i < 6; i++)
            {
                // 각 면의 삼각형 2개 그리기
                int triangleIndex = VoxelData.voxelTris[p, i];
                vertices.Add(VoxelData.voxelVerts[triangleIndex] + pos);
                triangles.Add(vertexIndex);

                uvs.Add(VoxelData.voxelUvs[i]);

                vertexIndex++;
            }
        }

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
