using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Entity_Spawner : MonoBehaviour
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

    [SerializeField] private GameObject[] entity_prefabs;
    [SerializeField] private int minSpawnCount = 1; //�ּ� ���� ����
    [SerializeField] int maxSpawnCount = 5; //�ִ� ���� ����
    [SerializeField] private float spawn_radius = 10f;//���� �ݰ�
    [SerializeField] private float spawn_distance = 10f; // �罺�� �Ÿ�, �÷��̾ �� �Ÿ��� �̵��ϸ� �ٽ� ������ �����մϴ�.

    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 last_spawn_position;

    
    private void Update()
    {
        // �÷��̾��� ���� ��ġ�� ������ ��ġ ������ �Ÿ��� ����Ͽ� �罺�� �Ÿ��� �ʰ��ߴ��� Ȯ���մϴ�.
        if (Vector3.Distance(player.transform.position, last_spawn_position) > spawn_distance)
        {
            // ������ �÷��̾� ��ġ�� ���� ��ġ�� �����մϴ�.
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
        // �÷��̾� �ֺ��� ���� ������ �����մϴ�.
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
