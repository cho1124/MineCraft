using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    public string playerTag = "Player";
    public string animalTag = "Animals";
    public float chaseDuration = 7f;
    public float detectionCooldown = 30f; // ���� �� ��Ÿ�� �ð�
    private Monster monsterScript;
    private float lastPlayerDetectionTime = -Mathf.Infinity; // ������ �÷��̾� ���� �ð�
    private float lastAnimalDetectionTime = -Mathf.Infinity; // ������ ���� ���� �ð�

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
                lastPlayerDetectionTime = Time.time; // �÷��̾� ���� �ð��� ������Ʈ
                StartCoroutine(ChaseTarget(other.transform, true));
            }
            else if (other.CompareTag(animalTag) && Time.time > lastAnimalDetectionTime + detectionCooldown)
            {
                lastAnimalDetectionTime = Time.time; // ���� ���� �ð��� ������Ʈ
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

                    // �÷��̾ �����Ǹ� �� ��ġ�� Ÿ������ ����
                    if (hit.transform.CompareTag(playerTag))
                    {
                        monsterScript.SetTarget(target.position);
                        yield return null;
                    }
                    // ������ �����ǰ� ���� �÷��̾ ���� ������ ������ �� ��ġ�� Ÿ������ ����
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
