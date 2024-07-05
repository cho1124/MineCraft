using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int depth = 20;
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public GameObject cubePrefab;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        float[,] sibal = GenerateHeights();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float roundedHeight = Mathf.Round(sibal[i, j] * 2);
                Instantiate(cubePrefab, new Vector3((float)i + 0.5f, roundedHeight, (float)j + 0.5f),Quaternion.identity,transform);
            }
        }
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width,height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x,y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;
        return Mathf.PerlinNoise(xCoord, yCoord)*10f;
    }
}
