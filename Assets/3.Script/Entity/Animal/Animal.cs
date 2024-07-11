using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Entity
{
    //������ �����Ӱ� �����ϸ� �ɰſ���

    /*
    ������ ��ũ��Ʈ�� ���� ������ ������Ʈ�� 10ȸ �浹�ϸ� 
    ������ 1/3 ����� ���� ���ο� ������Ʈ�� �����ϴ� �޼��� 
    */

    /*
     1.  ���� ���λ��� / �������� 
     -> �������� �ν����Ϳ� �����ؼ� �װŸ� ���λ��·� ��� �װͿ��� 
    1/2 scale �� �Ǹ� �����ΰɷ� �ϴ°� ���
     
     2. ���������϶� 
     ������� ����+3���� ������ ������ �Ǵ°ɷ�? (��¥�� �������� �޶�� �ҵ�)
     3���� ��� ī��Ʈ �Ұ��̳�? (���� �ð� �������� ������...?)
     
     
     */

    public GameObject adultPrefab; //  ���� ������ ������
    public GameObject animalPrefab; //  ���� ������ ������
    public bool isHungry = false; // ����� ����
    private bool isAdult; // ���� �������� ����
    private float growthTime; // ���� �ð� 
    private float currentTime = 0f; // ���� �ð�
    public GameObject targetAnimal; // �浹�� ������ Ÿ�� ����

    private int collisionCount = 0; // �浹 Ƚ��
    private const int collisionThreshold = 2; // �浹 �Ӱ谪

    void Start()
    {
    if(transform.localScale==adultPrefab.transform.localScale/2)
        {
            isAdult = false; //��������
        }
        else
        {
            isAdult = true; // ���� ����
        }
    }

    void Update()
    {
        if (!isAdult)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= growthTime&& !isHungry)
            {
                GrowUp();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{name}�� {collision.gameObject.name}�� �浹�߽��ϴ�.");
        // Ÿ�� �������� �浹���� Ȯ��
        if(collision.gameObject==targetAnimal)
        {
            collisionCount ++;

            if(collisionCount>=collisionThreshold)
            {
                SpawnNewAnimal();
                collisionCount = 0;// �浹 Ƚ�� �ʱ�ȭ
            }
        }
    }

    private void SpawnNewAnimal()
    {
        // ���� ������Ʈ�� ũ��� �������� ũ�⸦ ��
        if (transform.localScale == animalPrefab.transform.localScale)
        {
            // ���� ������Ʈ�� ����
            GameObject newAnimal = Instantiate(gameObject, transform.position, transform.rotation);
            // ������ ������Ʈ�� ũ�⸦ 1/3���� ����
            newAnimal.transform.localScale = transform.localScale / 3;

            // ������ ������Ʈ�� �浹 ī��Ʈ�� �ʱ�ȭ
            Animal tracker = newAnimal.GetComponent<Animal>();
            if (tracker != null)
            {
                tracker.collisionCount = 0;
            }
        }
    }

    private void GrowUp()
    {
        // ���� ���·� ����
        transform.localScale *= 2;
        isAdult = true; // ���� ���·� ����
        Debug.Log($"{name}�� �������� �����߽��ϴ�.");
    }

    private void Eat()
    {
        isHungry = false; // ����� ���� �ذ�
        Debug.Log($"{name}�� ��Ḧ �Ծ����ϴ�. ����� ����: {isHungry}");
    }
}
