using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    /*
    플레이어 무기(?)인 hoe 에 달려있는 스크립트입니다.

    */
  
    public GameObject beafPrefab; // beaf 프리팹을 인스펙터에서 설정
  
    private Collider currentCollider;

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 이벤트 로그 출력
        Debug.Log("플레이어가 패고있습니다 : " + collision.gameObject.name);
        AudioManager.instance.PlayRandomSFX("Humanoid", "Attack1"); //타격음
        // Monster 또는 Animals 태그를 가진 오브젝트인지 확인
        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Animals"))
        {
            // Entity 컴포넌트 가져오기
            Entity entity = collision.gameObject.GetComponent<Entity>();
            if (entity != null)
            {
                // HP를 100 감소시키기
                Debug.Log("플레이어 무기로 공격당함");
                entity.TakeDamage(30);
                Debug.Log($"{entity.name} 를 때림");
            }
            
       }
    }
}
