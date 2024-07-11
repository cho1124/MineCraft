using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
public class Chunk
{

    /// 여기는 VoxelData를 기반으로 Chunk를 생성하는 Script
    /// 
    /// 설명할게 너무 많아서 그냥 스크립트 전체에 주석 달아놓음
    /// 
    /// 
    /// 


    public ChunkCoord coord;
    public Vector3 position;

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




    // Voxel맵, Chunk 안에 어떤 블록을 넣을껀지 byte단위의 3차원 배열
    public byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];



    World world;

    // chunk의 활성화 여부 -> 최적화
    private bool _isActive;

    // voxel맵이 채워졌는지 여부
    private bool isVoxelMapPopulated = false;

    private bool threadLocked = false;

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

    //public Vector3 position
    //{
    //    get { return chunkObject.transform.position; }
    //}

    public bool isEditable
    {
        get
        {

            if (!isVoxelMapPopulated || threadLocked)
                return false;
            else
                return true;
        }
    }

    // 생성자
    public Chunk(ChunkCoord _coord, World _world, bool generateOnLoad)
    {

        coord = _coord;
        world = _world;
        isActive = true;


        if (generateOnLoad)
        {
            Init();
        }

        //chunkObject = new GameObject()
    }

    // Chunk 초기화
    public void Init()
    {
        chunkObject = new GameObject();
        //ItemBlock = Resources.Load<GameObject>("2.Model/Prefabs/ItemBlock");
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.material;
        meshCollider = chunkObject.AddComponent<MeshCollider>();



        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = coord.x + ", " + coord.z;
        position = chunkObject.transform.position;

        Thread myThread = new Thread(new ThreadStart(PopulateVoxelMap));
        myThread.Start();

        //PopulateVoxelMap();
        //UpdateChunk();


        ////CreateMeshData();
        //
        ////CreateMesh();


        // 시각적 메쉬 -> 충돌 메쉬 설정
        meshCollider.sharedMesh = meshFilter.mesh;
    }


    // Chunk 내의 각 위치에 어떤 블록이 있을지 채운다
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
        _updateChunk();
        isVoxelMapPopulated = true;

    }

    public void UpdateChunk()
    {
        //_updateChunk();
        Thread myThread = new Thread(new ThreadStart(_updateChunk));
        myThread.Start();
    }


    // Chunk 업데이트 (블럭부분만), 메쉬 생성 
    private void _updateChunk()
    {
        threadLocked = true;

        //while(modifications.count > 0)
        //{
        //    VoxelMod v = modifications.Dequeue();
        //    Vector3 pos = v.position -= position;
        //    voxelMap[(int)pos.x, (int)pos.y, (int)pos.z] = v.id;
        //}


        ClearMeshData();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (world.blockTypes[voxelMap[x, y, z]].isSolid)
                    {
                        UpdateMeshData(new Vector3(x, y, z));
                        //AddVoxelDataToChunk(new Vector3(x, y, z)); 
                    }

                }
            }
        }

        lock (world.ChunksToDraw)
        {
            //CreateMesh();
            world.ChunksToDraw.Enqueue(this);
        }

        threadLocked = false;
    }

    // 메쉬 데이터 초기화
    void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }




    // 복셀이 청크에 있는지 검사
    public bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= VoxelData.ChunkWidth || y < 0 || y >= VoxelData.ChunkHeight || z < 0 || z >= VoxelData.ChunkWidth)
            return false;
        else
            return true;

    }

    // Voxel 편집
    //public void EditVoxel(Vector3 pos, byte newID)
    //{
    //    int xCheck = Mathf.FloorToInt(pos.x);
    //    int yCheck = Mathf.FloorToInt(pos.y);
    //    int zCheck = Mathf.FloorToInt(pos.z);
    //
    //    // Chunk의 위치를 빼서 월드좌표 -> 청크내의 상대좌표로 변환
    //    xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
    //    zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);
    //
    //    byte currentBlockID = voxelMap[xCheck, yCheck, zCheck];
    //    //byte voxelBlock = voxelMap[xCheck, yCheck, zCheck];
    //    //GameObject tempBlock = GameObject.Instantiate(voxelBlock)
    //    if (newID == 0)
    //    {
    //        GameObject.Instantiate(world.ItemBlock, new Vector3(pos.x, pos.y, pos.z) , Quaternion.identity);
    //        for (int p = 0; p < 6; p++)
    //        {
    //            int textureID = world.blockTypes[currentBlockID].GetTextureID(p);
    //            //AddTextureForBlock(world.blockTypes[currentBlockID].GetTextureID(p));
    //            Debug.Log($"Block at ({pos.x}, {pos.y}, {pos.z}) replaced. TextureID of face {p}: {textureID}");
    //        }
    //        Debug.Log($"x : {pos.x}, y : {pos.y}, z : {pos.z}");
    //    }
    //
    //    voxelMap[xCheck, yCheck, zCheck] = newID;
    //
    //    // 변경된 주변 Voxel 업데이트
    //    UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
    //
    //    // Chunk 업데이트
    //    UpdateChunk();
    //}
    public byte EditVoxel(Vector3 pos, byte newID)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        // Chunk의 위치를 빼서 월드좌표 -> 청크내의 상대좌표로 변환
        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        // 현재 블록 ID 저장
        byte currentBlockID = voxelMap[xCheck, yCheck, zCheck];

        // 새로운 블록 ID로 설정
        voxelMap[xCheck, yCheck, zCheck] = newID;

        // 변경된 주변 Voxel 업데이트
        UpdateSurroundingVoxels(xCheck, yCheck, zCheck);

        // Chunk 업데이트
        UpdateChunk();

        // 현재 블록 ID 반환
        return currentBlockID;
    }

    public byte EditVoxelCave(Vector3 pos, byte newID)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);
        voxelMap[xCheck, yCheck, zCheck] = newID;

        // 변경된 주변 Voxel 업데이트
        UpdateSurroundingVoxels(xCheck, yCheck, zCheck);

        // Chunk 업데이트
        UpdateChunk();
        byte currentBlockID = voxelMap[xCheck, yCheck, zCheck];
        return currentBlockID;
    }



        // 변경된 주변 Voxel 업데이트
        void UpdateSurroundingVoxels(int x, int y, int z)
    {
        // 변경된 Voxel의 위치 저장
        Vector3 thisVoxel = new Vector3(x, y, z);



        // Voxel의 6면을 검사해서 해당 Voxel이 현재 Chunk 내에 없다면 그 Voxel이 속한 Chunk를 찾아서 업데이트
        for (int p = 0; p < 6; p++)
        {
            Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {
                world.GetChunkFromVector3(currentVoxel + position).UpdateChunk();
            }
        }


    }


    // 빈공간인지 아닌지 판단 -> 그래야 보이는 면만 그리니까...
    // 이건 Chunk 내부에서만 검사
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
    
        xCheck -= Mathf.FloorToInt(position.x);
        zCheck -= Mathf.FloorToInt(position.z);
    
    
        return voxelMap[xCheck, yCheck, zCheck];
    }



    // 아마 이 프로젝트에서 가장 중요한 메서드
    // 복셀의 각 면을 검사해서 보이는 면만 메쉬 데이터에 추가
    // 이 메소드를 UpdateChunk에서 for문을 3번으로 돌려서 한 Chunk에 보이는 면만 렌더링하니 게임성능이 최적화
    public void UpdateMeshData(Vector3 pos)
    {

        // 블록은 6면체

        for (int p = 0; p < 6; p++)
        {

            // face check(면이 바라보는 방향으로 +1 이동하여 확인) 했을때
            // soild(빈공간이 아닌)가 아닌경우에만 큐브의 면이 그려지도록....
            // -> 청크의 외곽 부분만 면이 그려지고, 내부에는 면이 그려지지 않도록 최적화 


            // 현재 면이 외부와 접해있는지 확인
            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                // 현재 Voxel의 ID를 가져옴
                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];


                // 각 면의 정점을 추가
                // 한 면은 사각형이니까 4번...
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                }

                // 텍스쳐 좌표 추가
                AddTexture(world.blockTypes[blockID].GetTextureID(p));


                // 삼각형 인덱스 추가
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);

                // 다음면의 첫번째 정점 인덱스 설정하기 위해 4만큼 증가
                vertexIndex += 4;

            }
        }

    }


    /// 새로운 mesh 객체 생성 -> 이전에 수집된 정점, 삼각형 인덱스, UV 좌표를 mesh에 할당
    /// -> 법선벡터 재계산(조명계산) -> 다 생성된 mesh를 meshFileter에 할당해서 렌더링

    public void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        // 법선벡터 재계산
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }

    private void AddTexture(int textureID)
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

    private void AddTextureForBlock(int textureID)
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


    // 두 Chunk 가 서로 같은지 판단
    // 어따씀??? -> 플레이어 위치 업데이트, 활성 Chunk 관리
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


public struct ChunkData
{
    public ChunkCoord coord;
    public NativeArray<byte> voxelMap; // Voxel 데이터
}