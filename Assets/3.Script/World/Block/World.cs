using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;


public class World : MonoBehaviour
{
    /// Chunk�� ������� World ����
    /// ĳ���� �ֺ� ���� ���� ���� Chunk�� ���̰� �ؼ� ����ȭ
    /// 
    /// 
    ///
    




    // �÷��̾�
    public Transform player;
    public Vector3 spawnPoint;
    public Camera camera;


    //  ================================================== //
    //  ================= ���� ���� ���� ================== //
    //  ================================================== //


    [Header("World Generation Values")]
    public int seed;
    //public int seedOffset;
    public BiomeAttribute biome;


    //for shader
    [Range(0.95f, 0f)]
    public float globalLightLevel;
    public Color day;
    public Color night;

    [Header("Performance")]
    public bool enableThreading;


    // blocks
    public Material material;
    public Material transparentMaterial;

    public BlockType[] blockTypes;
    public GameObject ItemBlock;

    // chunk
    public Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    public Queue<Chunk> ChunksToDraw = new Queue<Chunk>();


    public ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    List<Chunk> chunksToUpdate = new List<Chunk>();

    bool applyingModifications = false;

    //private bool isCreatingChunks;

    Queue<Queue<VoxelMod>> modifications = new Queue<Queue<VoxelMod>>();


    // ���� ����
    //[SerializeField]
    //public CaveGenerator caveGenerator;
    public float caveOffset;
    public float caveScale;

    // player�� ���� ����� ��ũ��
    public GameObject debugScreen;
    private Stopwatch stopwatch;




    //���� ����
    private void Start()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();

        Shader.SetGlobalFloat("minGlobalLightLevel", VoxelData.minLightLevel);
        Shader.SetGlobalFloat("maxGlobalLightLevel", VoxelData.maxLightLevel);

        UnityEngine.Random.InitState(seed);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.transform.position);
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
    }





    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);
        camera.backgroundColor = Color.Lerp(day, night, globalLightLevel);

        // ���̴� �۷ι�����Ʈ ����
        Shader.SetGlobalFloat("GlobalLightLevel", globalLightLevel);


        if (!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();

        //if (chunksToCreate.Count > 0 && !isCreatingChunks)
        //    StartCoroutine(CreateChunks_co());

        if (!applyingModifications)
        {
            ApplyModifications();
        }

        if (chunksToCreate.Count > 0)
        {
            CreateChunk();
        }
        if (chunksToUpdate.Count > 0)
        {
            UpdateChunks();
        }

        if (ChunksToDraw.Count > 0)
        {
            lock (ChunksToDraw)
            {
                if (ChunksToDraw.Peek().isEditable)
                    ChunksToDraw.Dequeue().CreateMesh();
            }
        }
        if (Input.GetKeyDown(KeyCode.F3))
            debugScreen.SetActive(!debugScreen.activeSelf);
    }



    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return chunks[x, z];
    }



    void GenerateWorld()
    {
        //PerlinWorm perlinWorm = new PerlinWorm();
        //List<Vector3Int> wormPositions = perlinWorm.CreateWorm(VoxelData.worldSizeInBlocks / 2, VoxelData.worldSizeInBlocks / 2);


        for (int x = VoxelData.worldSizeInChunks / 2 - VoxelData.viewDistanceInChunks / 2; x < VoxelData.worldSizeInChunks / 2 + VoxelData.viewDistanceInChunks / 2; x++)
        {
            for (int z = VoxelData.worldSizeInChunks / 2 - VoxelData.viewDistanceInChunks / 2; z < VoxelData.worldSizeInChunks / 2 + VoxelData.viewDistanceInChunks / 2; z++)
            {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
                activeChunks.Add(new ChunkCoord(x, z));
            }
        }

        //while(modifications.Count > 0)
        //{
        //    VoxelMod v = modifications.Dequeue();
        //    ChunkCoord c = GetChunkCoordFromVector3(v.position);
        //
        //    if(chunks[c.x,c.z] == null)
        //    {
        //        chunks[c.x, c.z] = new Chunk(c, this, true);
        //        activeChunks.Add(c);
        //    }
        //
        //    chunks[c.x, c.z].modifications.Enqueue(v);
        //    if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
        //    {
        //
        //
        //        chunksToUpdate.Add(chunks[c.x, c.z]);
        //    }
        //}

        //for(int i =0; i < chunksToUpdate.Count; i++)
        //{
        //    chunksToUpdate[0].UpdateChunk();
        //    chunksToUpdate.RemoveAt(0);
        //}

        spawnPoint = new Vector3(VoxelData.worldSizeInBlocks / 2, VoxelData.ChunkHeight - 80f, VoxelData.worldSizeInBlocks / 2) ;
        player.position = spawnPoint;

    }

    void CreateChunk()
    {
        ChunkCoord c = chunksToCreate[0];
        chunksToCreate.RemoveAt(0);
        activeChunks.Add(c);
        chunks[c.x, c.z].Init();
    }

    //void UpdateChunks()
    //{
    //    bool updated = false;
    //    int index = 0;
    //    while(!updated && index < chunksToUpdate.Count - 1)
    //    {
    //        if (chunksToUpdate[index].isVoxelMapPopulated)
    //        {
    //            chunksToUpdate[index].UpdateChunk();
    //            chunksToUpdate.RemoveAt(index);
    //            updated = true;
    //        }
    //        else
    //        {
    //            index++;
    //        }
    //    }
    //}


    void UpdateChunks()
    {
        bool updated = false;
        int index = 0;
        while (!updated && index < chunksToUpdate.Count - 1)
        {
            if (chunksToUpdate[index].isEditable)
            {
                chunksToUpdate[index].UpdateChunk();
                chunksToUpdate.RemoveAt(index);
                updated = true;
            }
            else
            {
                index++;
            }
        }
    }


    //void ApplyModifications()
    //{
    //
    //    applyingModifications = true;
    //
    //    while (modifications.Count > 0)
    //    {
    //
    //        Queue<VoxelMod> queue = modifications.Dequeue();
    //
    //        while (queue.Count > 0)
    //        {
    //
    //            VoxelMod v = queue.Dequeue();
    //
    //            ChunkCoord c = GetChunkCoordFromVector3(v.position);
    //
    //            if (chunks[c.x, c.z] == null)
    //            {
    //                chunks[c.x, c.z] = new Chunk(c, this, true);
    //                activeChunks.Add(c);
    //            }
    //
    //            chunks[c.x, c.z].modifications.Enqueue(v);
    //
    //            if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
    //                chunksToUpdate.Add(chunks[c.x, c.z]);
    //
    //        }
    //    }
    //
    //    applyingModifications = false;
    //
    //}

    void ApplyModifications()
    {
        applyingModifications = true;

        UnityEngine.Debug.Log("Applying modifications...");

        while (modifications.Count > 0)
        {
            Queue<VoxelMod> queue = modifications.Dequeue();

            if (queue == null)
            {
                UnityEngine.Debug.LogError("Dequeued queue is null");
                continue;
            }

            while (queue.Count > 0)
            {
                VoxelMod v = queue.Dequeue();

                if (v == null)
                {
                    UnityEngine.Debug.LogError("Dequeued VoxelMod is null");
                    continue;
                }

                ChunkCoord c = GetChunkCoordFromVector3(v.position);

                if (chunks[c.x, c.z] == null)
                {
                    chunks[c.x, c.z] = new Chunk(c, this, true);
                    activeChunks.Add(c);
                }

                if (chunks[c.x, c.z] == null)
                {
                    UnityEngine.Debug.LogError($"Chunk at {c.x}, {c.z} is null");
                    continue;
                }

                chunks[c.x, c.z].modifications.Enqueue(v);

                if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
                    chunksToUpdate.Add(chunks[c.x, c.z]);
            }
        }

        applyingModifications = false;

        UnityEngine.Debug.Log("Finished applying modifications");
    }


    //IEnumerator CreateChunks_co()
    //{
    //    isCreatingChunks = true;
    //
    //
    //    while (chunksToCreate.Count > 0)
    //    {
    //        chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
    //        chunksToCreate.RemoveAt(0);
    //        yield return null;
    //    }
    //
    //
    //    isCreatingChunks = false;
    //
    //}


    // ������ chunk �� Ȱ��ȭ�ؼ� ����ȭ

    void CheckViewDistance()
    {
        // �÷��̾��� ���� chunk ��ǥ
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);

        // �÷��̾��� ���� chunk ��ǥ ������Ʈ
        playerLastChunkCoord = playerChunkCoord;

        // ���� Ȱ��ȭ �� chunk ����� �����Ͽ� ����
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        activeChunks.Clear();
        // �÷��̾� �þ� �Ÿ� ���� ûũ �˻�
        for (int x = coord.x - VoxelData.viewDistanceInChunks; x < coord.x + VoxelData.viewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.viewDistanceInChunks; z < coord.z + VoxelData.viewDistanceInChunks; z++)
            {
                // chunk �� ���� ���� �ִ��� Ȯ��
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    // chunk�� ������ ���ο� chunk ����
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                        //CreateChunk(x, z);
                    }
                    // chunk�� ��Ȱ��ȭ �� ��쿣 Ȱ��ȭ 
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        //activeChunks.Add(new ChunkCoord(x, z));
                    }

                    // ���� chunk�� Ȱ�� chunk��Ͽ� �߰�
                    activeChunks.Add(new ChunkCoord(x, z));
                }

                // ���� Ȱ��ȭ �� chunk ��Ͽ��� ����
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);
                }
            }
        }

        // �þ߿� ��� chunk�� ��Ȱ��ȭ
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
    }



    // CheckVoxel���� �ٸ��� ���� ���ο��� �˻� -> ������ �� ŭ
    public bool CheckForVoxel(Vector3 pos)
    {
    
        ChunkCoord thisChunk = new ChunkCoord(pos);
        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y >= VoxelData.ChunkHeight)
        {
            return false;
        }
    
    
        Chunk chunk = chunks[thisChunk.x, thisChunk.z];
    
        if (chunk != null && chunk.isEditable)
        {
            int xCheck = Mathf.FloorToInt(pos.x) - (thisChunk.x * VoxelData.ChunkWidth);
            int yCheck = Mathf.FloorToInt(pos.y);
            int zCheck = Mathf.FloorToInt(pos.z) - (thisChunk.z * VoxelData.ChunkWidth);
    
            if (chunk.IsVoxelInChunk(xCheck, yCheck, zCheck))
            {
                return blockTypes[chunk.voxelMap[xCheck, yCheck, zCheck].id].isSolid;
            }
        }
    
        return blockTypes[GetVoxel(pos)].isSolid;
    
    }
    public VoxelState GetVoxelState(Vector3 pos)
    {

        ChunkCoord thisChunk = new ChunkCoord(pos);
        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y >= VoxelData.ChunkHeight)
        {
            return null;
        }


        Chunk chunk = chunks[thisChunk.x, thisChunk.z];

        if (chunk != null && chunk.isEditable)
        {
            int xCheck = Mathf.FloorToInt(pos.x) - (thisChunk.x * VoxelData.ChunkWidth);
            int yCheck = Mathf.FloorToInt(pos.y);
            int zCheck = Mathf.FloorToInt(pos.z) - (thisChunk.z * VoxelData.ChunkWidth);

            if (chunk.IsVoxelInChunk(xCheck, yCheck, zCheck))
            {
                return chunks[thisChunk.x,thisChunk.z].GetVoxelFromGlobalVector3(pos);
            }
        }

        return new VoxelState(GetVoxel(pos));

    }



    public bool CheckIfVoxelTransparent(Vector3 pos)
    {

        ChunkCoord thisChunk = new ChunkCoord(pos);
        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y >= VoxelData.ChunkHeight)
        {
            return false;
        }


        Chunk chunk = chunks[thisChunk.x, thisChunk.z];

        if (chunk != null && chunk.isEditable)
        {
            int xCheck = Mathf.FloorToInt(pos.x) - (thisChunk.x * VoxelData.ChunkWidth);
            int yCheck = Mathf.FloorToInt(pos.y);
            int zCheck = Mathf.FloorToInt(pos.z) - (thisChunk.z * VoxelData.ChunkWidth);

            if (chunk.IsVoxelInChunk(xCheck, yCheck, zCheck))
            {
                return blockTypes[chunk.voxelMap[xCheck, yCheck, zCheck].id].renderNeighborFaces;
            }
        }

        return blockTypes[GetVoxel(pos)].renderNeighborFaces;

    }





    // �Ƹ� �� �����Ҷ� �����̶���� ���̿��̶���� ��� �ַ� ���⼭ ���ϰ�
    public byte GetVoxel(Vector3 pos)
    {
        // ImmutablePass
        int yPos = Mathf.FloorToInt(pos.y);


        // ���� ���̶��
        if (!IsVoxelInWorld(pos))
            return 0;


        // ûũ ���� �عٴ�
        if (yPos == 0)
            return 1;

        // =============================================================================================================================================== //
        // ========================================================     ù��° ����    ==================================================================== //
        // =============================================================================================================================================== //

        // �޸� ������ ����ؼ�...
        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;

        byte voxelValue = 0;



        if (yPos == terrainHeight)
        {
            // grass
            voxelValue = 3;
        }
        else if


        (yPos < terrainHeight && yPos > terrainHeight - 4) // - 4);
        {

            voxelValue = 5; //return 5; 
        }
        else if (yPos > terrainHeight)
        {
            return 0;
        }
        else
        {
            voxelValue = 2;//return 2;

        }
        // =============================================================================================================================================== //
        // ========================================================     �ι�° ����    ==================================================================== //
        // =============================================================================================================================================== //
        if (yPos < terrainHeight + 20)
        {

            float airPocket = Noise.Get3DPerlin(pos, caveOffset, caveScale);
            float airPocketBorder = Noise.Get3DPerlin(pos, caveOffset + 0.1f, caveScale);
            if (airPocket > 0.6f)
            {
                return 0;

            }
            if (airPocketBorder > 0.4f && airPocketBorder < 0.59f)
            {
                return 0;
            }
        }

        if (voxelValue == 2)
        {



            foreach (Lode lode in biome.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                {
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                    {
                        //if(lode.blockID == 0)
                        //{
                        //    return voxelValue = 0;
                        //}
                        voxelValue = lode.blockID;
                    }


                }
            }





        }


        // =============================================================================================================================================== //
        // ========================================================     ����° ����    ==================================================================== //
        // =============================================================================================================================================== //


        if (yPos == terrainHeight)
        {
            if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.treeZoneScale) > biome.treeZoneThreshold)
            {
 
                if(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.treePlacementScale) > biome.treePlacementThreshold)
                {
                    //modifications.Enqueue(new VoxelMod(new Vector3(pos.x, pos.y + 1, pos.z), 6));
                    modifications.Enqueue(Structure.MakeTree(pos, biome.minTreeHeight, biome.maxTreeHeight));
                }
            }
        }
        return voxelValue;



    }



    bool IsChunkInWorld(ChunkCoord coord)
    {

        return coord.x >= 0 && coord.x < VoxelData.worldSizeInChunks &&
               coord.z >= 0 && coord.z < VoxelData.worldSizeInChunks;

    }


    public bool IsVoxelInWorld(Vector3 pos)
    {


        return pos.x >= 0 && pos.x < VoxelData.worldSizeInBlocks &&
               pos.y >= 0 && pos.y < VoxelData.ChunkHeight &&
               pos.z >= 0 && pos.z < VoxelData.worldSizeInBlocks;
    }
}






[System.Serializable]

public class BlockType
{
    public string blockName;
    public bool isSolid;
    public bool renderNeighborFaces;
    public float transparency;

    [Header("Texture IDs")]
    public int topFaceTexture;
    public int frontFaceTexture;
    public int backFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;
    public int bottomFaceTexture;


    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0: return backFaceTexture;
            case 1: return frontFaceTexture;
            case 2: return topFaceTexture;
            case 3: return bottomFaceTexture;
            case 4: return leftFaceTexture;
            case 5: return rightFaceTexture;

            default:
                UnityEngine.Debug.Log("Error in GetTextureID; invalid face index"); ;
                return 0;
        }

    }




}

public class VoxelMod
{
    public Vector3 position;
    public byte id;

    public VoxelMod()
    {
        position = new Vector3();
        id = 0;
    }

    public VoxelMod (Vector3 _position, byte _id)
    {
        position = _position;
        id = _id;
    }
}
