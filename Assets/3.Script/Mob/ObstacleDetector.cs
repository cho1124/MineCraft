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
    private float lastDetectionTime = -Mathf.Infinity; // ������ ���� �ð�

    private void Start()
    {
        monsterScript = GetComponentInParent<Monster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time > lastDetectionTime + detectionCooldown) // ��Ÿ�� üũ
        {
            if (other.CompareTag(playerTag) || other.CompareTag(animalTag)) {
                if (monsterScript != null) {
                    monsterScript.SetTargetAndRun(other.transform.position);
                    lastDetectionTime = Time.time; // ���� �ð� ������Ʈ
                }
            }
        }
    }
}
