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




    // Voxel��, Chunk �ȿ� � ������ �������� byte������ 3���� �迭
    //public byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    public VoxelState[,,] voxelMap = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    World world;

    // chunk�� Ȱ��ȭ ���� -> ����ȭ
    private bool _isActive;

    // voxel���� ä�������� ����
    public bool isVoxelMapPopulated = false;

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

    public bool isEditable
    {
        get
        {

            if (!isVoxelMapPopulated)
                return false;
            else
                return true;
        }
    }

    // ������
    //public Chunk(ChunkCoord _coord, World _world, bool generateOnLoad)
    //{
    //
    //    coord = _coord;
    //    world = _world;
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


    public Chunk(ChunkCoord _coord, World _world)
    {

        coord = _coord;
        world = _world;
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
        //meshRenderer.material = world.material;
        materials[0] = world.material;
        materials[1] = world.transparentMaterial;

        meshRenderer.materials = materials;
        meshCollider = chunkObject.AddComponent<MeshCollider>();



        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = coord.x + ", " + coord.z;
        position = chunkObject.transform.position;

        //if (world.enableThreading)
        //{
        //
        //    Thread myThread = new Thread(new ThreadStart(PopulateVoxelMap));
        //    myThread.Start();
        //}
        //else
            PopulateVoxelMap();

        //PopulateVoxelMap();
        //UpdateChunk();


        ////CreateMeshData();
        //
        ////CreateMesh();


        // �ð��� �޽� -> �浹 �޽� ����
        meshCollider.sharedMesh = meshFilter.mesh;
    }


    // Chunk ���� �� ��ġ�� � ������ ������ ä���
    void PopulateVoxelMap()
    {

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    voxelMap[x, y, z] = new VoxelState(world.GetVoxel(new Vector3(x, y, z) + position));
                    //Debug.Log($"Position: {voxelMap[x,y,z]}, Noise Value: {noiseValue}, Voxel Value: {voxelValue}");



                }
            }
        }
        //UpdateChunk();
        isVoxelMapPopulated = true;


        lock (world.ChunkUpdateThreadLock)
        {

        world.chunksToUpdate.Add(this);
        }

    }

    //public void UpdateChunk()
    //{
    //    //_updateChunk();
    //    if (world.enableThreading)
    //    {
    //        Thread myThread = new Thread(new ThreadStart(_updateChunk));
    //        myThread.Start();
    //    }
    //    else
    //        _updateChunk();
    //}


    // Chunk ������Ʈ (�����κи�), �޽� ���� 
    public void UpdateChunk()
    {
        //threadLocked = true;

        while (modifications.Count > 0)
        {
            VoxelMod v = modifications.Dequeue();
            Vector3 pos = v.position -= position;
            voxelMap[(int)pos.x, (int)pos.y, (int)pos.z].id = v.id;
        }


        ClearMeshData();
        CalculateLight();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (world.blockTypes[voxelMap[x, y, z].id].isSolid)
                    {
                        UpdateMeshData(new Vector3(x, y, z));
                        //AddVoxelDataToChunk(new Vector3(x, y, z)); 
                    }

                }
            }
        }

        //lock (world.ChunksToDraw)
        //{
            //CreateMesh();
            world.ChunksToDraw.Enqueue(this);
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

        // ���� ���� ID ����
        byte currentBlockID = voxelMap[xCheck, yCheck, zCheck].id;

        // ���ο� ���� ID�� ����
        voxelMap[xCheck, yCheck, zCheck].id = newID;


        lock (world.ChunkUpdateThreadLock)
        {
            world.chunksToUpdate.Insert(0,this);
            // ����� �ֺ� Voxel ������Ʈ
            UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
        }
        //// Chunk ������Ʈ
        //UpdateChunk();

        // ���� ���� ID ��ȯ
        return currentBlockID;
    }

    //public byte EditVoxelCave(Vector3 pos, byte newID)
    //{
    //    int xCheck = Mathf.FloorToInt(pos.x);
    //    int yCheck = Mathf.FloorToInt(pos.y);
    //    int zCheck = Mathf.FloorToInt(pos.z);
    //    voxelMap[xCheck, yCheck, zCheck] = newID;
    //
    //    // ����� �ֺ� Voxel ������Ʈ
    //    UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
    //
    //    // Chunk ������Ʈ
    //    UpdateChunk();
    //    byte currentBlockID = voxelMap[xCheck, yCheck, zCheck];
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
                world.chunksToUpdate.Insert(0, world.GetChunkFromVector3(currentVoxel + position)); //.UpdateChunk();
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
    //        return world.CheckIfVoxelTransparent(pos + position);//world.blockTypes[world.GetVoxel(pos + position)].isSolid;
    //
    //    return world.blockTypes[voxelMap[x, y, z].id].renderNeighborFaces;
    //
    //}
    VoxelState CheckVoxel(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.GetVoxelState(pos + position);//world.blockTypes[world.GetVoxel(pos + position)].isSolid;

        return voxelMap[x, y, z];

    }



    public VoxelState GetVoxelFromGlobalVector3(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(position.x);
        zCheck -= Mathf.FloorToInt(position.z);


        return voxelMap[xCheck, yCheck, zCheck];
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

                    VoxelState thisVoxel = voxelMap[x, y, z];

                    if (thisVoxel.id > 0 && world.blockTypes[thisVoxel.id].transparency < lightRay)
                        lightRay = world.blockTypes[thisVoxel.id].transparency;

                    thisVoxel.globalLightPercent = lightRay;

                    voxelMap[x, y, z] = thisVoxel;

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

                if(IsVoxelInChunk(neighbor.x, neighbor.y, neighbor.y))
                {
                    if(voxelMap[neighbor.x, neighbor.y, neighbor.y].globalLightPercent < voxelMap[v.x,v.y,v.z].globalLightPercent - VoxelData.lightFalloff)
                    {
                        voxelMap[neighbor.x, neighbor.y, neighbor.y].globalLightPercent = voxelMap[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFalloff;

                        if(voxelMap[neighbor.x, neighbor.y, neighbor.y].globalLightPercent > VoxelData.lightFalloff)
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

        //byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z].id;
        byte blockID = voxelMap[x, y, z].id;
        //bool renderNeighborFaces = world.blockTypes[blockID].renderNeighborFaces;

        // ������ 6��ü
        for (int p = 0; p < 6; p++)
        {

            // face check(���� �ٶ󺸴� �������� +1 �̵��Ͽ� Ȯ��) ������
            // soild(������� �ƴ�)�� �ƴѰ�쿡�� ť���� ���� �׷�������....
            // -> ûũ�� �ܰ� �κи� ���� �׷�����, ���ο��� ���� �׷����� �ʵ��� ����ȭ 
            VoxelState neighbor = CheckVoxel(pos + VoxelData.faceChecks[p]);

            // ���� ���� �ܺο� �����ִ��� Ȯ��
            if (neighbor != null && world.blockTypes[neighbor.id].renderNeighborFaces)
            {
                // ���� Voxel�� ID�� ������
                //byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];





                // �� ���� ������ �߰�
                // �� ���� �簢���̴ϱ� 4��...
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
                }

                // �ؽ��� ��ǥ �߰�
                AddTexture(world.blockTypes[blockID].GetTextureID(p));

                //float lightLevel;
                //int yPos = (int)pos.y + 1;
                //bool inShade = false;
                //while (yPos < VoxelData.ChunkHeight)
                //{
                //    if (voxelMap[(int)pos.x, yPos, (int)pos.z].id != 0)
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

                //if (!renderNeighborFaces)
                //{
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                //}
                //else
                //{
                //    transparentTriangles.Add(vertexIndex);
                //    transparentTriangles.Add(vertexIndex + 1);
                //    transparentTriangles.Add(vertexIndex + 2);
                //    transparentTriangles.Add(vertexIndex + 2);
                //    transparentTriangles.Add(vertexIndex + 1);
                //    transparentTriangles.Add(vertexIndex + 3);
                //
                //    // �ﰢ�� �ε��� �߰�
                //}

                // �������� ù��° ���� �ε��� �����ϱ� ���� 4��ŭ ����
                vertexIndex += 4;


            }
        }

    }


    /// ���ο� mesh ��ü ���� -> ������ ������ ����, �ﰢ�� �ε���, UV ��ǥ�� mesh�� �Ҵ�
    /// -> �������� ����(�������) -> �� ������ mesh�� meshFileter�� �Ҵ��ؼ� ������

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

        // �������� ����
        mesh.RecalculateNormals();

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

public struct ChunkData
{
    public ChunkCoord coord;
    public NativeArray<byte> voxelMap; // Voxel ������
}