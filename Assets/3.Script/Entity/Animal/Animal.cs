using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Entity
{
    //������ �����Ӱ� �����ϸ� �ɰſ���

    /*
    ������ ��ũ��Ʈ�� ���� ������ ������Ʈ�� 3ȸ �浹�ϸ� (�ֱ� raycast�� �ν��ϴ� 10���� ��ü�� �����ϰ� ���� ��ü�� 3�� ������ ����)
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
    private bool isAdult; // ���� �������� ����
    private float growthTime; // ���� �ð� 
    private float currentTime = 0f; // ���� �ð�
    public GameObject targetAnimal; // �浹�� ������ Ÿ�� ����
    public int hungerLevel = 6; // ����� ������

    private int collisionCount = 0; // �浹 Ƚ��
    private const int collisionThreshold = 2; // �浹 �Ӱ谪

    private const int maxHungerLevel = 10;
    private bool isHungry = false; // ����� ����
    private bool isFull = false; // ��θ� ����
    public float baseSpeed; // �⺻ �̵� �ӵ�, �ν����Ϳ��� ����
    private float speedIncrease = 1f; // �߰� �̵� �ӵ�
    private float currentSpeed; // ���� �̵� �ӵ�

    protected Queue<GameObject> recentAnimals = new Queue<GameObject>(); // �ֱ� Ž���� 10���� ��ü�� ������ ť
    protected Dictionary<GameObject, int> animalCount = new Dictionary<GameObject, int>(); // Ž���� ��ü�� Ž�� Ƚ���� ������ ��ųʸ�

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
        // �ʱ� �̵� �ӵ� ����
        currentSpeed = baseSpeed;

        // ����� ���� ��ƾ ����
        StartCoroutine(HungerRoutine());
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
        if (hungerLevel <= 3)
        {
            SearchForFood();
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
        // Food �±װ� ���� ������Ʈ�� �Դ� ó��
        if (collision.gameObject.CompareTag("Food") && !isFull)
        {
            Eat();
            Destroy(collision.gameObject); // ���� ������Ʈ ����
        }
    }

    private void SpawnNewAnimal()
    {
        // ���� ������Ʈ�� ũ��� �������� ũ�⸦ ��
        if (transform.localScale == adultPrefab.transform.localScale)
        {
            // ���� ������Ʈ�� ����
            GameObject newAnimal = Instantiate(gameObject, transform.position, transform.rotation);
            // ������ ������Ʈ�� ũ�⸦ 1/3���� ����
            newAnimal.transform.localScale = transform.localScale / 2;

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
        if (hungerLevel < maxHungerLevel)
        {
            hungerLevel++;
            Debug.Log($"{name}�� ��Ḧ �Ծ����ϴ�. ����� ������: {hungerLevel}");
        }

        // ����� ���� ����
        if (hungerLevel > 5)
        {
            isHungry = false;
            isFull = true;
        }
        else
        {
            isHungry = true;
            isFull = false;
        }
    }

    private IEnumerator HungerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);

            if (hungerLevel > 0)
            {
                hungerLevel--;
                Debug.Log($"{name}�� ����� ������ ����: {hungerLevel}");
            }

            // ����� ���� ����
            if (hungerLevel > 5)
            {
                isHungry = false;
                isFull = true;
                currentSpeed = baseSpeed; // ��θ� ���¿����� �⺻ �ӵ�
            }
            else
            {
                isHungry = true;
                isFull = false;

                if (hungerLevel <= 3)
                {
                    currentSpeed = baseSpeed + speedIncrease; // ����� ���¿����� �ӵ� ����
                }
                else
                {
                    currentSpeed = baseSpeed; // �⺻ �ӵ�
                }
            }
        }
    }

    private void SearchForFood()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Food"))
            {
                MoveTowards(hitCollider.transform.position);
                break;
            }
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
