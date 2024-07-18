using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    public string playerTag = "Player";
    public string animalTag = "Animals";
    public float chaseDuration = 7f;
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
    }

    private IEnumerator ChaseTarget(Transform target, bool isPlayer)
    {
        float startTime = Time.time;

        while (Time.time < startTime + chaseDuration)
        {
            if (target != null)
            {
                RaycastHit hit;
                Vector3 direction = (target.position - transform.position).normalized;

                if (Physics.Raycast(transform.position, direction, out hit))
                {
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

                    // 플레이어가 감지되면 그 위치를 타겟으로 설정
                    if (hit.transform.CompareTag(playerTag))
                    {
                        monsterScript.SetTarget(target.position);
                        yield return null;
                    }
                    // 동물이 감지되고 현재 플레이어를 추적 중이지 않으면 그 위치를 타겟으로 설정
                    else if (hit.transform.CompareTag(animalTag) && !monsterScript.IsChasingPlayer)
                    {
                        monsterScript.SetTarget(target.position);
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
        monsterScript.EndChaseAndWander();
    }
}
