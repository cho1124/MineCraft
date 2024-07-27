using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    //������ �÷��̾ ���� �±׸� ���� ������Ʈ�� �����Ǹ� 5�ʰ� �� ��������
    // raycast�� ��ٰ� ���� ���·� ���ư�
    public string playerTag = "Player";
    public string animalTag = "Animals";
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
            if (other.CompareTag(playerTag) && Time.time > lastPlayerDetectionTime + detectionCooldown) {
                lastPlayerDetectionTime = Time.time; // �÷��̾� ���� �ð��� ������Ʈ
                monsterScript.SetTargetAndRun(other.transform.position, true);
            }
            else if (other.CompareTag(animalTag) && Time.time > lastAnimalDetectionTime + detectionCooldown) {
                lastAnimalDetectionTime = Time.time; // ���� ���� �ð��� ������Ʈ
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
