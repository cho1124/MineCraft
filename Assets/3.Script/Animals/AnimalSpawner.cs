using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    /*
    플레이어를 중심으로 30f범위(인스펙터에서 조정가능)로 동물을 랜덤 스폰 하게 만들것
    treespawner 처럼 min max 갯수 넣을 수 있게 해서 최소 최대 생성갯수 정하기
    생성될 오브젝트는 동물 5종*2(성인/새끼)=10종이니까 배열로 프리팹 넣기
    한번 생성하고 나서 player의 position이 x나 z축으로 30f이상 변했을 시 
    재스폰 하게. 
    (현재 테스트 맵에서는 3f범위로 동물 스폰되게 해보자. )

    플레이어 위치에서 생성되어서 플레이어랑 부딧치는 경우가 많아서 플레이어주변 반경 일정거리
    생성되지 않게 하려고 함
    */

    public GameObject[] animalPrefabs;// 동물 프리팹 배열
    public int minSpawnCount = 1; //최소 스폰 갯수
    public int maxSpawnCount = 5; //최대 스폰 갯수
    public float spawnRadius = 3f;//스폰 반경
    public float respawnDistance = 3f; // 재스폰 거리, 플레이어가 이 거리를 이동하면 다시 동물을 스폰합니다.
    public float invincibilityDuration = 2f; // 무적 상태 지속 시간

    private Vector3 lastPlayerPosition;
    private Transform playerTransform;

    void Start()
    {
        // 플레이어의 Transform을 찾습니다. "Player" 태그가 붙은 게임 오브젝트를 찾습니다.
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // 마지막 플레이어 위치를 현재 플레이어 위치로 초기화합니다.
        lastPlayerPosition = playerTransform.position;
        // 처음으로 동물을 스폰합니다.
        SpawnAnimals();
    }

    void Update()
    {
        // 플레이어의 현재 위치와 마지막 위치 사이의 거리를 계산하여 재스폰 거리를 초과했는지 확인합니다.
        if (Vector3.Distance(playerTransform.position, lastPlayerPosition) > respawnDistance)
        {
            // 마지막 플레이어 위치를 현재 위치로 갱신합니다.
            lastPlayerPosition = playerTransform.position;
            SpawnAnimals();
        }
    }

    void SpawnAnimals()
    {
        // 스폰할 동물의 개수를 랜덤으로 결정합니다.
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnAnimal();
        }
    }
    void SpawnAnimal()
    {
        // 랜덤 위치를 얻어 스폰 위치로 설정합니다.
        Vector3 spawnPosition = GetRandomPosition();
        // 랜덤으로 동물 프리팹을 선택합니다.
        GameObject animalPrefab = animalPrefabs[Random.Range(0, animalPrefabs.Length)];
        // 동물 프리팹을 스폰 위치에 생성합니다.
        GameObject spawnedAnimal = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
        // 생성된 동물에 "Animals" 태그를 추가합니다.
        spawnedAnimal.tag = "Animals";
        // 생성된 동물에 무적 상태를 설정합니다.
        Animal animal = spawnedAnimal.GetComponent<Animal>();
        if (animal != null)
        {
            animal.SetInvincible(invincibilityDuration);
        }
    }

    Vector3 GetRandomPosition()
    {
        // 플레이어 주변의 랜덤 방향을 설정합니다.
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += playerTransform.position;

        // 플레이어 주변 1f 공간에 동물이 스폰되지 않도록 합니다.
        while (Vector3.Distance(randomDirection, playerTransform.position) < 1f)
        {
            randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection += playerTransform.position;
        }

        randomDirection.y = playerTransform.position.y; //y축 고정
        return randomDirection;
    }
}
