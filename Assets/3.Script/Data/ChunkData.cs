using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ChunkData
{
    int x;
    int y;

    public Vector2Int position
    {

        get { return new Vector2Int(x, y); }
        set
        {

            x = value.x;
            y = value.y;
        }


    }




    public ChunkData(Vector2Int pos) { position = pos; }
    public ChunkData(int _x, int _y) { x = _x; y = _y; }


    [System.NonSerialized]
    public Chunk chunk;


    [HideInInspector]
    public VoxelState[,,] map = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];




    public void Populate()
    {

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    Vector3 voxelGlobalPos = new Vector3(x + position.x, y, z + position.y);

                    map[x, y, z] = new VoxelState(World.Instance.GetVoxel(voxelGlobalPos), this, new Vector3Int(x,y,z);
                    //Debug.Log($"Position: {voxelMap[x,y,z]}, Noise Value: {noiseValue}, Voxel Value: {voxelValue}");

                    for(int p = 0; p<6; p++)
                    {
                        Vector3Int neighbourV3 = new Vector3Int(x, y, z) + VoxelData.faceChecks[p];
                        if (IsVoxelInChunk(neighbourV3))
                            map[x, y, z].neighbours[p] = VoxelFromV3Int(neighbourV3);
                        else
                            map[x, y, z].neighbours[p] = World.Instance.worldData.GetVoxel(voxelGlobalPos + VoxelData.faceChecks[p]);


                    }


                }
            }
        }

        Lighting.RecalculateNaturalLight(this);

        World.Instance.worldData.AddToModifiedChunkList(this);


    }


    public bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= VoxelData.ChunkWidth || y < 0 || y >= VoxelData.ChunkHeight || z < 0 || z >= VoxelData.ChunkWidth)
            return false;
        else
            return true;

    }

    public bool IsVoxelInChunk(Vector3Int pos)
    {
        return IsVoxelInChunk(pos.x, pos.y, pos.z);
    }

    public VoxelState VoxelFromV3Int(Vector3Int pos)
    {
        return map[pos.x, pos.y, pos.z];
    }

}
