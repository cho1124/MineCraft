using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PopObject : MonoBehaviour
{

    // 시발 다시는 복셀 안한다
    // 단일 블록도 정점 다 추가하고... 삼각형 다 추가하고...메쉬 추가하고... 에휴 시팔
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

        // 잔디면 캘때 흙으로 나옴
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

    // 블록 캤을때 쬐끄만 블록이 튀어 오르는걸 하고싶었는데
    // dotween이 지들끼리 점프카운트 공유하는지 처음 캘때만 튀어오르고 나머지는 안오름;;
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
