using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {

    public static float Get2DPerlin (Vector2 position, float offset, float scale)
    {
        return Mathf.PerlinNoise((position.x + 0.1f) / VoxelData.viewDistanceInChunks * scale + offset, (position.y + 0.1f) / VoxelData.viewDistanceInChunks * scale + offset);
    }


    public static bool Get3DPerlin (Vector3 position, float offset, float scale, float threshold)
    {

        float x = (position.x + offset + 0.1f) * scale;
        float y = (position.y + offset + 0.1f) * scale;
        float z = (position.x + offset + 0.1f) * scale;

        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float ca = Mathf.PerlinNoise(z, x);
        float cb = Mathf.PerlinNoise(z, y);

        if ((ab + bc + ac + ba + cb + ca) / 6f > threshold)
            return true;
        else
            return false;

    }


}
