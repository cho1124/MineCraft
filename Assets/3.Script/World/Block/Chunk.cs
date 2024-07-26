using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
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
    List<int> waterTriangles = new List<int>();

    List<Color> colors = new List<Color>();

    Material[] materials = new Material[3];



    // 2d 텍스쳐를 3d 모델로 매핑되도록...
    List<Vector2> uvs = new List<Vector2>();




    // chunk의 활성화 여부 -> 최적화
    private bool _isActive;



    ChunkData chunkData;


    List<Vector3> normals = new List<Vector3>();


    public bool isActive
    {

        get { return _isActive; }

        set
        {

            _isActive = value;
            if (chunkObject != null)
                chunkObject.SetActive(value);
        }

    }

    public Chunk(ChunkCoord _coord)
    {

        coord = _coord;
    }

    public void Init()
    {


        chunkObject = new GameObject();

        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        materials[0] = World.Instance.material;
        materials[1] = World.Instance.transparentMaterial;
        materials[2] = World.Instance.waterMaterial;

        meshRenderer.materials = materials;
        meshCollider = chunkObject.AddComponent<MeshCollider>();



        chunkObject.transform.SetParent(World.Instance.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = coord.x + ", " + coord.z;
        position = chunkObject.transform.position;

        chunkData = World.Instance.worldData.RequestChunk(new Vector2Int((int)position.x, (int)position.z), true);



        //// 시각적 메쉬 -> 충돌 메쉬 설정
        meshCollider.sharedMesh = meshFilter.mesh;

        lock (World.Instance.ChunkUpdateThreadLock)
            World.Instance.chunksToUpdate.Add(this);


    }



    // 여기에 populatechunkData.map이 있었다 20240717
    // Chunk 내의 각 위치에 어떤 블록이 있을지 채운다



    // Chunk 업데이트 (블럭부분만), 메쉬 생성 
    public void UpdateChunk()
    {



        ClearMeshData();
        //CalculateLight();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (World.Instance.blockTypes[chunkData.map[x, y, z].id].isSolid)
                    {
                        UpdateMeshData(new Vector3(x, y, z));

                    }

                }
            }
        }

        World.Instance.ChunksToDraw.Enqueue(this);

    }

    // 메쉬 데이터 초기화
    void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        transparentTriangles.Clear();
        waterTriangles.Clear();
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

        chunkData.ModifyVoxel(new Vector3Int(xCheck, yCheck, zCheck), newID, World.Instance._player.orientation);
        // 새로운 블록 ID로 설정



        lock (World.Instance.ChunkUpdateThreadLock)
        {
            World.Instance.chunksToUpdate.Insert(0, this);
            // 변경된 주변 Voxel 업데이트
            UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
        }
        //// Chunk 업데이트


        // 현재 블록 ID 반환
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
                World.Instance.chunksToUpdate.Insert(0, World.Instance.GetChunkFromVector3(currentVoxel + position)); //.UpdateChunk();
            }
        }


    }


    // 빈공간인지 아닌지 판단 -> 그래야 보이는 면만 그리니까...
    // 이건 Chunk 내부에서만 검사
    VoxelState CheckVoxel(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return World.Instance.GetVoxelState(pos + position);//World.Instance.blockTypes[World.Instance.GetVoxel(pos + position)].isSolid;

        return chunkData.map[x, y, z];

    }


    //void CalculateLight()
    //{
    //    Queue<Vector3Int> litVoxels = new Queue<Vector3Int>();
    //
    //
    //    for (int x = 0; x < VoxelData.ChunkWidth; x++)
    //    {
    //        for (int z = 0; z < VoxelData.ChunkWidth; z++)
    //        {
    //
    //            float lightRay = 1f;
    //
    //            for (int y = VoxelData.ChunkHeight - 1; y >= 0; y--)
    //            {
    //
    //                VoxelState thisVoxel = chunkData.map[x, y, z];
    //
    //                if (thisVoxel.id > 0 && World.Instance.blockTypes[thisVoxel.id].opacity < lightRay)
    //                    if (World.Instance.blockTypes[thisVoxel.id].opacity == 2)
    //                        lightRay = World.Instance.blockTypes[thisVoxel.id].opacity * 0.25f;
    //                    else
    //                        lightRay = World.Instance.blockTypes[thisVoxel.id].opacity;
    //
    //                thisVoxel.globalLightPercent = lightRay;
    //
    //                chunkData.map[x, y, z] = thisVoxel;
    //
    //                if (lightRay > VoxelData.lightFalloff)
    //                {
    //                    litVoxels.Enqueue(new Vector3Int(x, y, z));
    //                }
    //
    //
    //            }
    //
    //        }
    //    }
    //
    //    while (litVoxels.Count > 0)
    //    {
    //        Vector3Int v = litVoxels.Dequeue();
    //
    //        for (int p = 0; p < 6; p++)
    //        {
    //            Vector3 currentVoxel = v + VoxelData.faceChecks[p];
    //            Vector3Int neighbor = new Vector3Int((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z);
    //
    //            if (IsVoxelInChunk(neighbor.x, neighbor.y, neighbor.y))
    //            {
    //                if (chunkData.map[neighbor.x, neighbor.y, neighbor.y].globalLightPercent < chunkData.map[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFalloff)
    //                {
    //                    chunkData.map[neighbor.x, neighbor.y, neighbor.y].globalLightPercent = chunkData.map[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFalloff;
    //
    //                    if (chunkData.map[neighbor.x, neighbor.y, neighbor.y].globalLightPercent > VoxelData.lightFalloff)
    //                    {
    //                        litVoxels.Enqueue(neighbor);
    //                    }
    //                }
    //            }
    //
    //
    //        }
    //    }
    //
    //}


    // 복셀의 각 면을 검사해서 보이는 면만 메쉬 데이터에 추가
    // UpdateChunk에서 for문을 3번으로 돌려서 한 Chunk에 보이는 면만 렌더링해서 게임성능을 최적화

    public void UpdateMeshData(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        byte blockID = chunkData.map[x, y, z].id;
        VoxelState voxel = chunkData.map[x, y, z];


        float rot = 0f;
        switch (voxel.orientation)
        {
            case 0:
                rot = 180f;
                break;
            case 5:
                rot = 270f;
                break;
            case 1:
                rot = 0f;
                break;
            default:
                rot = 90f;
                break;
        }




        // 블록은 6면체
        for (int p = 0; p < 6; p++)
        {


            int translatedP = p;

            if (voxel.orientation != 1)
            {
                if (voxel.orientation == 0)
                {
                    if (p == 0) translatedP = 1;
                    else if (p == 1) translatedP = 0;
                    else if (p == 4) translatedP = 5;
                    else if (p == 5) translatedP = 4;
                }
                else if (voxel.orientation == 5)
                {
                    if (p == 0) translatedP = 5;
                    else if (p == 1) translatedP = 4;
                    else if (p == 4) translatedP = 0;
                    else if (p == 5) translatedP = 1;
                }
                else if (voxel.orientation == 4)
                {
                    if (p == 0) translatedP = 4;
                    else if (p == 1) translatedP = 5;
                    else if (p == 4) translatedP = 1;
                    else if (p == 5) translatedP = 0;
                }
            }


            // face check(면이 바라보는 방향으로 +1 이동하여 확인) 했을때
            // soild(빈공간이 아닌)가 아닌경우에만 큐브의 면이 그려지도록....
            // -> 청크의 외곽 부분만 면이 그려지고, 내부에는 면이 그려지지 않도록 최적화 
            VoxelState neighbour = CheckVoxel(pos + VoxelData.faceChecks[translatedP]);


            // 현재 면이 외부와 접해있는지 확인
            if (
                neighbour != null && 
                World.Instance.blockTypes[neighbour.id].renderNeighborFaces && 
                !(voxel.properties.isWater && chunkData.map[x,y+1,z].properties.isWater)
                )
            {
                float lightLevel = neighbour.globalLightPercent;
                int faceVertCount = 0;


                for (int i = 0; i < voxel.properties.meshData.faces[p].vertData.Length; i++)
                {
                    VertData vertData = voxel.properties.meshData.faces[p].GetVertData(i);
                    vertices.Add(pos + vertData.GetRotatedPosition(new Vector3(0, rot, 0)));
                    normals.Add(VoxelData.faceChecks[p]);
                    colors.Add(new Color(0, 0, 0, lightLevel));
                    if (voxel.properties.isWater)
                    {
                        uvs.Add(voxel.properties.meshData.faces[p].vertData[i].uv);
                    }
                    else
                    {
                        AddTexture(voxel.properties.GetTextureID(p), vertData.uv);
                    }

                    faceVertCount++;



                }

                if (!voxel.properties.renderNeighborFaces)
                {

                    for (int i = 0; i < voxel.properties.meshData.faces[p].triangles.Length; i++)
                        triangles.Add(vertexIndex + voxel.properties.meshData.faces[p].triangles[i]);

                }
                else
                {
                    if (voxel.properties.isWater)
                    {

                        for (int i = 0; i < voxel.properties.meshData.faces[p].triangles.Length; i++)
                            waterTriangles.Add(vertexIndex + voxel.properties.meshData.faces[p].triangles[i]);

                    }
                    else
                    {

                        for (int i = 0; i < voxel.properties.meshData.faces[p].triangles.Length; i++)
                            transparentTriangles.Add(vertexIndex + voxel.properties.meshData.faces[p].triangles[i]);
                    }

                }

                vertexIndex += faceVertCount;



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

        mesh.subMeshCount = 3;

        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(transparentTriangles.ToArray(), 1);
        mesh.SetTriangles(waterTriangles.ToArray(), 2);
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.normals = normals.ToArray();

        // 법선벡터 재계산
        //mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }



    private void AddTexture(int textureID, Vector2 uv)
    {

        float y = textureID / VoxelData.textureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);

        // 텍스쳐 아틀라스 정규화
        x *= VoxelData.normalizedBlockTextureSize;
        y *= VoxelData.normalizedBlockTextureSize;

        y = 1f - y - VoxelData.normalizedBlockTextureSize;


        x += VoxelData.normalizedBlockTextureSize * uv.x;
        y += VoxelData.normalizedBlockTextureSize * uv.y;

        uvs.Add(new Vector2(x, y));
        //uvs.Add(new Vector2(x, y));
        //uvs.Add(new Vector2(x, y + VoxelData.normalizedBlockTextureSize));
        //uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y));
        //uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y + VoxelData.normalizedBlockTextureSize));


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
