using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMesh : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;
    public float updateInterval = 2f; // NavMesh 업데이트 간격 (초 단위)
    private float lastUpdateTime;

    void Start() {
        lastUpdateTime = Time.time;
        UpdateNavMesh();
    }

    void Update() {
        if (Time.time - lastUpdateTime >= updateInterval) {
            UpdateNavMesh();
            lastUpdateTime = Time.time;
        }
    }

    public void UpdateNavMesh() {
        navMeshSurface.BuildNavMesh();
    }
}
