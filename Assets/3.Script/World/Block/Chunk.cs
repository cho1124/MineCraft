using System.Collections;
using System.Collections.Generic;
//using System.Threading;
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
    List<int> transparentTriangles = new List<int>();

    List<Color> colors = new List<Color>();

    Material[] materials = new Material[2];



    // 2d 텍스쳐를 3d 모델로 매핑되도록...
    List<Vector2> uvs = new List<Vector2>();




    // Voxel맵, Chunk 안에 어떤 블록을 넣을껀지 byte단위의 3차원 배열
    //public byte[,,] chunkData.map = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    //public VoxelState[,,] chunkData.map = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    //public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    //World.Instance World.Instance;

    // chunk의 활성화 여부 -> 최적화
    private bool _isActive;

    //// voxel맵이 채워졌는지 여부
    //public bool ischunkData.mapPopulated = false;

    ChunkData chunkData;



    List<Vector3> normals = new List<Vector3>();
    //private bool threadLocked = false;

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

    //public bool isEditable
    //{
    //    get
    //    {
    //
    //        if (!ischunkData.mapPopulated)
    //            return false;
    //        else
    //            return true;
    //    }
    //}

    // 생성자
    //public Chunk(ChunkCoord _coord, World.Instance _World.Instance, bool generateOnLoad)
    //{
    //
    //    coord = _coord;
    //    World.Instance = _World.Instance;
    //    isActive = true;
    //
    //
    //    if (generateOnLoad)
    //    {
    //        Init();
    //    }
    //
    //    //chunkObject = new GameObject()
    //}


    public Chunk(ChunkCoord _coord)
    {

        coord = _coord;

        //isActive = true;

        //chunkObject = new GameObject()
    }



    // Chunk 초기화
    public void Init()
    {
        chunkObject = new GameObject();
        //ItemBlock = Resources.Load<GameObject>("2.Model/Prefabs/ItemBlock");
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        //meshRenderer.material = World.Instance.material;
        materials[0] = World.Instance.material;
        materials[1] = World.Instance.transparentMaterial;

        meshRenderer.materials = materials;
        meshCollider = chunkObject.AddComponent<MeshCollider>();



        chunkObject.transform.SetParent(World.Instance.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = coord.x + ", " + coord.z;
        position = chunkObject.transform.position;

        chunkData = World.Instance.worldData.RequestChunk(new Vector2Int((int)position.x, (int)position.z), true);

        //PopulatechunkData.map();

        //PopulatechunkData.map();
        //UpdateChunk();


        ////CreateMeshData();
        //
        ////CreateMesh();


        // 시각적 메쉬 -> 충돌 메쉬 설정
        meshCollider.sharedMesh = meshFilter.mesh;

        lock (World.Instance.ChunkUpdateThreadLock)
            World.Instance.chunksToUpdate.Add(this);

        
    }



    // 여기에 populatechunkData.map이 있었다 20240717
    // Chunk 내의 각 위치에 어떤 블록이 있을지 채운다
    //void PopulatechunkData.map()
    //{
    //
    //    for (int y = 0; y < VoxelData.ChunkHeight; y++)
    //    {
    //        for (int x = 0; x < VoxelData.ChunkWidth; x++)
    //        {
    //            for (int z = 0; z < VoxelData.ChunkWidth; z++)
    //            {
    //                chunkData.map[x, y, z] = new VoxelState(World.Instance.GetVoxel(new Vector3(x, y, z) + position));
    //                //Debug.Log($"Position: {chunkData.map[x,y,z]}, Noise Value: {noiseValue}, Voxel Value: {voxelValue}");
    //
    //
    //
    //            }
    //        }
    //    }
    //    //UpdateChunk();
    //    ischunkData.mapPopulated = true;
    //
    //
    //    lock (World.Instance.ChunkUpdateThreadLock)
    //    {
    //
    //        World.Instance.chunksToUpdate.Add(this);
    //    }
    //
    //}

    //public void UpdateChunk()
    //{
    //    //_updateChunk();
    //    if (World.Instance.enableThreading)
    //    {
    //        Thread myThread = new Thread(new ThreadStart(_updateChunk));
    //        myThread.Start();
    //    }
    //    else
    //        _updateChunk();
    //}


    // Chunk 업데이트 (블럭부분만), 메쉬 생성 
    public void UpdateChunk()
    {
        //threadLocked = true;

        //while (modifications.Count > 0)
        //{
        //    VoxelMod v = modifications.Dequeue();
        //    Vector3 pos = v.position -= position;
        //    chunkData.map[(int)pos.x, (int)pos.y, (int)pos.z].id = v.id;
        //}


        ClearMeshData();
        CalculateLight();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (World.Instance.blockTypes[chunkData.map[x, y, z].id].isSolid)
                    {
                        UpdateMeshData(new Vector3(x, y, z));
                        //AddVoxelDataToChunk(new Vector3(x, y, z)); 
                    }

                }
            }
        }

        //lock (World.Instance.ChunksToDraw)
        //{
        //CreateMesh();
        World.Instance.ChunksToDraw.Enqueue(this);
        //}
        //
        //threadLocked = false;
    }

    // 메쉬 데이터 초기화
    void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        transparentTriangles.Clear();
        uvs.Clear();
        colors.Clear();
        normals.Clear();
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
    //    byte currentBlockID = chunkData.map[xCheck, yCheck, zCheck];
    //    //byte voxelBlock = chunkData.map[xCheck, yCheck, zCheck];
    //    //GameObject tempBlock = GameObject.Instantiate(voxelBlock)
    //    if (newID == 0)
    //    {
    //        GameObject.Instantiate(World.Instance.ItemBlock, new Vector3(pos.x, pos.y, pos.z) , Quaternion.identity);
    //        for (int p = 0; p < 6; p++)
    //        {
    //            int textureID = World.Instance.blockTypes[currentBlockID].GetTextureID(p);
    //            //AddTextureForBlock(World.Instance.blockTypes[currentBlockID].GetTextureID(p));
    //            Debug.Log($"Block at ({pos.x}, {pos.y}, {pos.z}) replaced. TextureID of face {p}: {textureID}");
    //        }
    //        Debug.Log($"x : {pos.x}, y : {pos.y}, z : {pos.z}");
    //    }
    //
    //    chunkData.map[xCheck, yCheck, zCheck] = newID;
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
        byte currentBlockID = chunkData.map[xCheck, yCheck, zCheck].id;

        // 새로운 블록 ID로 설정
        chunkData.map[xCheck, yCheck, zCheck].id = newID;
        World.Instance.worldData.AddToModifiedChunkList(chunkData);


        lock (World.Instance.ChunkUpdateThreadLock)
        {
            World.Instance.chunksToUpdate.Insert(0, this);
            // 변경된 주변 Voxel 업데이트
            UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
        }
        //// Chunk 업데이트
        //UpdateChunk();

        // 현재 블록 ID 반환
        return currentBlockID;
    }

    //public byte EditVoxelCave(Vector3 pos, byte newID)
    //{
    //    int xCheck = Mathf.FloorToInt(pos.x);
    //    int yCheck = Mathf.FloorToInt(pos.y);
    //    int zCheck = Mathf.FloorToInt(pos.z);
    //    chunkData.map[xCheck, yCheck, zCheck] = newID;
    //
    //    // 변경된 주변 Voxel 업데이트
    //    UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
    //
    //    // Chunk 업데이트
    //    UpdateChunk();
    //    byte currentBlockID = chunkData.map[xCheck, yCheck, zCheck];
    //    return currentBlockID;
    //}



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
                World.Instance.chunksToUpdate.Insert(0, World.Instance.GetChunkFromVector3(currentVoxel + position)); //.UpdateChunk();
            }
        }


    }


    // 빈공간인지 아닌지 판단 -> 그래야 보이는 면만 그리니까...
    // 이건 Chunk 내부에서만 검사
    //bool CheckVoxel(Vector3 pos)
    //{
    //
    //    int x = Mathf.FloorToInt(pos.x);
    //    int y = Mathf.FloorToInt(pos.y);
    //    int z = Mathf.FloorToInt(pos.z);
    //
    //    if (!IsVoxelInChunk(x, y, z))
    //        return World.Instance.CheckIfVoxelTransparent(pos + position);//World.Instance.blockTypes[World.Instance.GetVoxel(pos + position)].isSolid;
    //
    //    return World.Instance.blockTypes[chunkData.map[x, y, z].id].renderNeighborFaces;
    //
    //}
    VoxelState CheckVoxel(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return World.Instance.GetVoxelState(pos + position);//World.Instance.blockTypes[World.Instance.GetVoxel(pos + position)].isSolid;

        return chunkData.map[x, y, z];

    }



    public VoxelState GetVoxelFromGlobalVector3(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(position.x);
        zCheck -= Mathf.FloorToInt(position.z);


        return chunkData.map[xCheck, yCheck, zCheck];
    }

    void CalculateLight()
    {
        Queue<Vector3Int> litVoxels = new Queue<Vector3Int>();


        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int z = 0; z < VoxelData.ChunkWidth; z++)
            {

                float lightRay = 1f;

                for (int y = VoxelData.ChunkHeight - 1; y >= 0; y--)
                {

                    VoxelState thisVoxel = chunkData.map[x, y, z];

                    if (thisVoxel.id > 0 && World.Instance.blockTypes[thisVoxel.id].transparency < lightRay)
                        lightRay = World.Instance.blockTypes[thisVoxel.id].transparency;

                    thisVoxel.globalLightPercent = lightRay;

                    chunkData.map[x, y, z] = thisVoxel;

                    if (lightRay > VoxelData.lightFalloff)
                    {
                        litVoxels.Enqueue(new Vector3Int(x, y, z));
                    }


                }

            }
        }

        while (litVoxels.Count > 0)
        {
            Vector3Int v = litVoxels.Dequeue();

            for (int p = 0; p < 6; p++)
            {
                Vector3 currentVoxel = v + VoxelData.faceChecks[p];
                Vector3Int neighbor = new Vector3Int((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z);

                if (IsVoxelInChunk(neighbor.x, neighbor.y, neighbor.y))
                {
                    if (chunkData.map[neighbor.x, neighbor.y, neighbor.y].globalLightPercent < chunkData.map[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFalloff)
                    {
                        chunkData.map[neighbor.x, neighbor.y, neighbor.y].globalLightPercent = chunkData.map[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFalloff;

                        if (chunkData.map[neighbor.x, neighbor.y, neighbor.y].globalLightPercent > VoxelData.lightFalloff)
                        {
                            litVoxels.Enqueue(neighbor);
                        }
                    }
                }


            }
        }

    }


    // 아마 이 프로젝트에서 가장 중요한 메서드
    // 복셀의 각 면을 검사해서 보이는 면만 메쉬 데이터에 추가
    // 이 메소드를 UpdateChunk에서 for문을 3번으로 돌려서 한 Chunk에 보이는 면만 렌더링하니 게임성능이 최적화
    public void UpdateMeshData(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        //byte blockID = chunkData.map[(int)pos.x, (int)pos.y, (int)pos.z].id;
        byte blockID = chunkData.map[x, y, z].id;
        //bool renderNeighborFaces = World.Instance.blockTypes[blockID].renderNeighborFaces;
        VoxelState voxel = chunkData.map[x, y, z];

        // 블록은 6면체
        for (int p = 0; p < 6; p++)
        {

            // face check(면이 바라보는 방향으로 +1 이동하여 확인) 했을때
            // soild(빈공간이 아닌)가 아닌경우에만 큐브의 면이 그려지도록....
            // -> 청크의 외곽 부분만 면이 그려지고, 내부에는 면이 그려지지 않도록 최적화 
            VoxelState neighbor = CheckVoxel(pos + VoxelData.faceChecks[p]);

            // 현재 면이 외부와 접해있는지 확인
            if (neighbor != null && World.Instance.blockTypes[neighbor.id].renderNeighborFaces)
            {
                // 현재 Voxel의 ID를 가져옴
                //byte blockID = chunkData.map[(int)pos.x, (int)pos.y, (int)pos.z];





                // 각 면의 정점을 추가
                // 한 면은 사각형이니까 4번...
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                }

                for (int i = 0; i < 4; i++)
                {
                    normals.Add(VoxelData.faceChecks[p]);
                }
                // 텍스쳐 좌표 추가
                AddTexture(World.Instance.blockTypes[blockID].GetTextureID(p));

                //float lightLevel;
                //int yPos = (int)pos.y + 1;
                //bool inShade = false;
                //while (yPos < VoxelData.ChunkHeight)
                //{
                //    if (chunkData.map[(int)pos.x, yPos, (int)pos.z].id != 0)
                //    {
                //        inShade = true;
                //        break;
                //    }
                //
                //
                //
                //    yPos++;
                //}
                //
                //if (inShade)
                //{
                //
                //
                //    lightLevel = 0.4f;
                //}
                //else
                //{ lightLevel = 0f; }
                float lightLevel = neighbor.globalLightPercent;

                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));

                if (!World.Instance.blockTypes[neighbor.id].renderNeighborFaces)
                {
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 3);
                }
                else
                {
                    transparentTriangles.Add(vertexIndex);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 3);

                    // 삼각형 인덱스 추가
                }

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
        // mesh.triangles = triangles.ToArray();

        mesh.subMeshCount = 2;

        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(transparentTriangles.ToArray(), 1);
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.normals = normals.ToArray();

        // 법선벡터 재계산
        //mesh.RecalculateNormals();

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


[System.Serializable]
public class VoxelState
{
    public byte id;
    public float globalLightPercent;

    public VoxelState()
    {
        id = 0;
        globalLightPercent = 0f;
    }


    public VoxelState(byte _id)
    {
        id = _id;
        globalLightPercent = 0f;

    }
}

//public struct ChunkData
//{
//    public ChunkCoord coord;
//    public NativeArray<byte> chunkData.map; // Voxel 데이터
//}