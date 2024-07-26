using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
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
    List<int> waterTriangles = new List<int>();

    List<Color> colors = new List<Color>();

    Material[] materials = new Material[3];



    // 2d �ؽ��ĸ� 3d �𵨷� ���εǵ���...
    List<Vector2> uvs = new List<Vector2>();




    // chunk�� Ȱ��ȭ ���� -> ����ȭ
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



        //// �ð��� �޽� -> �浹 �޽� ����
        meshCollider.sharedMesh = meshFilter.mesh;

        lock (World.Instance.ChunkUpdateThreadLock)
            World.Instance.chunksToUpdate.Add(this);


    }



    // ���⿡ populatechunkData.map�� �־��� 20240717
    // Chunk ���� �� ��ġ�� � ����� ������ ä���



    // Chunk ������Ʈ (���κи�), �޽� ���� 
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

    // �޽� ������ �ʱ�ȭ
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




    // ������ ûũ�� �ִ��� �˻�
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

        // Chunk�� ��ġ�� ���� ������ǥ -> ûũ���� �����ǥ�� ��ȯ
        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        // ���� ��� ID ����
        byte currentBlockID = chunkData.map[xCheck, yCheck, zCheck].id;

        chunkData.ModifyVoxel(new Vector3Int(xCheck, yCheck, zCheck), newID, World.Instance._player.orientation);
        // ���ο� ��� ID�� ����



        lock (World.Instance.ChunkUpdateThreadLock)
        {
            World.Instance.chunksToUpdate.Insert(0, this);
            // ����� �ֺ� Voxel ������Ʈ
            UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
        }
        //// Chunk ������Ʈ


        // ���� ��� ID ��ȯ
        return currentBlockID;
    }




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


    // ������ �� ���� �˻��ؼ� ���̴� �鸸 �޽� �����Ϳ� �߰�
    // UpdateChunk���� for���� 3������ ������ �� Chunk�� ���̴� �鸸 �������ؼ� ���Ӽ����� ����ȭ

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




        // ����� 6��ü
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


            // face check(���� �ٶ󺸴� �������� +1 �̵��Ͽ� Ȯ��) ������
            // soild(������� �ƴ�)�� �ƴѰ�쿡�� ť���� ���� �׷�������....
            // -> ûũ�� �ܰ� �κи� ���� �׷�����, ���ο��� ���� �׷����� �ʵ��� ����ȭ 
            VoxelState neighbour = CheckVoxel(pos + VoxelData.faceChecks[translatedP]);


            // ���� ���� �ܺο� �����ִ��� Ȯ��
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


    /// ���ο� mesh ��ü ���� -> ������ ������ ����, �ﰢ�� �ε���, UV ��ǥ�� mesh�� �Ҵ�
    /// -> �������� ����(������) -> �� ������ mesh�� meshFileter�� �Ҵ��ؼ� ������

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

        // �������� ����
        //mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }



    private void AddTexture(int textureID, Vector2 uv)
    {

        float y = textureID / VoxelData.textureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);

        // �ؽ��� ��Ʋ�� ����ȭ
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
