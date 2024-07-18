using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Lighting
{

    public static void RecalculateNaturalLight(ChunkData chunkData)
    { 
        for(int x = 0; x< VoxelData.ChunkWidth; x++)
        {

            for (int z = 0; z < VoxelData.ChunkWidth; z++)
            {

                CastNaturalLight(chunkData, x, z, VoxelData.ChunkHeight - 1);

            }

        }

    }



    public static void CastNaturalLight(ChunkData chunkData, int x, int z,int startY)
    {
        if(startY > VoxelData.ChunkHeight - 1)
        {
            startY = VoxelData.ChunkHeight - 1;
            Debug.LogWarning("Attempted to cast natural light from above world.");
        }



        bool obstructed = false;

        for(int y = startY; y>-1; y--)
        {
            VoxelState voxel = chunkData.map[x, y, z];
            

            if(obstructed)
            {
                voxel.light = 0;
            }
            else if(voxel.properties.opacity>0)
            {
                voxel.light = 0;
                obstructed = true;

            }else
            {
                voxel.light = 15;
            }
        }




    }


}
