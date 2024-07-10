using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public GameObject TrunkPrefab; // ��� ������
    public GameObject LeafPrefab;  // �� ������
    public int trunkHeight = 5;    // ����� ����
    public Vector3Int leafSize = new Vector3Int(5, 1, 5); // ���� ũ�� (������ ��ǥ)

    void Start()
    {
        GenerateTree();
    }

    void GenerateTree()
    {
        // Ʈ��ũ ����
        for (int i = 0; i < trunkHeight; i++)
        {
            Vector3Int position = new Vector3Int(0, i, 0);
            Instantiate(TrunkPrefab, position, Quaternion.identity, transform);
        }

        // �� ����
        Vector3Int leafStartPosition = new Vector3Int(-(leafSize.x / 2), trunkHeight, -(leafSize.z / 2));
        for (int x = 0; x < leafSize.x; x++)
        {
            for (int z = 0; z < leafSize.z; z++)
            {
                Vector3Int position = new Vector3Int(leafStartPosition.x + x, leafStartPosition.y, leafStartPosition.z + z);
                Instantiate(LeafPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}
