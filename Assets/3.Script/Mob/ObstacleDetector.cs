using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    public string playerTag = "Player";
    public string animalTag = "Animals";
    public float chaseDuration = 7f;
    public float detectionCooldown = 30f; // 감지 후 쿨타임 시간
    public float maxChaseDistance = 10f; // 최대 추적 거리
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
            if (other.CompareTag(playerTag) && Time.time > lastPlayerDetectionTime + detectionCooldown)
            {
                lastPlayerDetectionTime = Time.time; // 플레이어 감지 시간을 업데이트
                StartCoroutine(ChaseTarget(other.transform, true));
            }
            else if (other.CompareTag(animalTag) && Time.time > lastAnimalDetectionTime + detectionCooldown)
            {
                lastAnimalDetectionTime = Time.time; // 동물 감지 시간을 업데이트
                StartCoroutine(ChaseTarget(other.transform, false));
            }
        }
        else
        {
            // 플레이어나 동물이 아닌 경우 점프
            monsterScript.JumpAndChangeState();
        }
    }

    private IEnumerator ChaseTarget(Transform target, bool isPlayer)
    {
        float startTime = Time.time;
        monsterScript.IsChasingPlayer = isPlayer;

        while (Time.time < startTime + chaseDuration)
        {
            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (distanceToTarget > maxChaseDistance)
                {
                    // 목표가 감지 범위를 벗어났을 때 추적을 멈춤
                    break;
                }

                RaycastHit hit;
                Vector3 direction = (target.position - transform.position).normalized;

                if (Physics.Raycast(transform.position, direction, out hit))
                {
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

                    if (hit.transform.CompareTag(playerTag) || (hit.transform.CompareTag(animalTag) && !monsterScript.IsChasingPlayer))
                    {
                        monsterScript.SetTarget(target.position);
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("추격을 그만둡니다. ");
        monsterScript.EndChaseAndWander();
    }
}
