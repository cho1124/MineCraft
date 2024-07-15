using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public GameObject melonSeedPrefab;
    public GameObject beafPrefab; // beaf �������� �ν����Ϳ��� ����
    public Material leafMaterial;

    private Collider currentCollider;

    private void OnTriggerEnter(Collider other)
    {
        // �浹 �̺�Ʈ �α� ���
        Debug.Log("�÷��̾ �а��ֽ��ϴ� : " + other.gameObject.name);

        // ���� �浹 ����
        currentCollider = other;
    }

    private void Update()
    {
        // ���콺 ���� Ŭ�� Ȯ��
        if (Input.GetMouseButtonDown(0) && currentCollider != null)
        {
            // �浹�� ������Ʈ�� ���͸����� ������
            Renderer renderer = currentCollider.gameObject.GetComponent<Renderer>();

            if (renderer != null && renderer.material == leafMaterial)
            {
                // �浹�� ������Ʈ�� melonSeedPrefab���� ����
                Vector3 position = currentCollider.transform.position;
                Quaternion rotation = currentCollider.transform.rotation;

                // ���� ������Ʈ�� ����
                Destroy(currentCollider.gameObject);

                // ���ο� ������ �ν��Ͻ� ����
                Instantiate(melonSeedPrefab, position, rotation);
            }

            // �浹�� ������Ʈ�� "Animals" �±׸� ������ �ִ��� Ȯ��
            if (currentCollider.gameObject.CompareTag("Animals"))
            {
                // �浹�� ������Ʈ�� ��ġ�� ȸ�� ���� ����
                Vector3 position = currentCollider.transform.position;
                Quaternion rotation = currentCollider.transform.rotation;

                // ���� ���� ������Ʈ�� ����
                Destroy(currentCollider.gameObject);

                // ���ο� beaf ������ �ν��Ͻ� ����
                Instantiate(beafPrefab, position, rotation);
            }

            // �浹 ���� �ʱ�ȭ
            currentCollider = null;
        }
    }
}

