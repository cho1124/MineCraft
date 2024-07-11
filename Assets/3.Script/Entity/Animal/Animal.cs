using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Entity
{
    //동물도 자유롭게 구현하면 될거에용

    /*
    동일한 스크립트를 가진 각각의 오브젝트가 3회 충돌하면 (최근 raycast로 인식하는 10개의 개체를 저장하고 같은 개체가 3번 있으면 생성)
    본인의 1/3 사이즈를 가진 새로운 오브젝트를 생성하는 메서드 
    */

    /*
     1.  동물 성인상태 / 새끼상태 
     -> 프리팹을 인스펙터에 저장해서 그거를 성인상태로 잡고 그것에서 
    1/2 scale 이 되면 새끼인걸로 하는게 어떨지
     
     2. 새끼상태일때 
     배고프지 않음+3일이 지나면 성인이 되는걸로? (날짜는 동물별로 달라야 할듯)
     3일을 어떻게 카운트 할것이냐? (현실 시간 기준으로 잡을지...?)
     
     
     */

    public GameObject adultPrefab; //  성인 상태의 프리팹
    private bool isAdult; // 성인 상태인지 여부
    private float growthTime; // 성장 시간 
    private float currentTime = 0f; // 현재 시간
    public GameObject targetAnimal; // 충돌을 추적할 타겟 동물
    public int hungerLevel = 6; // 배고픔 게이지

    private int collisionCount = 0; // 충돌 횟수
    private const int collisionThreshold = 2; // 충돌 임계값

    private const int maxHungerLevel = 10;
    private bool isHungry = false; // 배고픔 상태
    private bool isFull = false; // 배부름 상태
    public float baseSpeed; // 기본 이동 속도, 인스펙터에서 설정
    private float speedIncrease = 1f; // 추가 이동 속도
    private float currentSpeed; // 현재 이동 속도

    protected Queue<GameObject> recentAnimals = new Queue<GameObject>(); // 최근 탐색된 10개의 개체를 저장할 큐
    protected Dictionary<GameObject, int> animalCount = new Dictionary<GameObject, int>(); // 탐색된 개체의 탐색 횟수를 저장할 딕셔너리

    void Start()
    {
    if(transform.localScale==adultPrefab.transform.localScale/2)
        {
            isAdult = false; //새끼상태
        }
        else
        {
            isAdult = true; // 성인 상태
        }
        // 초기 이동 속도 설정
        currentSpeed = baseSpeed;

        // 배고픔 감소 루틴 시작
        StartCoroutine(HungerRoutine());
    }

    void Update()
    {
        if (!isAdult)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= growthTime&& !isHungry)
            {
                GrowUp();
            }
        }
        if (hungerLevel <= 3)
        {
            SearchForFood();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{name}과 {collision.gameObject.name}이 충돌했습니다.");
        // 타겟 동물과의 충돌인지 확인
        if(collision.gameObject==targetAnimal)
        {
            collisionCount ++;

            if(collisionCount>=collisionThreshold)
            {
                SpawnNewAnimal();
                collisionCount = 0;// 충돌 횟수 초기화
            }
        }
        // Food 태그가 붙은 오브젝트를 먹는 처리
        if (collision.gameObject.CompareTag("Food") && !isFull)
        {
            Eat();
            Destroy(collision.gameObject); // 음식 오브젝트 제거
        }
    }

    private void SpawnNewAnimal()
    {
        // 현재 오브젝트의 크기와 프리팹의 크기를 비교
        if (transform.localScale == adultPrefab.transform.localScale)
        {
            // 현재 오브젝트를 복제
            GameObject newAnimal = Instantiate(gameObject, transform.position, transform.rotation);
            // 복제된 오브젝트의 크기를 1/3으로 설정
            newAnimal.transform.localScale = transform.localScale / 2;

            // 복제된 오브젝트의 충돌 카운트를 초기화
            Animal tracker = newAnimal.GetComponent<Animal>();
            if (tracker != null)
            {
                tracker.collisionCount = 0;
            }
        }
    }

    private void GrowUp()
    {
        // 성인 상태로 성장
        transform.localScale *= 2;
        isAdult = true; // 성인 상태로 변경
        Debug.Log($"{name}가 성인으로 성장했습니다.");
    }

    private void Eat()
    {
        if (hungerLevel < maxHungerLevel)
        {
            hungerLevel++;
            Debug.Log($"{name}가 사료를 먹었습니다. 배고픔 게이지: {hungerLevel}");
        }

        // 배고픔 상태 관리
        if (hungerLevel > 5)
        {
            isHungry = false;
            isFull = true;
        }
        else
        {
            isHungry = true;
            isFull = false;
        }
    }

    private IEnumerator HungerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);

            if (hungerLevel > 0)
            {
                hungerLevel--;
                Debug.Log($"{name}의 배고픔 게이지 감소: {hungerLevel}");
            }

            // 배고픔 상태 관리
            if (hungerLevel > 5)
            {
                isHungry = false;
                isFull = true;
                currentSpeed = baseSpeed; // 배부름 상태에서는 기본 속도
            }
            else
            {
                isHungry = true;
                isFull = false;

                if (hungerLevel <= 3)
                {
                    currentSpeed = baseSpeed + speedIncrease; // 배고픔 상태에서는 속도 증가
                }
                else
                {
                    currentSpeed = baseSpeed; // 기본 속도
                }
            }
        }
    }

    private void SearchForFood()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Food"))
            {
                MoveTowards(hitCollider.transform.position);
                break;
            }
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
