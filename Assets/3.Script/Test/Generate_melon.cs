using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//animals tag ���� ������Ʈ�� �� ��ũ��Ʈ�� ���� ���� 10�� �浹�ϸ� melon ����

public class Generate_melon : MonoBehaviour
{
    public GameObject melonPrefab; // Melon �������� �ν����Ϳ��� �Ҵ�
    public Vector3 spawnOffset; // Melon ������ ���� ��ġ�� ������
    private Dictionary<Vector3, int> collisionCount = new Dictionary<Vector3, int>();
    // �浹 Ƚ���� ������ ��ųʸ�

    void OnCollisionEnter(Collision collision)
    {
    if(collision.gameObject.CompareTag("Animals"))
        {
            Vector3 collisionPoint = collision.contacts[0].point;

            //�浹 ������ �������� �浹 Ƚ���� ������Ŵ
            if(collisionCount.ContainsKey(collisionPoint))
            {
                collisionCount[collisionPoint]++;
            }
            else
            {
                collisionCount[collisionPoint] = 1;
            }

            //�浹 Ƚ���� 10ȸ �̻��� ��� Melon �������� ����
            if(collisionCount[collisionPoint]>=10)
            {
                Instantiate(melonPrefab, collisionPoint + spawnOffset, Quaternion.identity);
                collisionCount[collisionPoint] = 0;//�浹 Ƚ�� �ʱ�ȭ
                Debug.Log("����� �����Ǿ����ϴ�! ");

            }
        }
    }
}
