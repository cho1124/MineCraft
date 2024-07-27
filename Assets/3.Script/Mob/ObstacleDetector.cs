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

    private void Start()
    {
        monsterScript = GetComponentInParent<Monster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) || other.CompareTag(animalTag))
        {
            if (other.CompareTag(playerTag) && Time.time > lastPlayerDetectionTime + detectionCooldown) {
                lastPlayerDetectionTime = Time.time; // 플레이어 감지 시간을 업데이트
                monsterScript.SetTargetAndRun(other.transform.position, true);
            }
            else if (other.CompareTag(animalTag) && Time.time > lastAnimalDetectionTime + detectionCooldown) {
                lastAnimalDetectionTime = Time.time; // 동물 감지 시간을 업데이트
                monsterScript.SetTargetAndRun(other.transform.position, false);
            }
        }
        else
        {
            if (monsterScript != null) {
                monsterScript.JumpAndChangeState();
            }
        }
    }
}
