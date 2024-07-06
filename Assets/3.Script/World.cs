using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class World : MonoBehaviour
{

    public int seed;
    // 플레이어
    public Transform player;
    public Vector3 spawnPoint;


    // blocks
    public Material material;
    public BlockType[] blockTypes;

    // chunk
    Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord playerLastChunkCoord;


    // perlin
    public int perlinOffset = 500;
    public float perlinScale = 0.25f;


    private void Start()
    {
        UnityEngine.Random.InitState(seed);


        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.transform.position);
    }
    private void Update()
    {

        if (!GetChunkCoordFromVector3(player.transform.position).Equals(playerLastChunkCoord))
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

                CreateChunk(new ChunkCoord(x, z));

            }
        }

        spawnPoint = new Vector3(VoxelData.worldSizeInBlocks / 2, VoxelData.ChunkHeight + 2, VoxelData.worldSizeInBlocks / 2);
        player.position = spawnPoint;

    }

    private void CheckViewDistance()
    {

        int chunkX = Mathf.FloorToInt(player.position.x / VoxelData.ChunkWidth);
        int chunkZ = Mathf.FloorToInt(player.position.z / VoxelData.ChunkWidth);

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = chunkX - VoxelData.viewDistanceInChunks / 2; x < chunkX + VoxelData.viewDistanceInChunks / 2; x++)
        {
            for (int z = chunkZ - VoxelData.viewDistanceInChunks / 2; z < chunkZ + VoxelData.viewDistanceInChunks / 2; z++)
            {

                // If the chunk is within the world bounds and it has not been created.
                if (IsChunkInWorld(x, z))
                {

                    ChunkCoord thisChunk = new ChunkCoord(x, z);

                    if (chunks[x, z] == null)
                        CreateChunk(thisChunk);
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(thisChunk);
                    }
                    // Check if this chunk was already in the active chunks list.
                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                    {

                        //if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        if (previouslyActiveChunks[i].x == x && previouslyActiveChunks[i].z == z)
                            previouslyActiveChunks.RemoveAt(i);

                    }

                }
            }
        }

        foreach (ChunkCoord coord in previouslyActiveChunks)
            chunks[coord.x, coord.z].isActive = false;

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

        // 기본 terrain

        int terrainHeight = Mathf.FloorToInt(VoxelData.ChunkHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), perlinOffset, perlinScale));
        if (yPos == terrainHeight)
            return 3;
        else if (yPos > terrainHeight)
            return 0;
        else
            return 2;


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

    private void CreateChunk(ChunkCoord coord)
    {

        chunks[coord.x, coord.z] = new Chunk(new ChunkCoord(coord.x, coord.z), this);
        activeChunks.Add(new ChunkCoord(coord.x, coord.z));


    }

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

