using System.Collections.Generic;
using UnityEngine;

public class PerlinWorm
{
    public int maxWormLength = 80;
    public float resolution = 1.0f;
    public int seed = 123;
    public int wormWidth = 2; // 지렁이의 너비를 정의합니다.

    public List<Vector3Int> CreateWorm(int cx, int cz, int startY)
    {
        float sx = Random.Range(0, VoxelData.ChunkWidth);
        float sy = startY;
        float sz = Random.Range(0, VoxelData.ChunkWidth);
        int ammount = 10;

        List<Vector3Int> wormPositions = new List<Vector3Int>();

        for (int ci = 1; ci <= ammount; ci++)
        {
            Vector3 startPosition = ConvertChGridToReal(cx, cz, sx, sy, sz, true);
            Transform wormTransform = new GameObject().transform;
            wormTransform.position = startPosition;
            int maxlength = Random.Range(100, maxWormLength);

            for (int i = 1; i <= maxlength; i++)
            {
                float x = Mathf.PerlinNoise((wormTransform.position.x / resolution) + 0.1f, seed + ci) + 0.01f;
                float y = Mathf.PerlinNoise((wormTransform.position.y / resolution) + 0.1f, seed + ci) + 0.01f;
                float z = Mathf.PerlinNoise((wormTransform.position.z / resolution) + 0.1f, seed + ci) + 0.01f;
                wormTransform.rotation *= Quaternion.Euler(x * (Random.Range(-48, 45) + ci), y * (Random.Range(-48, 45) + ci), z * (Random.Range(-48, 45) + ci));
                wormTransform.position += wormTransform.forward * -resolution;

                Vector3Int roundedPos = Vector3Int.FloorToInt(wormTransform.position);
                if (IsInWorldBounds(roundedPos))
                {
                    AddWormWidthPositions(roundedPos, wormPositions);
                }
            }
        }

        return wormPositions;
    }

    private void AddWormWidthPositions(Vector3Int centerPos, List<Vector3Int> wormPositions)
    {
        for (int x = -wormWidth; x <= wormWidth; x++)
        {
            for (int y = -wormWidth; y <= wormWidth; y++)
            {
                for (int z = -wormWidth; z <= wormWidth; z++)
                {
                    Vector3Int pos = centerPos + new Vector3Int(x, y, z);
                    if (IsInWorldBounds(pos) && !wormPositions.Contains(pos))
                    {
                        wormPositions.Add(pos);
                    }
                }
            }
        }
    }

    public static Vector3 ConvertChGridToReal(int cx, int cz, float x, float y, float z, bool toBlockInstead)
    {
        float scale = toBlockInstead ? 1 : 4;
        return new Vector3((x + 8 * cx), y, (z + 8 * cz)) * scale;
    }

    private bool IsInWorldBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < VoxelData.worldSizeInBlocks &&
               pos.y >= 0 && pos.y < VoxelData.ChunkHeight &&
               pos.z >= 0 && pos.z < VoxelData.worldSizeInBlocks;
    }
}
