using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class World : MonoBehaviour
{
    // 플레이어
    public Transform player;
    public Vector3 spawnPoint;


    //  ================================================== //
    //  ================= 월드 생성 관련 ================== //
    //  ================================================== //


    public int seed;
    //public int seedOffset;
    public BiomeAttribute biome;


    // blocks
    public Material material;
    public BlockType[] blockTypes;

    // chunk
    Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();

    public ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    private bool isCreatingChunks;


    public GameObject debugScreen;



    //// perlin
    //public int perlinOffset = 500;
    //public float perlinScale = 0.25f;
    //



    //월드 생성
    private void Start()
    {
        UnityEngine.Random.InitState(seed);
        //seedOffset = UnityEngine.Random.Range(100, 10000);

        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.transform.position);
    }





    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);

        if (!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();

        if (chunksToCreate.Count > 0 && !isCreatingChunks)
            StartCoroutine(CreateChunks_co());

        if (Input.GetKeyDown(KeyCode.F3))
            debugScreen.SetActive(!debugScreen.activeSelf);
    }



    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }




    void GenerateWorld()
    {

        for (int x = VoxelData.worldSizeInChunks / 2 - VoxelData.viewDistanceInChunks / 2; x < VoxelData.worldSizeInChunks / 2 + VoxelData.viewDistanceInChunks / 2; x++)
        {
            for (int z = VoxelData.worldSizeInChunks / 2 - VoxelData.viewDistanceInChunks / 2; z < VoxelData.worldSizeInChunks / 2 + VoxelData.viewDistanceInChunks / 2; z++)
            {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);
                activeChunks.Add(new ChunkCoord(x, z));
            }
        }

        spawnPoint = new Vector3(VoxelData.worldSizeInBlocks / 2, VoxelData.ChunkHeight - 50f, VoxelData.worldSizeInBlocks / 2);
        player.position = spawnPoint;

    }

    IEnumerator CreateChunks_co()
    {
        isCreatingChunks = true;


        while(chunksToCreate.Count > 0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }


        isCreatingChunks = false;

    }


    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - VoxelData.viewDistanceInChunks; x < coord.x + VoxelData.viewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.viewDistanceInChunks; z < coord.z + VoxelData.viewDistanceInChunks; z++)
            {
                if (IsChunkInWorld(new ChunkCoord(x,z)))
                {
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                        //CreateChunk(x, z);
                    }
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        //activeChunks.Add(new ChunkCoord(x, z));
                    }
                    activeChunks.Add(new ChunkCoord(x, z));
                }

                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);
                }
            }
        }

        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
    }



    public bool CheckForVoxel(Vector3 pos)
    {

        //ChunkCoord thisChunk = new ChunkCoord(pos);
        //
        //if(!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
        //{
        //    return false;
        //}
        //
        //if(chunks[thisChunk.x, thisChunk.z]!= null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
        //{
        //    return blockTypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;
        //}
        //
        //return blockTypes[GetVoxel(pos)].isSolid;
        //--------------------------------------------
        ChunkCoord thisChunk = new ChunkCoord(pos);
        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y >= VoxelData.ChunkHeight)
        {
            return false;
        }


        Chunk chunk = chunks[thisChunk.x, thisChunk.z];

        if (chunk != null && chunk.isVoxelMapPopulated)
        {
            int xCheck = Mathf.FloorToInt(pos.x) - (thisChunk.x * VoxelData.ChunkWidth);
            int yCheck = Mathf.FloorToInt(pos.y);
            int zCheck = Mathf.FloorToInt(pos.z) - (thisChunk.z * VoxelData.ChunkWidth);

            if (chunk.IsVoxelInChunk(xCheck, yCheck, zCheck))
            {
                return blockTypes[chunk.voxelMap[xCheck, yCheck, zCheck]].isSolid;
            }
        }

        return blockTypes[GetVoxel(pos)].isSolid;

        //int xCheck = Mathf.FloorToInt(_x);
        //int yCheck = Mathf.FloorToInt(_y);
        //int zCheck = Mathf.FloorToInt(_z);
        //
        //int xChunk = xCheck / VoxelData.ChunkWidth;
        //int zChunk = zCheck / VoxelData.ChunkWidth;
        //
        //
        //xCheck -= (xChunk * VoxelData.ChunkWidth);
        //zCheck -= (zChunk * VoxelData.ChunkWidth);
        //
        //return blockTypes[chunks[xChunk, zChunk].voxelMap[xCheck, yCheck, zCheck]].isSolid;

    }





    // 아마 맵 생성할때 동굴이라던가 바이옴이라던가 등등 주로 여기서 쓰일것
    public byte GetVoxel(Vector3 pos)
    {
        // ImmutablePass
        int yPos = Mathf.FloorToInt(pos.y);


        // 월드 밖이라면
        if (!IsVoxelInWorld(pos))
            return 0;

        // 청크 제일 밑바닥
        if (yPos == 0)
            return 1;

        // =============================================================================================================================================== //
        // ========================================================     첫번째 생성    ==================================================================== //
        // =============================================================================================================================================== //

        // 펄린 노이즈 사용해서...
        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;

        byte voxelValue = 0;

        if (yPos == terrainHeight)
        {
            voxelValue = 3;
        }
        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
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
        // ========================================================     두번째 생성    ==================================================================== //
        // =============================================================================================================================================== //

        if (voxelValue == 2)
        {
            foreach (Lode lode in biome.lodes)
            {
                if(yPos > lode.minHeight && yPos < lode.maxHeight)
                {
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;

                }
            }
        }
        return voxelValue;



        //// 바닥
        //if (pos.x < 0 || pos.x > VoxelData.worldSizeInBlocks - 1 || pos.y < 0 || pos.y > VoxelData.ChunkHeight - 1 || pos.z < 0 || pos.z > VoxelData.worldSizeInBlocks - 1)
        //    //voxelMap[x, y, z] = 1;
        //    return 0;
        //if (pos.y < 1)
        //    return 1;
        //// 최상층
        //else if (pos.y == VoxelData.ChunkHeight - 1)
        //{
        //    float tempNoise = Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, 0.1f);
        //    if (tempNoise < 0.5)
        //        return 3;
        //    else
        //        return 4;
        //
        //}
        //    //voxelMap[x, y, z] = 3;
        //    //return 3;
        //// 중간
        //else
        //    //voxelMap[x, y, z] = 2;
        //    return 2;
    }

    //void CreateChunk(int x, int z)
    //{
    //    chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
    //    activeChunks.Add(new ChunkCoord(x, z));
    //}


    bool IsChunkInWorld(ChunkCoord coord)
    {

        return coord.x >= 0 && coord.x < VoxelData.worldSizeInChunks &&
               coord.z >= 0 && coord.z < VoxelData.worldSizeInChunks;

    }


    public bool IsVoxelInWorld(Vector3 pos)
    {
        // if (pos.x > 0 && pos.x < VoxelData.worldSizeInBlocks - 1 &&
        //     pos.y > 0 && pos.y < VoxelData.ChunkHeight - 1 &&
        //     pos.z > 0 && pos.z < VoxelData.worldSizeInBlocks - 1)
        //     return true;
        // else
        //     return false;

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
                Debug.Log("Error in GetTextureID; invalid face index"); ;
                return 0;
        }

    }




}

