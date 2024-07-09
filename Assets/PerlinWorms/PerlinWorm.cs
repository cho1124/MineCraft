using System.Collections.Generic;
using UnityEngine;

public class PerlinWorm : MonoBehaviour
{
    public int maxWormLength = 80;
    public float resolution = 1.0f;
    public int seed = 123;

    private void Start()
    {
        List<Dictionary<string, bool>> worms = CreateWorm(0, 0);
        foreach (var worm in worms)
        {
            foreach (var kvp in worm)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value}");
            }
        }
    }

    public static Vector3 RoundPos(Vector3 v3)
    {
        return new Vector3(Mathf.Floor(v3.x + 0.5f), Mathf.Floor(v3.y + 0.5f), Mathf.Floor(v3.z + 0.5f));
    }

    public static Vector3 ConvertChGridToReal(int cx, int cz, float x, float y, float z, bool toBlockInstead)
    {
        float scale = toBlockInstead ? 1 : 4;
        return new Vector3((x + 8 * cx), y, (z + 8 * cz)) * scale;
    }

    public List<Dictionary<string, bool>> CreateWorm(int cx, int cz)
    {
        float sx = Random.Range(0, 8); // Random.Range(0, 8)
        float sy = Random.Range(0, 128);
        float sz = Random.Range(0, 8); // Random.Range(0, 8)
        int ammount = 10;//Random.Range(0, 10);  // Random.Range(0, 4)

        List<Dictionary<string, bool>> worms = new List<Dictionary<string, bool>>();
        Debug.Log($"Branches: {ammount}");

        for (int ci = 1; ci <= ammount; ci++)
        {
            Color color = Random.ColorHSV();
            Dictionary<string, bool> wormData = new Dictionary<string, bool>();
            Vector3 startPosition = ConvertChGridToReal(cx, cz, sx, sy, sz, true);
            Transform wormTransform = new GameObject().transform;
            wormTransform.position = startPosition;
            int maxlength = Random.Range(100, maxWormLength);

            for (int i = 1; i <= maxlength; i++)
            {
                float x = Mathf.PerlinNoise((wormTransform.position.x / resolution) + 0.1f, seed + ci) + 0.01f;
                float y = Mathf.PerlinNoise((wormTransform.position.y / resolution) + 0.1f, seed + ci) + 0.01f;
                float z = Mathf.PerlinNoise((wormTransform.position.z / resolution) + 0.1f, seed + ci) + 0.01f;
                wormTransform.rotation *= Quaternion.Euler(x * (Random.Range(-48,45) + ci), y * (Random.Range(-48, 45) + ci), z * (Random.Range(-48, 45) + ci));
                //wormTransform.rotation *= Quaternion.Euler(x * (1 + ci), y * (1 + ci), z * (1 + ci));
                wormTransform.position += wormTransform.forward * -resolution;

                Vector3 roundedPos = RoundPos(wormTransform.position);
                string posKey = roundedPos.ToString();
                wormData[posKey] = false;

                GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
                part.transform.position = roundedPos;
                part.transform.localScale = new Vector3(1, 1, 1);
                part.GetComponent<Renderer>().material.color = color;
                part.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                if (i == 1)
                {
                    part.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
                }
                part.transform.parent = transform;
            }

            worms.Add(wormData);
        }

        return worms;
    }
}
