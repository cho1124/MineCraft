using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {

    public static float Get2DPerlin (Vector2 position, float offset, float scale)
    {
        return Mathf.PerlinNoise((position.x + 0.1f) / VoxelData.ChunkWidth * scale + offset, (position.y + 0.1f) / VoxelData.ChunkWidth * scale + offset);
    }



    public static float Get3DPerlin(Vector3 position, float offset, float scale)
    {
        float x = (position.x + offset) * scale;
        float y = (position.y + offset) * scale;
        float z = (position.z + offset) * scale;
        x += 0.1f;
        y += 0.1f;
        z += 0.1f;

        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float zx = Mathf.PerlinNoise(z, x);

        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float xz = Mathf.PerlinNoise(x, z);

        return (xy + yz + zx + yx + zy + xz) / 6f * 2 - 0.7f;
    }




    public static bool Get3DPerlin (Vector3 position, float offset, float scale, float threshold)
    {
        float noiseValue = Get3DPerlin(position, offset, scale);
        return noiseValue > threshold;
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //public int length;
    //public float stepSize = 1.0f;
    //public float noiseScale = 0.1f;
    //public int maxCount = 4;
    //
    //
    //public BiomeAttribute biome;
    //public Vector3[] wormPath;
    //public Vector3 worldScanner;
    //public World world;
    ///void Start()
    ///{
    ///    wormPath = new Vector3[length];
    ///    GenerateWorm();
    ///    //PrintWormPath();
    ///}
    //public void GenerateWorm()
    //{
    //    world = GameObject.Find("World").GetComponent<World>();
    //    wormPath = new Vector3[length];
    //    Vector3 currentPosition = new Vector3(Random.Range(0, VoxelData.ChunkWidth * VoxelData.worldSizeInChunks), Random.Range(1, biome.terrainHeight), VoxelData.ChunkWidth * VoxelData.worldSizeInChunks);
    //    Vector3 worldScanner = Vector3.zero;
    //
    //    for (int c = 0; c < maxCount; c++)
    //    {
    //
    //        for (int i = 0; i < length; i++)
    //        {
    //            // 3차원 펄린 노이즈로 각도 계산
    //            float angleX = Mathf.PerlinNoise(currentPosition.x * noiseScale, currentPosition.z * noiseScale) * Mathf.PI * 2.0f;
    //            float angleY = Mathf.PerlinNoise(currentPosition.y * noiseScale, currentPosition.z * noiseScale) * Mathf.PI * 2.0f;
    //            
    //            Vector3 direction = new Vector3(Mathf.Cos(angleX) * Mathf.Sin(angleY), Mathf.Sin(angleX) * Mathf.Sin(angleY), Mathf.Cos(angleY));
    //            
    //            currentPosition += direction * stepSize;
    //            
    //            wormPath[i] = currentPosition;
    //        }
    //    }
    //
    //    for(int i = 0; i < length; i++)
    //    {
    //
    //        if (world.CheckForVoxel(new Vector3(wormPath[i].x, wormPath[i].y, wormPath[i].z)))
    //        { 
    //        
    //        
    //        
    //        }
    //    }

        //for(int x = 0; x < VoxelData.ChunkWidth * VoxelData.worldSizeInChunks; x++)
        //{
        //    for(int z = 0; z < VoxelData.ChunkWidth * VoxelData.worldSizeInChunks; z++)
        //    {
        //        for(int y = 0; y < biome.terrainHeight; y++)
        //        {
        //            wormPath
        //        }
        //    }
        //}
    }


//}