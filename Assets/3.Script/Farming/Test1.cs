using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public GameObject melonSeedPrefab;
    public GameObject beafPrefab; // beaf �������� �ν����Ϳ��� ����
    public Material leafMaterial;

    private Collision currentCollision;

    private void OnCollisionEnter(Collision collision)
    {
        // �浹 �̺�Ʈ �α� ���
        Debug.Log("�÷��̾ �а��ֽ��ϴ�. : " + collision.gameObject.name);

        // ���� �浹 ����
        currentCollision = collision;
    }

        private void Update()
        {
            // ���콺 ���� Ŭ�� Ȯ��
            if (Input.GetMouseButtonDown(0) && currentCollision != null)
            {
                // �浹�� ������Ʈ�� ���͸����� ������
                Renderer renderer = currentCollision.gameObject.GetComponent<Renderer>();

                if (renderer != null && renderer.material == leafMaterial)
                {
                    // �浹�� ������Ʈ�� melonSeedPrefab���� ����
                    Vector3 position = currentCollision.transform.position;
                    Quaternion rotation = currentCollision.transform.rotation;

                    // ���� ������Ʈ�� ����
                    Destroy(currentCollision.gameObject);

                    // ���ο� ������ �ν��Ͻ� ����
                    Instantiate(melonSeedPrefab, position, rotation);
                }

                // �浹�� ������Ʈ�� "Animals" �±׸� ������ �ִ��� Ȯ��
                if (currentCollision.gameObject.CompareTag("Animals"))
                {
                    // �浹�� ������Ʈ�� ��ġ�� ȸ�� ���� ����
                    Vector3 position = currentCollision.transform.position;
                    Quaternion rotation = currentCollision.transform.rotation;

                    // ���� ���� ������Ʈ�� ����
                    Destroy(currentCollision.gameObject);

                    // ���ο� beaf ������ �ν��Ͻ� ����
                    Instantiate(beafPrefab, position, rotation);
                }

                // �浹 ���� �ʱ�ȭ
                currentCollision = null;
            }
        }
    }

