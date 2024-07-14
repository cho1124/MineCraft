using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMesh : MonoBehaviour //오류가 너무 많이 걸려서 오브젝트 이동 이벤트가 있을시에만 업데이트 하도록 수정
{
  //  public NavMeshSurface navMeshSurface;
  //  public float updateInterval = 10f; // NavMesh 업데이트 간격 (초 단위)
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
  //  // 오브젝트에 변화가 있을 때 이 메서드를 호출합니다.
  //  public void OnObjectChanged() {
  //      needsUpdate = true;
  //  }
}
