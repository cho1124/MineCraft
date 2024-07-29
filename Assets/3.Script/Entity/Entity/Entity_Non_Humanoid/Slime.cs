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

        for(int i = 0; i < 3; i++)
        {
            while (Vector3.Distance(random_direction, transform.position) < 3f)
            {
                random_direction = Random.insideUnitSphere * 5f;
                random_direction += spawn_position;
            }
            random_direction.y = spawn_position.y;
            Instantiate(slime_small, random_direction, Quaternion.identity);
        }
    }
}
