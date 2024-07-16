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

    public GameObject melonSeedPrefab;
    public GameObject melonPrefab;
    public GameObject beafPrefab; // beaf �������� �ν����Ϳ��� ����
    public Material brownLightMaterial;
    public Material brownDarkMaterial;

    private Renderer renderer;
    private Collider currentCollider;
    private Dictionary<GameObject, Coroutine> fertilizedGrounds = new Dictionary<GameObject, Coroutine>();

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
            // �浹�� ������Ʈ�� "Animals" �±׸� ������ �ִ��� Ȯ��
            if (currentCollider.gameObject.CompareTag("Animals"))
            {

                // Health ������Ʈ ��������
                Entity entity = currentCollider.GetComponent<Entity>();
                if (entity != null) {
                    // HP�� 20 ���ҽ�Ű��
                    entity.TakeDamage(20);
                    Debug.Log($"{entity.name} �� ����");
                }

                // HP�� ���ҵ� ���� ���� ������Ʈ�� HP�� 0�� ���� �ʾ��� ���, �Ʒ� ������ �������� ����
                if (entity == null || entity.Health > 0) {
                    return;
                }

                // �浹�� ������Ʈ�� ��ġ�� ȸ�� ���� ����
                Vector3 position = currentCollider.transform.position;
                Quaternion rotation = currentCollider.transform.rotation;

                // ��� �������� ������ ���ϴ� ���� �߰�
                int meatCount = 1;//�⺻��

                //�浹�� ������Ʈ�� �̸��� ���� ��� ���� ����
                string animalName = currentCollider.gameObject.name;

                    if (animalName.Contains("Cat"))
                    {
                        meatCount = Random.Range(1, 3);
                    }
                    else if (animalName.Contains("Dog"))
                    {
                        meatCount = Random.Range(2, 4);
                    }
                    else if (animalName.Contains("Chicken"))
                    {
                        meatCount = Random.Range(1, 4);
                    }
                    else if (animalName.Contains("Lion"))
                    {
                        meatCount = Random.Range(3, 6);
                    }

                    if(animalName.Contains("Baby"))
                {
                    meatCount = Mathf.CeilToInt(meatCount / 2.0f);
                }

                // ���� ���� ������Ʈ�� ����
                Destroy(currentCollider.gameObject);

                // ���ο� beaf ������ �ν��Ͻ� ����
                for (int i = 0; i < meatCount; i++)
                {
                    Vector3 spawnPosition = position + new Vector3(i*0.5f, 0, 0);
                    Instantiate(beafPrefab, spawnPosition, rotation);
                }
                }

            // �浹�� ������Ʈ�� "Grass" �±׸� ������ �ִ��� Ȯ��
            if (currentCollider.gameObject.CompareTag("Grass"))
            {
                // �浹�� ������Ʈ�� ��ġ�� ȸ�� ���� ����
                Vector3 position = currentCollider.transform.position;
                Quaternion rotation = currentCollider.transform.rotation;

                // ���� ���� ������Ʈ�� ����
                Destroy(currentCollider.gameObject);

                // ���ο� beaf ������ �ν��Ͻ� ����
                Instantiate(melonSeedPrefab, position, rotation);
            }

            // �浹�� ������Ʈ�� "Grass" �±׸� ������ �ִ��� Ȯ��
            if (currentCollider.gameObject.CompareTag("Ground"))
            {
                //�浹�� ������Ʈ�� Renderer ������Ʈ�� ������
                renderer = currentCollider.gameObject.GetComponent<Renderer>();
                if(renderer !=null)
                {
                    if (renderer.material == brownLightMaterial)
                    {
                        Debug.Log("��Ḧ �־����ϴ�");
                        //���͸����� �ٲ�
                        renderer.material = brownDarkMaterial;

                        if(fertilizedGrounds.ContainsKey(currentCollider.gameObject))
                        {
                            StopCoroutine(fertilizedGrounds[currentCollider.gameObject]);
                            fertilizedGrounds.Remove(currentCollider.gameObject);

                        }

                        //300f �Ŀ� ������� ����
                        StartCoroutine(ChangeToMelonAfterTime(currentCollider.gameObject, 300f));

                    }
                    else
                    {
                        //���� �������� ����
                        renderer.material = brownLightMaterial;
                    }
                }
            }

            // �浹 ���� �ʱ�ȭ
            currentCollider = null;
        }
    }

    private IEnumerator ChangeToMelonAfterTime(GameObject ground, float time)
    {
        float elapsedTime = 0f;
        Renderer groundRenderer = ground.GetComponent<Renderer>();

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            // 60f ���� ��ȭ�� ������ ���� �������� ����
            if (elapsedTime >= 60f && groundRenderer.material == brownDarkMaterial)
            {
                groundRenderer.material = brownLightMaterial;
                yield break;
            }

            yield return null;
        }

        // 300f �Ŀ� ������� ����
        if (groundRenderer.material == brownDarkMaterial)
        {
            Vector3 position = ground.transform.position;
            Quaternion rotation = ground.transform.rotation;
            Destroy(ground);
            Instantiate(melonPrefab, position, rotation);
        }
    }
}

