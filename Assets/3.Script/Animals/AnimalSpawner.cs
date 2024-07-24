using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    /*
    �÷��̾ �߽����� 30f����(�ν����Ϳ��� ��������)�� ������ ���� ���� �ϰ� �����
    treespawner ó�� min max ���� ���� �� �ְ� �ؼ� �ּ� �ִ� �������� ���ϱ�
    ������ ������Ʈ�� ���� 5��*2(����/����)=10���̴ϱ� �迭�� ������ �ֱ�
    �ѹ� �����ϰ� ���� player�� position�� x�� z������ 30f�̻� ������ �� 
    �罺�� �ϰ�. 
    (���� �׽�Ʈ �ʿ����� 3f������ ���� �����ǰ� �غ���. )

    �÷��̾� ��ġ���� �����Ǿ �÷��̾�� �ε�ġ�� ��찡 ���Ƽ� �÷��̾��ֺ� �ݰ� �����Ÿ�
    �������� �ʰ� �Ϸ��� ��
    */

    public GameObject[] animalPrefabs;// ���� ������ �迭
    public int minSpawnCount = 1; //�ּ� ���� ����
    public int maxSpawnCount = 5; //�ִ� ���� ����
    public float spawnRadius = 3f;//���� �ݰ�
    public float respawnDistance = 3f; // �罺�� �Ÿ�, �÷��̾ �� �Ÿ��� �̵��ϸ� �ٽ� ������ �����մϴ�.
    public float invincibilityDuration = 2f; // ���� ���� ���� �ð�

    private Vector3 lastPlayerPosition;
    private Transform playerTransform;

    void Start()
    {
        // �÷��̾��� Transform�� ã���ϴ�. "Player" �±װ� ���� ���� ������Ʈ�� ã���ϴ�.
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // ������ �÷��̾� ��ġ�� ���� �÷��̾� ��ġ�� �ʱ�ȭ�մϴ�.
        lastPlayerPosition = playerTransform.position;
        // ó������ ������ �����մϴ�.
        SpawnAnimals();
    }

    void Update()
    {
        // �÷��̾��� ���� ��ġ�� ������ ��ġ ������ �Ÿ��� ����Ͽ� �罺�� �Ÿ��� �ʰ��ߴ��� Ȯ���մϴ�.
        if (Vector3.Distance(playerTransform.position, lastPlayerPosition) > respawnDistance)
        {
            // ������ �÷��̾� ��ġ�� ���� ��ġ�� �����մϴ�.
            lastPlayerPosition = playerTransform.position;
            SpawnAnimals();
        }
    }

    void SpawnAnimals()
    {
        // ������ ������ ������ �������� �����մϴ�.
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnAnimal();
        }
    }
    void SpawnAnimal()
    {
        // ���� ��ġ�� ��� ���� ��ġ�� �����մϴ�.
        Vector3 spawnPosition = GetRandomPosition();
        // �������� ���� �������� �����մϴ�.
        GameObject animalPrefab = animalPrefabs[Random.Range(0, animalPrefabs.Length)];
        // ���� �������� ���� ��ġ�� �����մϴ�.
        GameObject spawnedAnimal = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
        // ������ ������ "Animals" �±׸� �߰��մϴ�.
        spawnedAnimal.tag = "Animals";
        // ������ ������ ���� ���¸� �����մϴ�.
        Animal animal = spawnedAnimal.GetComponent<Animal>();
        if (animal != null)
        {
            animal.SetInvincible(invincibilityDuration);
        }
    }

    Vector3 GetRandomPosition()
    {
        // �÷��̾� �ֺ��� ���� ������ �����մϴ�.
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += playerTransform.position;

        // �÷��̾� �ֺ� 1f ������ ������ �������� �ʵ��� �մϴ�.
        while (Vector3.Distance(randomDirection, playerTransform.position) < 1f)
        {
            randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection += playerTransform.position;
        }

        randomDirection.y = playerTransform.position.y; //y�� ����
        return randomDirection;
    }
}
