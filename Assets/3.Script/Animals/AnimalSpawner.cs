using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    /*
    플레이어를 중심으로 30f범위로 동물을 랜덤 스폰 하게 만들것
    treespawner 처럼 min max 갯수 넣을 수 있게 해서 최소 최대 생성갯수 정하기
    생성될 오브젝트는 동물 5종*2(성인/새끼)=10종이니까 배열로 프리팹 넣기
    한번 생성하고 나서 player의 position이 x나 z축으로 30f이상 변했을 시 
    재스폰 하게. 
    (현재 테스트 맵에서는 3f범위로 동물 스폰되게 해보자. )


    */

    public GameObject[] animalPrefabs;// 동물 프리팹 배열
    public int minSpawnCount = 1; //최소 스폰 갯수
    public int maxSpawnCount = 5; //최대 스폰 갯수
    public float spawnRadius = 3f;//스폰 반경
    public float respawnDistance = 3f; //재스폰 거리

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
        randomDirection.y = playerTransform.position.y; //y축 고정
        return randomDirection;
    }
}
