using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    /*
    �÷��̾ �߽����� 30f������ ������ ���� ���� �ϰ� �����
    treespawner ó�� min max ���� ���� �� �ְ� �ؼ� �ּ� �ִ� �������� ���ϱ�
    ������ ������Ʈ�� ���� 5��*2(����/����)=10���̴ϱ� �迭�� ������ �ֱ�
    �ѹ� �����ϰ� ���� player�� position�� x�� z������ 30f�̻� ������ �� 
    �罺�� �ϰ�. 
    (���� �׽�Ʈ �ʿ����� 3f������ ���� �����ǰ� �غ���. )


    */

    public GameObject[] animalPrefabs;// ���� ������ �迭
    public int minSpawnCount = 1; //�ּ� ���� ����
    public int maxSpawnCount = 5; //�ִ� ���� ����
    public float spawnRadius = 3f;//���� �ݰ�
    public float respawnDistance = 3f; //�罺�� �Ÿ�

    private Vector3 lastPlayerPosition;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastPlayerPosition = playerTransform.position;
        SpawnAnimals();
    }

    void Update()
    {
        if (Vector3.Distance(playerTransform.position, lastPlayerPosition) > respawnDistance)
        {
            lastPlayerPosition = playerTransform.position;
            SpawnAnimals();
        }
    }

    void SpawnAnimals()
    {
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnAnimal();
        }
    }
    void SpawnAnimal()
    {
        Vector3 spawnPosition = GetRandomPosition();
        GameObject animalPrefab = animalPrefabs[Random.Range(0, animalPrefabs.Length)];
        Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
    }

    Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += playerTransform.position;
        randomDirection.y = playerTransform.position.y; //y�� ����
        return randomDirection;
    }
}
