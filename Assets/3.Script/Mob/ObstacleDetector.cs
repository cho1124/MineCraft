using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    //공간에 플레이어나 동물 태그를 가진 오브젝트가 감지되면 5초간 그 방향으로
    // raycast를 쏘다가 이전 상태로 돌아감
    public string playerTag = "Player";
    public string animalTag = "Animals";
    public float detectionCooldown = 30f; // 감지 후 쿨타임 시간
    private Monster monsterScript;
    private float lastPlayerDetectionTime = -Mathf.Infinity; // 마지막 플레이어 감지 시간
    private float lastAnimalDetectionTime = -Mathf.Infinity; // 마지막 동물 감지 시간
    private float lastDetectionTime = -Mathf.Infinity; // 마지막 감지 시간

    private void Start()
    {
        monsterScript = GetComponentInParent<Monster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time > lastDetectionTime + detectionCooldown) // 쿨타임 체크
        {
            if (other.CompareTag(playerTag) || other.CompareTag(animalTag)) {
                if (monsterScript != null) {
                    monsterScript.SetTargetAndRun(other.transform.position);
                    lastDetectionTime = Time.time; // 감지 시간 업데이트
                }
            }
        }
    }
}
