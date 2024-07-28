using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PopObject : MonoBehaviour
{

    // �ù� �ٽô� ���� ���Ѵ�
    // ���� ��ϵ� ���� �� �߰��ϰ�... �ﰢ�� �� �߰��ϰ�...�޽� �߰��ϰ�... ���� ����
    [SerializeField]
    private float setScale = 0.25f;
    [SerializeField]
    private float tempRotateY;
    [SerializeField]
    private float tempPositionY = 1;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private int vertexIndex = 0;

    [SerializeField]
    public GameObject worldObject;
    private World world;

    private Vector3 initialPosition;

    public void Initialize(World world, Vector3 position, byte blockID)
    {
        this.world = world;

        transform.localScale = new Vector3(setScale, setScale, setScale);
        initialPosition = position;
        transform.position = position;

        // Initialize MeshFilter and MeshRenderer here
        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        transform.GetComponent<ItemComponent>();
        


        ApplyBlockMeshAndTexture(blockID);

        JumpAnimation();
    }

    private void ApplyBlockMeshAndTexture(byte blockID)
    {
        //Debug.Log("Applying mesh and texture for block ID: " + blockID);

        // �ܵ�� Ķ�� ������ ����
        if (blockID == 3)
            blockID = 5;

        BlockType blockType = world.blockTypes[blockID];

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int p = 0; p < 6; p++)
        {
            for (int i = 0; i < 4; i++)
            {
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, i]]);
            }

            AddTexture(uvs, blockType.GetTextureID(p));

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 3);

            vertexIndex += 4;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshRenderer.material = world.material;

        //Debug.Log("Mesh and texture applied successfully.");
    }

    private void AddTexture(List<Vector2> uvs, int textureID)
    {
        float y = textureID / VoxelData.textureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);

        x *= VoxelData.normalizedBlockTextureSize;
        y *= VoxelData.normalizedBlockTextureSize;

        y = 1f - y - VoxelData.normalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.normalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y + VoxelData.normalizedBlockTextureSize));
    }

    // ��� ĺ���� �ز��� ����� Ƣ�� �����°� �ϰ�;��µ�
    // dotween�� ���鳢�� ����ī��Ʈ �����ϴ��� ó�� Ķ���� Ƣ������� �������� �ȿ���;;
    private void JumpAnimation()
    {
        transform.DOKill();
        transform.DOJump(initialPosition + new Vector3(0.5f, 0.5f, 0.5f), 1f, 1, 1f)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() =>
                 {
                     initialPosition = transform.position;
                 });
    }

    private void Update()
    {
        if (tempRotateY >= 359f)
            tempRotateY = 1f;
        tempRotateY += 1f;

        transform.Rotate(new Vector3(0, tempRotateY * Time.deltaTime, 0));

        tempPositionY += Time.deltaTime;
        float newYPosition = Mathf.Sin(tempPositionY) * 0.2f + 0.5f;
        transform.position = new Vector3(initialPosition.x, initialPosition.y + newYPosition, initialPosition.z);
    }
}
