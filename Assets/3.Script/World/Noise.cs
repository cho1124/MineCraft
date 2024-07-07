using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {

    public static float Get2DPerlin (Vector2 position, float offset, float scale)
    {
        return Mathf.PerlinNoise((position.x + 0.1f) / VoxelData.viewDistanceInChunks * scale + offset, (position.y + 0.1f) / VoxelData.viewDistanceInChunks * scale + offset);
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

        //float x = (position.x + offset + 0.1f) * scale;
        //float y = (position.y + offset + 0.1f) * scale;
        //float z = (position.x + offset + 0.1f) * scale;
        //x -= 0.1f;
        //y -= 0.1f;
        //z -= 0.1f;
        //
        //float xy = Mathf.PerlinNoise(x, y);
        //float yz = Mathf.PerlinNoise(y, z);
        //float zx = Mathf.PerlinNoise(x, z);
        //
        //float yx = Mathf.PerlinNoise(y, x);
        //float zy = Mathf.PerlinNoise(z, y);
        //float xz = Mathf.PerlinNoise(z, x);
        //
        //if ((xy + yz + zx + yx + zy + xz) / 6f * 2 -0.9 > threshold)
        //    return true;
        //else
        //    return false;
        //

        float noiseValue = Get3DPerlin(position, offset, scale);
        return noiseValue > threshold;
    }


}