using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    /*
    (���� ������� �ʱ� ���� �ָ� ����)
    ¥ġ�� ��� �׽�Ʈ��
  
    1. grass �±� ���� ť�� �ǰ��� melonseed ����
    2. ground �±� ���� ť�� �ǰ��� ���Ѱ������� �ٲ�
    3. ���Ѱ������� �ٲ� ť�긦 ������ "��Ḧ �־����ϴ�" �α� �߸鼭 ���� �������� �ٲ�
    4. ���� �������� �ٲ� ť��� 60f�� �ƹ��� ��ȭ�� ������ �ٽ� ���� �������� �ٲ�
    (�ֱ������� �÷��̾ ��� �����־����)
    5. 300f���� ���� ���� ť�갡 �����Ǹ� ���� ���� ť�갡 ������� �ٲ�! 
  
    [���seed �̿��� �����]
    */
  
    public GameObject beafPrefab; // beaf �������� �ν����Ϳ��� ����
  
    private Collider currentCollider;
    private Dictionary<GameObject, Coroutine> fertilizedGrounds = new Dictionary<GameObject, Coroutine>();
  
    private void OnTriggerEnter(Collider other)
    {
        // �浹 �̺�Ʈ �α� ���
        Debug.Log("�÷��̾ �а��ֽ��ϴ� : " + other.gameObject.name);
  
        // ���� �浹 ����
        currentCollider = other;
    }
  
    private void Update() {
        // ���콺 ���� Ŭ�� Ȯ��
        if (Input.GetMouseButtonDown(0) && currentCollider != null) {
            // Monster �Ǵ� Animals �±׸� ���� ������Ʈ���� Ȯ��
            if (currentCollider.gameObject.CompareTag("Monster") || currentCollider.gameObject.CompareTag("Animals")) {
                // Health ������Ʈ ��������
                Entity entity = currentCollider.GetComponent<Entity>();
                if (entity != null) {
                    // HP�� 100 ���ҽ�Ű��
                    Debug.Log("�÷��̾� ����� ���ݴ���");
                    entity.TakeDamage(100);
                    Debug.Log($"{entity.name} �� ����");

                }
            }
            currentCollider = null;
        }
    }
}
