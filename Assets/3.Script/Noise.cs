using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {

    public static float Get2DPerlin (Vector2 position, float offset, float scale)
    {
        return Mathf.PerlinNoise((position.x + 0.1f) / VoxelData.viewDistanceInChunks * scale + offset, (position.y + 0.1f) / VoxelData.viewDistanceInChunks * scale + offset);
    }


}
