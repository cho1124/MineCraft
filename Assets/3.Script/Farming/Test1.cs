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

    public GameObject melonSeedPrefab;
    public GameObject melonPrefab;
    public GameObject beafPrefab; // beaf 프리팹을 인스펙터에서 설정
    public Material brownLightMaterial;
    public Material brownDarkMaterial;

    private Renderer renderer;
    private Collider currentCollider;
    private Dictionary<GameObject, Coroutine> fertilizedGrounds = new Dictionary<GameObject, Coroutine>();

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

                // Health 컴포넌트 가져오기
                Entity entity = currentCollider.GetComponent<Entity>();
                if (entity != null) {
                    // HP를 20 감소시키기
                    entity.TakeDamage(20);
                    Debug.Log($"{entity.name} 를 때림");
                }

                // HP가 감소된 후의 동물 오브젝트의 HP가 0이 되지 않았을 경우, 아래 로직을 실행하지 않음
                if (entity == null || entity.Health > 0) {
                    return;
                }

                // 충돌한 오브젝트의 위치와 회전 값을 저장
                Vector3 position = currentCollider.transform.position;
                Quaternion rotation = currentCollider.transform.rotation;

                // 고기 프리팹의 개수를 정하는 로직 추가
                int meatCount = 1;//기본값

                //충돌한 오브젝트의 이름에 따라 고기 개수 결정
                string animalName = currentCollider.gameObject.name;

                    if (animalName.Contains("Cat"))
                    {
                        meatCount = Random.Range(1, 3);
                    }
                    else if (animalName.Contains("Dog"))
                    {
                        meatCount = Random.Range(2, 4);
                    }
                    else if (animalName.Contains("Chicken"))
                    {
                        meatCount = Random.Range(1, 4);
                    }
                    else if (animalName.Contains("Lion"))
                    {
                        meatCount = Random.Range(3, 6);
                    }

                    if(animalName.Contains("Baby"))
                {
                    meatCount = Mathf.CeilToInt(meatCount / 2.0f);
                }

                // 기존 동물 오브젝트를 삭제
                Destroy(currentCollider.gameObject);

                // 새로운 beaf 프리팹 인스턴스 생성
                for (int i = 0; i < meatCount; i++)
                {
                    Vector3 spawnPosition = position + new Vector3(i*0.5f, 0, 0);
                    Instantiate(beafPrefab, spawnPosition, rotation);
                }
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
                    if (renderer.material == brownLightMaterial)
                    {
                        Debug.Log("비료를 주었습니다");
                        //메터리얼을 바꿈
                        renderer.material = brownDarkMaterial;

                        if(fertilizedGrounds.ContainsKey(currentCollider.gameObject))
                        {
                            StopCoroutine(fertilizedGrounds[currentCollider.gameObject]);
                            fertilizedGrounds.Remove(currentCollider.gameObject);

                        }

                        //300f 후에 멜론으로 변경
                        StartCoroutine(ChangeToMelonAfterTime(currentCollider.gameObject, 300f));

                    }
                    else
                    {
                        //연한 갈색으로 변경
                        renderer.material = brownLightMaterial;
                    }
                }
            }

            // 충돌 정보 초기화
            currentCollider = null;
        }
    }

    private IEnumerator ChangeToMelonAfterTime(GameObject ground, float time)
    {
        float elapsedTime = 0f;
        Renderer groundRenderer = ground.GetComponent<Renderer>();

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            // 60f 동안 변화가 없으면 연한 갈색으로 변경
            if (elapsedTime >= 60f && groundRenderer.material == brownDarkMaterial)
            {
                groundRenderer.material = brownLightMaterial;
                yield break;
            }

            yield return null;
        }

        // 300f 후에 멜론으로 변경
        if (groundRenderer.material == brownDarkMaterial)
        {
            Vector3 position = ground.transform.position;
            Quaternion rotation = ground.transform.rotation;
            Destroy(ground);
            Instantiate(melonPrefab, position, rotation);
        }
    }
}

