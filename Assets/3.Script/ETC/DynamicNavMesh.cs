using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMesh : MonoBehaviour //������ �ʹ� ���� �ɷ��� ������Ʈ �̵� �̺�Ʈ�� �����ÿ��� ������Ʈ �ϵ��� ����
{
  //  public NavMeshSurface navMeshSurface;
  //  public float updateInterval = 10f; // NavMesh ������Ʈ ���� (�� ����)
  //  private float lastUpdateTime;
  //  private bool needsUpdate = false;
  //
  //  void Start() {
  //      lastUpdateTime = Time.time;
  //      UpdateNavMesh();
  //  }
  //
  //  void Update() {
  //      if (needsUpdate && Time.time - lastUpdateTime >= updateInterval) {
  //          UpdateNavMesh();
  //          lastUpdateTime = Time.time;
  //          needsUpdate = false;
  //      }
  //  }
  //
  //  public void UpdateNavMesh() {
  //      navMeshSurface.BuildNavMesh();
  //  }
  //
  //  // ������Ʈ�� ��ȭ�� ���� �� �� �޼��带 ȣ���մϴ�.
  //  public void OnObjectChanged() {
  //      needsUpdate = true;
  //  }
}
