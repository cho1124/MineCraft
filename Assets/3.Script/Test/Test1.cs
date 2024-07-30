using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    /*
    �÷��̾� ����(?)�� hoe �� �޷��ִ� ��ũ��Ʈ�Դϴ�.

    */
  
    public GameObject beafPrefab; // beaf �������� �ν����Ϳ��� ����
  
    private Collider currentCollider;

    private void OnCollisionEnter(Collision collision)
    {
        // �浹 �̺�Ʈ �α� ���
        Debug.Log("�÷��̾ �а��ֽ��ϴ� : " + collision.gameObject.name);
        AudioManager.instance.PlayRandomSFX("Humanoid", "Attack1"); //Ÿ����
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
