using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public GameObject melonSeedPrefab;
    public GameObject beafPrefab; // beaf 프리팹을 인스펙터에서 설정
    public Material brownLightMaterial;

    private Renderer renderer;
    private Collider currentCollider;

    private void OnTriggerEnter(Collider other)
    {
        // 충돌 이벤트 로그 출력
        Debug.Log("플레이어가 패고있습니다 : " + other.gameObject.name);

        // 현재 충돌 저장
        currentCollider = other;
    }

    private void Update()
    {
        // 마우스 왼쪽 클릭 확인
        if (Input.GetMouseButtonDown(0) && currentCollider != null)
        {
            // 충돌한 오브젝트가 "Animals" 태그를 가지고 있는지 확인
            if (currentCollider.gameObject.CompareTag("Animals"))
            {
                // 충돌한 오브젝트의 위치와 회전 값을 저장
                Vector3 position = currentCollider.transform.position;
                Quaternion rotation = currentCollider.transform.rotation;

                // 기존 동물 오브젝트를 삭제
                Destroy(currentCollider.gameObject);

                // 새로운 beaf 프리팹 인스턴스 생성
                Instantiate(beafPrefab, position, rotation);
            }

            // 충돌한 오브젝트가 "Grass" 태그를 가지고 있는지 확인
            if (currentCollider.gameObject.CompareTag("Grass"))
            {
                // 충돌한 오브젝트의 위치와 회전 값을 저장
                Vector3 position = currentCollider.transform.position;
                Quaternion rotation = currentCollider.transform.rotation;

                // 기존 동물 오브젝트를 삭제
                Destroy(currentCollider.gameObject);

                // 새로운 beaf 프리팹 인스턴스 생성
                Instantiate(melonSeedPrefab, position, rotation);
            }

            // 충돌한 오브젝트가 "Grass" 태그를 가지고 있는지 확인
            if (currentCollider.gameObject.CompareTag("Ground"))
            {
                //충돌한 오브젝트의 Renderer 컴포넌트를 가져옴
                renderer = currentCollider.gameObject.GetComponent<Renderer>();
                if(renderer !=null)
                {
                    //메터리얼을 바꿈
                    renderer.material = brownLightMaterial;
                }
            }

            // 충돌 정보 초기화
            currentCollider = null;
        }
    }
}

