using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Voxel Mesh Data", menuName = "Minecraft/Voxel Mesh Data")]
public class VoxelMeshData : ScriptableObject
{
    public string blockName;
    public FaceMeshData[] faces;

}

[System.Serializable]
public class VertData
{
    public Vector3 position;
    public Vector2 uv;

    public VertData (Vector3 pos, Vector2 _uv)
    {
        position = pos;
        uv = _uv;
    }

    public Vector3 GetRotatedPosition (Vector3 angles)
    {
        Vector3 center = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 direction = position - center;
        direction = Quaternion.Euler(angles) * direction;
        return direction + center;
    }
}

[System.Serializable]
public class FaceMeshData
{
    public string direction;
    //public Vector3 normal;
    public VertData[] vertData;
    public int[] triangles;

    public VertData GetVertData(int index)
    {
        return vertData[index];
    }


}

