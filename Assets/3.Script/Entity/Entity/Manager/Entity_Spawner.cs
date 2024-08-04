using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Entity_Spawner : MonoBehaviour
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

    [SerializeField] private GameObject[] entity_prefabs;
    [SerializeField] private int minSpawnCount = 1; //최소 스폰 갯수
    [SerializeField] int maxSpawnCount = 5; //최대 스폰 갯수
    [SerializeField] private float spawn_radius = 10f;//스폰 반경
    [SerializeField] private float spawn_distance = 10f; // 재스폰 거리, 플레이어가 이 거리를 이동하면 다시 동물을 스폰합니다.

    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 last_spawn_position;

    
    private void Update()
    {
        // 플레이어의 현재 위치와 마지막 위치 사이의 거리를 계산하여 재스폰 거리를 초과했는지 확인합니다.
        if (Vector3.Distance(player.transform.position, last_spawn_position) > spawn_distance)
        {
            // 마지막 플레이어 위치를 현재 위치로 갱신합니다.
            last_spawn_position = player.transform.position;
            Spawn_Entity();
        }
    }

    private void Spawn_Entity()
    {
        for(int i = 0; i < Random.Range(minSpawnCount, maxSpawnCount + 1); i++) Instantiate(entity_prefabs[Random.Range(0, entity_prefabs.Length)], Get_Random_Position(), Quaternion.identity);
    }

    private Vector3 Get_Random_Position()
    {
        // 플레이어 주변의 랜덤 방향을 설정합니다.
        Vector3 random_direction = Vector3.zero;

        random_direction = Random.insideUnitSphere * spawn_radius;
        random_direction += player.transform.position;

        while (Vector3.Distance(random_direction, player.transform.position) < 5f)
        {
            random_direction = Random.insideUnitSphere * spawn_radius;
            random_direction += player.transform.position;

        }
        return random_direction;
    }

    //private void OnDrawGizmos()
    //{
    //    Vector3 random_direction = Vector3.zero;
    //    random_direction = Random.insideUnitSphere * spawn_radius;
    //    Gizmos.DrawRay(player.transform.position, random_direction);
    //}
}
