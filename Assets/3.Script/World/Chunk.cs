using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;


    GameObject chunkObject;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    MeshCollider meshCollider;



    int vertexIndex = 0;




    // 정점 -> 여러 버텍스를 모아 폴리곤 -> 메쉬데이터
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();




    // 2d 텍스쳐를 3d 모델로 매핑되도록...
    List<Vector2> uvs = new List<Vector2>();




    public byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];



    World world;

    private bool _isActive;
    public bool isVoxelMapPopulated = false;
    
    public bool isActive
    {
        //get { return chunkObject.activeSelf; }
        get { return _isActive; }
        //set { chunkObject.SetActive(value); }
        set
        {

            _isActive = value;
            if (chunkObject != null)
                chunkObject.SetActive(value);
        }

    }
    public Chunk(ChunkCoord _coord, World _world, bool generateOnLoad)
    {

        coord = _coord;
        world = _world;
        isActive = true;


        if (generateOnLoad)
        {
            Init();
        }

        //chunkObject = new GameObject();
        //meshFilter = chunkObject.AddComponent<MeshFilter>();
        //meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        //meshRenderer.material = world.material;
        //meshCollider = chunkObject.AddComponent<MeshCollider>();
        //
        //
        //
        //chunkObject.transform.SetParent(world.transform);
        //chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        //chunkObject.name = coord.x + ", " + coord.z;
        //
        //PopulateVoxelMap();
        //CreateMeshData();
        //CreateMesh();
        //
        //
        //meshCollider.sharedMesh = meshFilter.mesh;

    }
    public void Init()
    {
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.material;
        meshCollider = chunkObject.AddComponent<MeshCollider>();



        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = coord.x + ", " + coord.z;

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();


        meshCollider.sharedMesh = meshFilter.mesh;
    }


    //public byte GetVoxelFromMap(Vector3 pos)
    //{
    //
    //    pos -= position;
    //
    //    return voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
    //
    //}

    // Chunk 내의 각 위치에 어떤 블록이 있을지...
    void PopulateVoxelMap()
    {

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
                    //Debug.Log($"Position: {voxelMap[x,y,z]}, Noise Value: {noiseValue}, Voxel Value: {voxelValue}");



                }
            }
        }

        isVoxelMapPopulated = true;

    }



    void CreateMeshData()
    {

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (world.blockTypes[voxelMap[x, y, z]].isSolid)
                    { 
                        AddVoxelDataToChunk(new Vector3(x, y, z)); 
                    }

                }
            }
        }

    }


    public Vector3 position
    {
        get { return chunkObject.transform.position; }
    }


    public bool IsVoxelInChunk(int x,int y, int z)
    {
        //if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
        //    return false;
        //else
        //    return true;

        if (x < 0 || x >= VoxelData.ChunkWidth || y < 0 || y >= VoxelData.ChunkHeight || z < 0 || z >= VoxelData.ChunkWidth)
            return false;
        else
            return true;
    

    }



    // 빈공간인지 아닌지 판단 -> 그래야 보이는 면만 그리니까...


    bool CheckVoxel(Vector3 pos)
    {
    
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.CheckForVoxel(pos + position);//world.blockTypes[world.GetVoxel(pos + position)].isSolid;
    
        return world.blockTypes[voxelMap[x, y, z]].isSolid;
    
    }

    public byte GetVoxelFromGlobalVector3(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);


        return voxelMap[xCheck, yCheck, zCheck];
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
    
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                }
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

    //void AddVoxelDataToChunk(Vector3 pos)
    //{
    //    for (int p = 0; p < 6; p++)
    //    {
    //        if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
    //        {
    //            byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
    //
    //            for (int i = 0; i < 4; i++)
    //            {
    //                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
    //            }
    //
    //            AddTexture(world.blockTypes[blockID].GetTextureID(p));
    //
    //            triangles.Add(vertexIndex);
    //            triangles.Add(vertexIndex + 1);
    //            triangles.Add(vertexIndex + 2);
    //            triangles.Add(vertexIndex + 2);
    //            triangles.Add(vertexIndex + 1);
    //            triangles.Add(vertexIndex + 3);
    //            vertexIndex += 4;
    //        }
    //    }
    //}


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


public class ChunkCoord
{
    public int x;
    public int z;




    public ChunkCoord()
    {
        x = 0;
        z = 0;

    }

    public ChunkCoord(int _x, int _z)
    {
        x = _x;
        z = _z;

    }

    public ChunkCoord(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int zCheck = Mathf.FloorToInt(pos.z);
    
        x = xCheck / VoxelData.ChunkWidth;
        z = zCheck / VoxelData.ChunkWidth;
    }


    public bool Equals(ChunkCoord other)
    {

        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;

    }
}