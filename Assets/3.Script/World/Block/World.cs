
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;


public class World : MonoBehaviour
{
    /// Chunk를 기반으로 World 생성
    /// 캐릭터 주변 일정 범위 내의 Chunk만 보이게 해서 최적화
    /// 
    /// 
    ///

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
    public GameObject ItemBlock;

    // chunk
    public Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();

    public ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    private bool isCreatingChunks;


    // 동굴 생성
    //[SerializeField]
    //public CaveGenerator caveGenerator;
    public float caveOffset;
    public float caveScale;

    // player에 쓰일 디버그 스크린
    public GameObject debugScreen;
    private Stopwatch stopwatch;




    //월드 생성
    private void Start()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();

        UnityEngine.Random.InitState(seed);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.transform.position);
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
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

        spawnPoint = new Vector3(VoxelData.worldSizeInBlocks / 2, VoxelData.ChunkHeight - 50f, VoxelData.worldSizeInBlocks / 2);
        player.position = spawnPoint;

    }

    IEnumerator CreateChunks_co()
    {
        isCreatingChunks = true;


        while (chunksToCreate.Count > 0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }


        isCreatingChunks = false;

    }


    // 보여할 chunk 만 활성화해서 최적화

    void CheckViewDistance()
    {
        // 플레이어의 현재 chunk 좌표
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);

        // 플레이어의 이전 chunk 좌표 업데이트
        playerLastChunkCoord = playerChunkCoord;

        // 현재 활성화 된 chunk 목록을 복사하여 저장
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        // 플레이어 시야 거리 내의 청크 검사
        for (int x = coord.x - VoxelData.viewDistanceInChunks; x < coord.x + VoxelData.viewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.viewDistanceInChunks; z < coord.z + VoxelData.viewDistanceInChunks; z++)
            {
                // chunk 가 월드 내에 있는지 확인
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    // chunk가 없으면 새로운 chunk 생성
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                        //CreateChunk(x, z);
                    }
                    // chunk가 비활성화 된 경우엔 활성화 
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        //activeChunks.Add(new ChunkCoord(x, z));
                    }

                    // 현재 chunk를 활성 chunk목록에 추가
                    activeChunks.Add(new ChunkCoord(x, z));
                }

                // 이전 활성화 된 chunk 목록에서 제거
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);
                }
            }
        }

        // 시야에 벗어난 chunk를 비활성화
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
    }



    // CheckVoxel과는 다르게 월드 내부에서 검사 -> 범위가 더 큼
    public bool CheckForVoxel(Vector3 pos)
    {

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
            // grass
            voxelValue = 3;
        }
        else if


        (yPos < terrainHeight && yPos > terrainHeight - (int)UnityEngine.Random.Range(1, 10)) // - 4);
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

            //float airPocket = Noise.Get3DPerlin(pos, caveOffset, caveScale);
            //float airPocketBorder = Noise.Get3DPerlin(pos, caveOffset + 0.1f, caveScale);
            //if(airPocket > 0.6f )
            //{
            //    return 0;
            //
            //}
            //if (airPocketBorder > 0.5f && airPocketBorder < 0.59f)
            //{
            //    return 0; 
            //}
            //if(airPocketBorder>0.4f && airPocketBorder < 0.7f)
            //{
            //    return 0;
            //}



        }

        //if (yPos == terrainHeight)
        //{
        //    // grass
        //    if(UnityEngine.Random.Range(0,101) > 10)
        //        voxelValue = 3;
        //}
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
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;


public class World : MonoBehaviour
{
    /// Chunk를 기반으로 World 생성
    /// 캐릭터 주변 일정 범위 내의 Chunk만 보이게 해서 최적화
    /// 
    /// 
    ///

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
    private bool isCreatingChunks;


    // 동굴 생성
    //[SerializeField]
    //public CaveGenerator caveGenerator;
    public float caveOffset;
    public float caveScale;

    // player에 쓰일 디버그 스크린
    public GameObject debugScreen;
    private Stopwatch stopwatch;




    //월드 생성
    private void Start()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();

        UnityEngine.Random.InitState(seed);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.transform.position);
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
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

        if (ChunksToDraw.Count > 0)
            lock (ChunksToDraw)
            {
                if (ChunksToDraw.Peek().isEditable)
                    ChunksToDraw.Dequeue().CreateMesh();
            }
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

        spawnPoint = new Vector3(VoxelData.worldSizeInBlocks / 2, VoxelData.ChunkHeight - 50f, VoxelData.worldSizeInBlocks / 2);
        player.position = spawnPoint;

    }

    IEnumerator CreateChunks_co()
    {
        isCreatingChunks = true;


        while (chunksToCreate.Count > 0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }


        isCreatingChunks = false;

    }


    // 보여할 chunk 만 활성화해서 최적화

    void CheckViewDistance()
    {
        // 플레이어의 현재 chunk 좌표
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);

        // 플레이어의 이전 chunk 좌표 업데이트
        playerLastChunkCoord = playerChunkCoord;

        // 현재 활성화 된 chunk 목록을 복사하여 저장
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        activeChunks.Clear();
        // 플레이어 시야 거리 내의 청크 검사
        for (int x = coord.x - VoxelData.viewDistanceInChunks; x < coord.x + VoxelData.viewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.viewDistanceInChunks; z < coord.z + VoxelData.viewDistanceInChunks; z++)
            {
                // chunk 가 월드 내에 있는지 확인
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    // chunk가 없으면 새로운 chunk 생성
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                        //CreateChunk(x, z);
                    }
                    // chunk가 비활성화 된 경우엔 활성화 
                    else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        //activeChunks.Add(new ChunkCoord(x, z));
                    }

                    // 현재 chunk를 활성 chunk목록에 추가
                    activeChunks.Add(new ChunkCoord(x, z));
                }

                // 이전 활성화 된 chunk 목록에서 제거
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);
                }
            }
        }

        // 시야에 벗어난 chunk를 비활성화
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
    }



    // CheckVoxel과는 다르게 월드 내부에서 검사 -> 범위가 더 큼
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
                return blockTypes[chunk.voxelMap[xCheck, yCheck, zCheck]].isSolid;
            }
        }

        return blockTypes[GetVoxel(pos)].isSolid;

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
                return blockTypes[chunk.voxelMap[xCheck, yCheck, zCheck]].isTransparent;
            }
        }

        return blockTypes[GetVoxel(pos)].isTransparent;

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
        // ========================================================     두번째 생성    ==================================================================== //
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

            //float airPocket = Noise.Get3DPerlin(pos, caveOffset, caveScale);
            //float airPocketBorder = Noise.Get3DPerlin(pos, caveOffset + 0.1f, caveScale);
            //if(airPocket > 0.6f )
            //{
            //    return 0;
            //
            //}
            //if (airPocketBorder > 0.5f && airPocketBorder < 0.59f)
            //{
            //    return 0; 
            //}
            //if(airPocketBorder>0.4f && airPocketBorder < 0.7f)
            //{
            //    return 0;
            //}



        }

        //if (yPos == terrainHeight)
        //{
        //    // grass
        //    if(UnityEngine.Random.Range(0,101) > 10)
        //        voxelValue = 3;
        //}
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
    public bool isTransparent;

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
