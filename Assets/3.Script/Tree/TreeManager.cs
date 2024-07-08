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
  //      // 트렁크 생성
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // 잎 생성
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
  //      // 트렁크 생성
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // 잎 생성 (더 큰 나무)
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
  //      // 트렁크 생성
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // 잎 생성 (작고 뭉친 나무)
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
  //      // 트렁크 생성
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // 잎 생성 (높고 층층이 있는 나무)
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
  //  void CreateType5Tree(Vector3 position) //잎으로만 이루어진 피라미드 형식의 낮은 나무
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int numLeaves = Random.Range(3, 6);
  //      int baseWidth = 3; // Base width of the leaves, 빈 공간이 생기지 않도록 더 넓게 설정
  // 
  //      // 잎 생성 (작고 뭉친 나무)
  //      for (int i = 0; i < numLeaves; i++)
  //      {
  //          int layerWidth = baseWidth - i; // Width of the current layer, 좀 더 넓게 설정
  //          int scale = 1; // 모든 큐브의 스케일을 1로 설정
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
  //  void CreateType6Tree(Vector3 position) //제일 큰 나무
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int trunkHeight = Random.Range(8, 12);
  //      int numLeavesLayers1 = Random.Range(7, 10);
  //      int numLeavesLayers2 = Random.Range(4, 7);
  // 
  //      // 트렁크 생성
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // 아래층 잎 생성 ( 제일 넓은 , 거대한 나무 잎 아래층)
  //      for (int i = 0; i < numLeavesLayers1; i++)
  //      {
  //          int layerWidth = numLeavesLayers1 - i; // 레이어의 넓이를 증가시킴
  //          for (int x = -layerWidth; x <= layerWidth; x++)
  //          {
  //              for (int z = -layerWidth; z <= layerWidth; z++)
  //              {
  //                  if (Mathf.Abs(x) + Mathf.Abs(z) <= layerWidth) // 다이아몬드 형태로 생성
  //                  {
  //                      Vector3 leafPosition = position + new Vector3(x, trunkHeight + i, z);
  //                      GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //                      leaf.transform.parent = tree.transform;
  //                  }
  //              }
  //          }
  //      }
  // 
  //      // 위층 잎 생성 ( 제일 넓은 , 거대한 나무 잎의 위층)
  //      for (int i = 0; i < numLeavesLayers2; i++)
  //      {
  //          int layerWidth = numLeavesLayers2 - i; // 레이어의 넓이를 증가시킴
  //          for (int x = -layerWidth; x <= layerWidth; x++)
  //          {
  //              for (int z = -layerWidth; z <= layerWidth; z++)
  //              {
  //                  if (Mathf.Abs(x) + Mathf.Abs(z) <= layerWidth) // 다이아몬드 형태로 생성
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
  //  void CreateType7Tree(Vector3 position) //두 층으로 이루어진 얇고 긴 나무 
  //  {
  //      GameObject tree = new GameObject("Tree");
  // 
  //      int trunkHeight = Random.Range(8, 12);
  //      int numLeavesLayers1 = Random.Range(7, 10);
  //      int numLeavesLayers2 = Random.Range(4, 7);
  // 
  //      // 트렁크 생성
  //      for (int i = 0; i < trunkHeight; i++)
  //      {
  //          Vector3 trunkPosition = position + new Vector3(0, i, 0);
  //          GameObject trunk = Instantiate(trunkPrefab, trunkPosition, Quaternion.identity);
  //          trunk.transform.parent = tree.transform;
  //      }
  // 
  //      // 첫 번째 잎 레이어 생성 (넓게 퍼지는 큰 잎)
  //      for (int i = 0; i < numLeavesLayers1; i++)
  //      {
  //          int layerWidth = numLeavesLayers1 - i; // 레이어의 넓이를 증가시킴
  //          for (int x = -layerWidth; x <= layerWidth; x++)
  //          {
  //              for (int z = -layerWidth; z <= layerWidth; z++)
  //              {
  //                  if (Mathf.Abs(x) + Mathf.Abs(z) <= layerWidth) // 다이아몬드 형태로 생성
  //                  {
  //                      Vector3 leafPosition = position + new Vector3(x, trunkHeight + i, z);
  //                      GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.identity);
  //                      leaf.transform.parent = tree.transform;
  //                  }
  //              }
  //          }
  //      }
  // 
  //      // 두 번째 잎 레이어 생성 (좁게 퍼지는 작은 잎)
  //      for (int i = 0; i < numLeavesLayers2; i++)
  //      {
  //          int layerWidth = numLeavesLayers2 - i; // 레이어의 넓이를 증가시킴
  //          for (int x = -layerWidth; x <= layerWidth; x++)
  //          {
  //              for (int z = -layerWidth; z <= layerWidth; z++)
  //              {
  //                  if (Mathf.Abs(x) + Mathf.Abs(z) <= layerWidth) // 다이아몬드 형태로 생성
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

