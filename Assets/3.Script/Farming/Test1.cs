using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    /*
    (물을 사용하지 않기 위해 애를 쓰며)
    짜치는 농사 테스트중
  
    1. grass 태그 붙은 큐브 뽀개면 melonseed 나옴
    2. ground 태그 붙은 큐브 뽀개면 연한갈색으로 바뀜
    3. 연한갈색으로 바뀐 큐브를 때리면 "비료를 주었습니다" 로그 뜨면서 진한 갈색으로 바뀜
    4. 진한 갈색으로 바뀐 큐브는 60f간 아무런 변화가 없으면 다시 연한 갈색으로 바뀜
    (주기적으로 플레이어가 계속 때려주어야함)
    5. 300f동안 진한 갈색 큐브가 유지되면 진한 갈색 큐브가 멜론으로 바뀜! 
  
    [멜론seed 이용방법 고민중]
    */
  
    public GameObject beafPrefab; // beaf 프리팹을 인스펙터에서 설정
  
    private Collider currentCollider;
    private Dictionary<GameObject, Coroutine> fertilizedGrounds = new Dictionary<GameObject, Coroutine>();
  
    private void OnTriggerEnter(Collider other)
    {
        // 충돌 이벤트 로그 출력
        Debug.Log("플레이어가 패고있습니다 : " + other.gameObject.name);
  
        // 현재 충돌 저장
        currentCollider = other;
    }
  
    private void Update() {
        // 마우스 왼쪽 클릭 확인
        if (Input.GetMouseButtonDown(0) && currentCollider != null) {
            // Monster 또는 Animals 태그를 가진 오브젝트인지 확인
            if (currentCollider.gameObject.CompareTag("Monster") || currentCollider.gameObject.CompareTag("Animals")) {
                // Health 컴포넌트 가져오기
                Entity entity = currentCollider.GetComponent<Entity>();
                if (entity != null) {
                    // HP를 100 감소시키기
                    Debug.Log("플레이어 무기로 공격당함");
                    entity.TakeDamage(100);
                    Debug.Log($"{entity.name} 를 때림");

                }
            }
            currentCollider = null;
        }
    }
}
