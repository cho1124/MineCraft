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

    private void OnCollisionEnter(Collision collision)
    {
        // �浹 �̺�Ʈ �α� ���
        Debug.Log("�÷��̾ �а��ֽ��ϴ� : " + collision.gameObject.name);

        // Monster �Ǵ� Animals �±׸� ���� ������Ʈ���� Ȯ��
        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Animals"))
        {
            // Entity ������Ʈ ��������
            Entity entity = collision.gameObject.GetComponent<Entity>();
            if (entity != null)
            {
                // HP�� 100 ���ҽ�Ű��
                Debug.Log("�÷��̾� ����� ���ݴ���");
                entity.TakeDamage(30);
                Debug.Log($"{entity.name} �� ����");
            }
            
       }
    }
}
