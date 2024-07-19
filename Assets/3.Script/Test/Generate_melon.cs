using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//animals tag 가진 오브젝트가 이 스크립트를 가진 곳과 10번 충돌하면 melon 생성

public class Generate_melon : MonoBehaviour
{
    public GameObject melonPrefab; // Melon 프리팹을 인스펙터에서 할당
    public Vector3 spawnOffset; // Melon 프리팹 생성 위치의 오프셋
    private Dictionary<Vector3, int> collisionCount = new Dictionary<Vector3, int>();
    // 충돌 횟수를 저장할 딕셔너리

    void OnCollisionEnter(Collision collision)
    {
    if(collision.gameObject.CompareTag("Animals"))
        {
            Vector3 collisionPoint = collision.contacts[0].point;

            //충돌 지점을 기준으로 충돌 횟수를 증가시킴
            if(collisionCount.ContainsKey(collisionPoint))
            {
                collisionCount[collisionPoint]++;
            }
            else
            {
                collisionCount[collisionPoint] = 1;
            }

            //충돌 횟수가 10회 이상인 경우 Melon 프리팹을 생성
            if(collisionCount[collisionPoint]>=10)
            {
                Instantiate(melonPrefab, collisionPoint + spawnOffset, Quaternion.identity);
                collisionCount[collisionPoint] = 0;//충돌 횟수 초기화
                Debug.Log("멜론이 생성되었습니다! ");

            }
        }
    }
}
