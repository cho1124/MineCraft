using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
  //  public GameObject trunkPrefab;
  //  public GameObject leafPrefab;
  //  public int initialTreeCount;
  //  public GameObject planeObject;
  //  //public Vector3 areaSize = new Vector3(100, 1, 100);
  // 
  //  private void Start()
  //  {
  //      MeshRenderer planeRenderer = planeObject.GetComponent<MeshRenderer>();
  //      Vector3 planeSize = planeRenderer.bounds.size;
  //      initialTreeCount = Random.Range(1, 100);
  //      for (int i = 0; i < initialTreeCount; i++)
  //      {
  //          Vector3 position = new Vector3(Random.Range(-planeSize.x / 2, planeSize.x / 2), 0, Random.Range(-planeSize.z / 2, planeSize.z / 2));
  //          CreateTree(position);
  //      }
  //  }
  // 
  //  void CreateTree(Vector3 position)
  //  {
  //      int treeType = Random.Range(0, 7);
  //      switch (treeType)
  //      {
  //          case 0:
  //              CreateType1Tree(position);
  //              break;
  //          case 1:
  //              CreateType2Tree(position);
  //              break;
  //          case 2:
  //              CreateType3Tree(position);
  //              break;
  //          case 3:
  //              CreateType4Tree(position);
  //              break;
  //          case 4:
  //              CreateType5Tree(position);
  //              break;
  //          case 5:
  //              CreateType6Tree(position);
  //              break;
  //          case 6:
  //              CreateType7Tree(position);
  //              break;
  //      }
  //  }
  // 
  //  void CreateType1Tree(Vector3 position)
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int trunkHeight = Random.Range(3, 6);
  //      float leafHeight = Random.Range(2, 3);
  // 
  //      // Ʈ��ũ ����
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // �� ����
  //      Vector3 leafPosition = position + new Vector3(0, trunkHeight, 0);
  //      GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //      leaf.transform.localScale = new Vector3(3, leafHeight, 3);
  //      leaf.transform.parent = tree.transform;
  //  }
  // 
  //  void CreateType2Tree(Vector3 position)
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int trunkHeight = Random.Range(6, 10);
  //      float leafHeight = Random.Range(4, 6);
  // 
  //      // Ʈ��ũ ����
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // �� ���� (�� ū ����)
  //      Vector3 leafPosition = position + new Vector3(0, trunkHeight, 0);
  //      GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //      leaf.transform.localScale = new Vector3(4, leafHeight, 4);
  //      leaf.transform.parent = tree.transform;
  //  }
  // 
  //  void CreateType3Tree(Vector3 position)
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int trunkHeight = Random.Range(2, 5);
  //      int numLeaves = Random.Range(3, 6);
  // 
  //      // Ʈ��ũ ����
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // �� ���� (�۰� ��ģ ����)
  //      for (int i = 0; i < numLeaves; i++)
  //      {
  //          Vector3 leafPosition = position + new Vector3(0, trunkHeight + i, 0);
  //          GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //          leaf.transform.localScale = new Vector3(2 - i * 0.3f, 1, 2 - i * 0.3f);
  //          leaf.transform.parent = tree.transform;
  //      }
  //  }
  // 
  //  void CreateType4Tree(Vector3 position)
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int trunkHeight = Random.Range(8, 12);
  //      int numLeavesLayers = Random.Range(4, 7);
  // 
  //      // Ʈ��ũ ����
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // �� ���� (���� ������ �ִ� ����)
  //      for (int i = 0; i < numLeavesLayers; i++)
  //      {
  //          float scale = Mathf.Lerp(4, 1, (float)i / numLeavesLayers);
  //          Vector3 leafPosition = position + new Vector3(0, trunkHeight + i, 0);
  //          GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //          leaf.transform.localScale = new Vector3(scale, 1, scale);
  //          leaf.transform.parent = tree.transform;
  //      }
  //  }
  // 
  //  void CreateType5Tree(Vector3 position) //�����θ� �̷���� �Ƕ�̵� ������ ���� ����
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int numLeaves = Random.Range(3, 6);
  //      int baseWidth = 3; // Base width of the leaves, �� ������ ������ �ʵ��� �� �а� ����
  // 
  //      // �� ���� (�۰� ��ģ ����)
  //      for (int i = 0; i < numLeaves; i++)
  //      {
  //          int layerWidth = baseWidth - i; // Width of the current layer, �� �� �а� ����
  //          int scale = 1; // ��� ť���� �������� 1�� ����
  // 
  //          for (int x = -layerWidth; x <= layerWidth; x++)
  //          {
  //              for (int z = -layerWidth; z <= layerWidth; z++)
  //              {
  //                  Vector3 leafPosition = position + new Vector3(x, i, z);
  //                  GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //                  leaf.transform.localScale = new Vector3(scale, 1, scale);
  //                  leaf.transform.parent = tree.transform;
  //              }
  //          }
  //      }
  //  }
  // 
  //  void CreateType6Tree(Vector3 position) //���� ū ����
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int trunkHeight = Random.Range(8, 12);
  //      int numLeavesLayers1 = Random.Range(7, 10);
  //      int numLeavesLayers2 = Random.Range(4, 7);
  // 
  //      // Ʈ��ũ ����
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // �Ʒ��� �� ���� ( ���� ���� , �Ŵ��� ���� �� �Ʒ���)
  //      for (int i = 0; i < numLeavesLayers1; i++)
  //      {
  //          int layerWidth = numLeavesLayers1 - i; // ���̾��� ���̸� ������Ŵ
  //          for (int x = -layerWidth; x <= layerWidth; x++)
  //          {
  //              for (int z = -layerWidth; z <= layerWidth; z++)
  //              {
  //                  if (Mathf.Abs(x) + Mathf.Abs(z) <= layerWidth) // ���̾Ƹ�� ���·� ����
  //                  {
  //                      Vector3 leafPosition = position + new Vector3(x, trunkHeight + i, z);
  //                      GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //                      leaf.transform.parent = tree.transform;
  //                  }
  //              }
  //          }
  //      }
  // 
  //      // ���� �� ���� ( ���� ���� , �Ŵ��� ���� ���� ����)
  //      for (int i = 0; i < numLeavesLayers2; i++)
  //      {
  //          int layerWidth = numLeavesLayers2 - i; // ���̾��� ���̸� ������Ŵ
  //          for (int x = -layerWidth; x <= layerWidth; x++)
  //          {
  //              for (int z = -layerWidth; z <= layerWidth; z++)
  //              {
  //                  if (Mathf.Abs(x) + Mathf.Abs(z) <= layerWidth) // ���̾Ƹ�� ���·� ����
  //                  {
  //                      Vector3 leafPosition = position + new Vector3(x, trunkHeight + numLeavesLayers1 + i, z);
  //                      GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //                      leaf.transform.parent = tree.transform;
  //                  }
  //              }
  //          }
  //      }
  //  }
  // 
  //  void CreateType7Tree(Vector3 position) //�� ������ �̷���� ��� �� ���� 
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int trunkHeight = Random.Range(8, 12);
  //      int numLeavesLayers1 = Random.Range(7, 10);
  //      int numLeavesLayers2 = Random.Range(4, 7);
  // 
  //      // Ʈ��ũ ����
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // ù ��° �� ���̾� ���� (�а� ������ ū ��)
  //      for (int i = 0; i < numLeavesLayers1; i++)
  //      {
  //          int layerWidth = numLeavesLayers1 - i; // ���̾��� ���̸� ������Ŵ
  //          for (int x = -layerWidth; x <= layerWidth; x++)
  //          {
  //              for (int z = -layerWidth; z <= layerWidth; z++)
  //              {
  //                  if (Mathf.Abs(x) + Mathf.Abs(z) <= layerWidth) // ���̾Ƹ�� ���·� ����
  //                  {
  //                      Vector3 leafPosition = position + new Vector3(x, trunkHeight + i, z);
  //                      GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //                      leaf.transform.parent = tree.transform;
  //                  }
  //              }
  //          }
  //      }
  // 
  //      // �� ��° �� ���̾� ���� (���� ������ ���� ��)
  //      for (int i = 0; i < numLeavesLayers2; i++)
  //      {
  //          int layerWidth = numLeavesLayers2 - i; // ���̾��� ���̸� ������Ŵ
  //          for (int x = -layerWidth; x <= layerWidth; x++)
  //          {
  //              for (int z = -layerWidth; z <= layerWidth; z++)
  //              {
  //                  if (Mathf.Abs(x) + Mathf.Abs(z) <= layerWidth) // ���̾Ƹ�� ���·� ����
  //                  {
  //                      Vector3 leafPosition = position + new Vector3(x, trunkHeight + numLeavesLayers1 + i, z);
  //                      GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //                      leaf.transform.parent = tree.transform;
  //                  }
  //              }
  //          }
  //      }
  //  }
  //
}

