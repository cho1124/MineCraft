using System.Collections;
using System.Collections.Generic;
//using System.Threading;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
public class Chunk
{

    /// ����� VoxelData�� ������� Chunk�� �����ϴ� Script
    /// 
    /// �����Ұ� �ʹ� ���Ƽ� �׳� ��ũ��Ʈ ��ü�� �ּ� �޾Ƴ���
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




    // ���� -> ���� ���ؽ��� ��� ������ -> �޽�������
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<int> transparentTriangles = new List<int>();

    List<Color> colors = new List<Color>();

    Material[] materials = new Material[2];



    // 2d �ؽ��ĸ� 3d �𵨷� ���εǵ���...
    List<Vector2> uvs = new List<Vector2>();




    // Voxel��, Chunk �ȿ� � ����� �������� byte������ 3���� �迭
    //public byte[,,] chunkData.map = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    //public VoxelState[,,] chunkData.map = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    //public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    //World.Instance World.Instance;

    // chunk�� Ȱ��ȭ ���� -> ����ȭ
    private bool _isActive;

    //// voxel���� ä�������� ����
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

    // ������
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



    // Chunk �ʱ�ȭ
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


        // �ð��� �޽� -> �浹 �޽� ����
        meshCollider.sharedMesh = meshFilter.mesh;

        lock (World.Instance.ChunkUpdateThreadLock)
            World.Instance.chunksToUpdate.Add(this);

        
    }



    // ���⿡ populatechunkData.map�� �־��� 20240717
    // Chunk ���� �� ��ġ�� � ����� ������ ä���
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


    // Chunk ������Ʈ (���κи�), �޽� ���� 
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

    // �޽� ������ �ʱ�ȭ
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




    // ������ ûũ�� �ִ��� �˻�
    public bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= VoxelData.ChunkWidth || y < 0 || y >= VoxelData.ChunkHeight || z < 0 || z >= VoxelData.ChunkWidth)
            return false;
        else
            return true;

    }

    // Voxel ����
    //public void EditVoxel(Vector3 pos, byte newID)
    //{
    //    int xCheck = Mathf.FloorToInt(pos.x);
    //    int yCheck = Mathf.FloorToInt(pos.y);
    //    int zCheck = Mathf.FloorToInt(pos.z);
    //
    //    // Chunk�� ��ġ�� ���� ������ǥ -> ûũ���� �����ǥ�� ��ȯ
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
    //    // ����� �ֺ� Voxel ������Ʈ
    //    UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
    //
    //    // Chunk ������Ʈ
    //    UpdateChunk();
    //}
    public byte EditVoxel(Vector3 pos, byte newID)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        // Chunk�� ��ġ�� ���� ������ǥ -> ûũ���� �����ǥ�� ��ȯ
        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        // ���� ��� ID ����
        byte currentBlockID = chunkData.map[xCheck, yCheck, zCheck].id;

        // ���ο� ��� ID�� ����
        chunkData.map[xCheck, yCheck, zCheck].id = newID;
        World.Instance.worldData.AddToModifiedChunkList(chunkData);


        lock (World.Instance.ChunkUpdateThreadLock)
        {
            World.Instance.chunksToUpdate.Insert(0, this);
            // ����� �ֺ� Voxel ������Ʈ
            UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
        }
        //// Chunk ������Ʈ
        //UpdateChunk();

        // ���� ��� ID ��ȯ
        return currentBlockID;
    }

    //public byte EditVoxelCave(Vector3 pos, byte newID)
    //{
    //    int xCheck = Mathf.FloorToInt(pos.x);
    //    int yCheck = Mathf.FloorToInt(pos.y);
    //    int zCheck = Mathf.FloorToInt(pos.z);
    //    chunkData.map[xCheck, yCheck, zCheck] = newID;
    //
    //    // ����� �ֺ� Voxel ������Ʈ
    //    UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
    //
    //    // Chunk ������Ʈ
    //    UpdateChunk();
    //    byte currentBlockID = chunkData.map[xCheck, yCheck, zCheck];
    //    return currentBlockID;
    //}



    // ����� �ֺ� Voxel ������Ʈ
    void UpdateSurroundingVoxels(int x, int y, int z)
    {
        // ����� Voxel�� ��ġ ����
        Vector3 thisVoxel = new Vector3(x, y, z);



        // Voxel�� 6���� �˻��ؼ� �ش� Voxel�� ���� Chunk ���� ���ٸ� �� Voxel�� ���� Chunk�� ã�Ƽ� ������Ʈ
        for (int p = 0; p < 6; p++)
        {
            Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {
                World.Instance.chunksToUpdate.Insert(0, World.Instance.GetChunkFromVector3(currentVoxel + position)); //.UpdateChunk();
            }
        }


    }


    // ��������� �ƴ��� �Ǵ� -> �׷��� ���̴� �鸸 �׸��ϱ�...
    // �̰� Chunk ���ο����� �˻�
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


    // �Ƹ� �� ������Ʈ���� ���� �߿��� �޼���
    // ������ �� ���� �˻��ؼ� ���̴� �鸸 �޽� �����Ϳ� �߰�
    // �� �޼ҵ带 UpdateChunk���� for���� 3������ ������ �� Chunk�� ���̴� �鸸 �������ϴ� ���Ӽ����� ����ȭ
    public void UpdateMeshData(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        //byte blockID = chunkData.map[(int)pos.x, (int)pos.y, (int)pos.z].id;
        byte blockID = chunkData.map[x, y, z].id;
        //bool renderNeighborFaces = World.Instance.blockTypes[blockID].renderNeighborFaces;
        VoxelState voxel = chunkData.map[x, y, z];

        // ����� 6��ü
        for (int p = 0; p < 6; p++)
        {

            // face check(���� �ٶ󺸴� �������� +1 �̵��Ͽ� Ȯ��) ������
            // soild(������� �ƴ�)�� �ƴѰ�쿡�� ť���� ���� �׷�������....
            // -> ûũ�� �ܰ� �κи� ���� �׷�����, ���ο��� ���� �׷����� �ʵ��� ����ȭ 
            VoxelState neighbor = CheckVoxel(pos + VoxelData.faceChecks[p]);

            // ���� ���� �ܺο� �����ִ��� Ȯ��
            if (neighbor != null && World.Instance.blockTypes[neighbor.id].renderNeighborFaces)
            {
                // ���� Voxel�� ID�� ������
                //byte blockID = chunkData.map[(int)pos.x, (int)pos.y, (int)pos.z];





                // �� ���� ������ �߰�
                // �� ���� �簢���̴ϱ� 4��...
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                }

                for (int i = 0; i < 4; i++)
                {
                    normals.Add(VoxelData.faceChecks[p]);
                }
                // �ؽ��� ��ǥ �߰�
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

                    // �ﰢ�� �ε��� �߰�
                }

                // �������� ù��° ���� �ε��� �����ϱ� ���� 4��ŭ ����
                vertexIndex += 4;


            }
        }

    }


    /// ���ο� mesh ��ü ���� -> ������ ������ ����, �ﰢ�� �ε���, UV ��ǥ�� mesh�� �Ҵ�
    /// -> �������� ����(������) -> �� ������ mesh�� meshFileter�� �Ҵ��ؼ� ������

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

        // �������� ����
        //mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }



    private void AddTexture(int textureID)
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

    private void AddTextureForBlock(int textureID)
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


    // �� Chunk �� ���� ������ �Ǵ�
    // �����??? -> �÷��̾� ��ġ ������Ʈ, Ȱ�� Chunk ����
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
//    public NativeArray<byte> chunkData.map; // Voxel ������
//}