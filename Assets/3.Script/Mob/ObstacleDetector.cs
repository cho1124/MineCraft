using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    //������ �÷��̾ ���� �±׸� ���� ������Ʈ�� �����Ǹ� 5�ʰ� �� ��������
    // raycast�� ��ٰ� ���� ���·� ���ư�
    public string playerTag = "Player";
    public string animalTag = "Animals";
    public float chaseDuration = 7f;
    public float detectionCooldown = 30f; // ���� �� ��Ÿ�� �ð�
    public float maxChaseDistance = 10f; // �ִ� ���� �Ÿ�
    private Monster monsterScript;
    private float lastPlayerDetectionTime = -Mathf.Infinity; // ������ �÷��̾� ���� �ð�
    private float lastAnimalDetectionTime = -Mathf.Infinity; // ������ ���� ���� �ð�
    private Coroutine chaseCoroutine;

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
                if (chaseCoroutine != null)
                {
                    StopCoroutine(chaseCoroutine);
                }
                chaseCoroutine = StartCoroutine(ChaseTarget(other.transform, true));
            }
            else if (other.CompareTag(animalTag) && Time.time > lastAnimalDetectionTime + detectionCooldown)
            {
                lastAnimalDetectionTime = Time.time; // ���� ���� �ð��� ������Ʈ
                if (chaseCoroutine != null)
                {
                    StopCoroutine(chaseCoroutine);
                }
                chaseCoroutine = StartCoroutine(ChaseTarget(other.transform, false));
            }
        }
        else
        {
            // �÷��̾ ������ �ƴ� ��� ����
            monsterScript.JumpAndChangeState();
        }
    }

    private IEnumerator ChaseTarget(Transform target, bool isPlayer)
    {
        float startTime = Time.time;
        float raycastEndTime = startTime + 5f; // 5�� �� Raycast�� ���߱� ���� Ÿ�̸�
        monsterScript.IsChasingPlayer = isPlayer;

        while (Time.time < startTime + chaseDuration)
        {
            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (distanceToTarget > maxChaseDistance)
                {
                    // ��ǥ�� ���� ������ ����� �� ������ ����
                    break;
                }

                if (Time.time < raycastEndTime)
                {
                    RaycastHit hit;
                    Vector3 direction = (target.position - transform.position).normalized;

                    if (Physics.Raycast(transform.position, direction, out hit))
                    {
                       // Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

                        if (hit.transform.CompareTag(playerTag) || (hit.transform.CompareTag(animalTag) && !monsterScript.IsChasingPlayer))
                        {
                            monsterScript.SetTarget(target.position);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("�߰��� �׸��Ӵϴ�. ");
        monsterScript.EndChaseAndWander();
    }
}
