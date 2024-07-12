using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VerticesData
{
    public PositionData positions;
    public UVData uvs;
    public NormalData normals;
    public VIndexData vindices;

    [System.Serializable]
    public class PositionData
    {
        public int stride;
        public int count;
        public float[] array;
    }

    [System.Serializable]
    public class UVData
    {
        public int stride;
        public int count;
        public float[] array;
    }

    [System.Serializable]
    public class NormalData
    {
        public int stride;
        public int count;
        public float[] array;
    }

    [System.Serializable]
    public class VIndexData
    {
        public int stride;
        public int count;
        public int[] array;
    }
}

public class ModelLoader : MonoBehaviour
{
    public string fileName = "entity/biped"; // JSON 파일명

    void Start()
    {
        // JSON 파일 읽기
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile == null)
        {
            Debug.LogError("Failed to load JSON file!");
            return;
        }

        // JSON 데이터 파싱
        VerticesData data = JsonUtility.FromJson<VerticesData>(jsonFile.text);
        if (data == null)
        {
            Debug.LogError("Failed to parse JSON data!");
            return;
        }

        // 메쉬 생성
        CreateMesh(data);
    }

    void CreateMesh(VerticesData data)
    {
        Mesh mesh = new Mesh();

        // 포지션 설정
        Vector3[] vertices = new Vector3[data.positions.count];
        for (int i = 0; i < data.positions.count; i++)
        {
            vertices[i] = new Vector3(
                data.positions.array[i * 3],
                data.positions.array[i * 3 + 1],
                data.positions.array[i * 3 + 2]
            );
        }
        mesh.vertices = vertices;

        // 텍스처 좌표 설정
        Vector2[] uvs = new Vector2[data.uvs.count];
        for (int i = 0; i < data.uvs.count; i++)
        {
            uvs[i] = new Vector2(
                data.uvs.array[i * 2],
                data.uvs.array[i * 2 + 1]
            );
        }
        mesh.uv = uvs;

        // 법선 벡터 설정
        Vector3[] normals = new Vector3[data.normals.count];
        for (int i = 0; i < data.normals.count; i++)
        {
            normals[i] = new Vector3(
                data.normals.array[i * 3],
                data.normals.array[i * 3 + 1],
                data.normals.array[i * 3 + 2]
            );
        }
        mesh.normals = normals;

        // 인덱스 설정
        int[] indices = new int[data.vindices.count];
        for (int i = 0; i < data.vindices.count; i++)
        {
            indices[i] = data.vindices.array[i];
        }
        mesh.triangles = indices;

        // 메쉬를 가진 게임 오브젝트 생성
        GameObject obj = new GameObject("LoadedMesh");
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;

        // 기본 머티리얼 설정
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }
}
