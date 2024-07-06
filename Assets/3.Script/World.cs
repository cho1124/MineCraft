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

    ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;


    //// perlin
    //public int perlinOffset = 500;
    //public float perlinScale = 0.25f;
    //

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

                CreateChunk(x, z);

            }
        }

        spawnPoint = new Vector3(VoxelData.worldSizeInBlocks / 2, VoxelData.ChunkHeight + 2, VoxelData.worldSizeInBlocks / 2);
        player.position = spawnPoint;

    }

    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - VoxelData.viewDistanceInChunks; x < coord.x + VoxelData.viewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.viewDistanceInChunks; z < coord.z + VoxelData.viewDistanceInChunks; z++)
            {
                if (IsChunkInWorld(x,z))
                {
                    if (chunks[x, z] == null)
                        CreateChunk(x, z);
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(new ChunkCoord(x, z));
                    }
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

    void CreateChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunks.Add(new ChunkCoord(x, z));
    }

    //public bool CheckForVoxel(float _x, float _y, float _z)
    //{
    //
    //    int xCheck = Mathf.FloorToInt(_x);
    //    int yCheck = Mathf.FloorToInt(_y);
    //    int zCheck = Mathf.FloorToInt(_z);
    //
    //    int xChunk = xCheck / VoxelData.ChunkWidth;
    //    int zChunk = zCheck / VoxelData.ChunkWidth;
    //
    //    xCheck -= (xChunk * VoxelData.ChunkWidth);
    //    zCheck -= (zChunk * VoxelData.ChunkWidth);
    //
    //    return blockTypes[chunks[xChunk, zChunk].voxelMap[xCheck, yCheck, zCheck]].isSolid;
    //
    //}

    bool IsChunkInWorld(int x, int z)
    {

        if (x > 0 && x < VoxelData.worldSizeInChunks - 1 && z > 0 && z < VoxelData.worldSizeInChunks - 1)
            return true;
        else
            return false;

    }


    bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x > 0 && pos.x < VoxelData.worldSizeInBlocks - 1 &&
            pos.y > 0 && pos.y < VoxelData.ChunkHeight - 1 &&
            pos.z > 0 && pos.z < VoxelData.worldSizeInBlocks - 1)
            return true;
        else
            return false;
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

