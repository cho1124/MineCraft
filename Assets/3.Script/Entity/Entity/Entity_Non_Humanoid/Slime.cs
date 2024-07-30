using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Slime : MonoBehaviour
    {

    [SerializeField] private GameObject slime_small;
    [SerializeField] private Vector3 spawn_position;
    [SerializeField] private Entity entity;

    protected void Awake() 
    {
        if (TryGetComponent(out entity))
        {
            entity.On_Death += Split; // 죽음 이벤트 구독
        }
    }

    private void Split()
    {
        spawn_position = transform.position; // 죽은 위치 저장
        Vector3 random_direction = Vector3.zero;

        for (int i = 0; i < 3; i++)
        {
            
            random_direction = new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f)).normalized * 2f;
            random_direction += spawn_position;
            Instantiate(slime_small, random_direction, Quaternion.identity);
        }
    }
}
