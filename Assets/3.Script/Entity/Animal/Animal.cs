using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Entity, IDamageable
{
    //동물도 자유롭게 구현하면 될거에용

    /*
    
    동물들은 배고픔 상태에서 이속이 빨라짐
    새끼는 foodGivenCount 가 오르지 않음.
    성인상태에서 5번 Animal_Food를 먹으면 tamed 가 이름에 붙고, 그 상태에서 
    5번 Animal_Food 를 또 먹으면 overgrowth됨.
    overgrowth 상황일때는 덩치가 커지고 고기를 2배 더 줌.
    예상되는 배고픔 게이지가 맥스배고픔게이지보다 클 경우 먹지 못함


     */

    private bool isInvincible = false; // 무적 상태 변수(동물이 태어나자마 몬스터와 부딧쳐 죽는 경우가 많아서)
    // animalspawner 스크립트에 무적상태 2초로 설정하여 태어나서 2초동안은 무적이 되도록

    public GameObject beafPrefab; // beaf 프리팹을 인스펙터에서 설정

    // 성장 관련 변수들
    public GameObject adultPrefab; //  성인 상태의 프리팹
    public GameObject babyPrefab; // 새끼 상태의 프리팹
    public GameObject overgrowthPrefab; // 테이밍 오브젝트 프리팹
    private bool isAdult; // 성인 상태인지 여부
    private float growthTime = 60; // 성장 기준 시간 
    private float growthTimeGaze = 0; // 배부름총족이후흐르는시간 :성장 타이머

    // 배고픔 관련 변수들
    public int hungerLevel = 7; // 배고픔 게이지
    private const int maxHungerLevel = 10;
    private bool isHungry = false; // 배고픔 상태
    private bool isFull = false; // 배부름 상태
    private float speedIncrease = 1f; // 추가 이동 속도
    private int hungerIncrease;

    // 플레이어가 음식을 준 횟수를 저장할 변수
    private int foodGivenCount = 0;
    private const int OvergrowthThreshold = 5;
    private bool isOvergrowth = false;
    private bool isTamed = false;

    // 이동 속도 관련 변수들
    public float walkSpeed = 3; // 기본 이동 속도, 인스펙터에서 설정
    public float runSpeed = 4; // 기본 이동 속도, 인스펙터에서 설정
    private float currentSpeed; // 현재 이동 속도
    public float minWalkTime = 4f; //걷기 최소 시간
    public float maxWalkTime = 8f; //걷기 최대 시간
    public float jumpForce = 4f; //점프할때 힘

    // 개체 복사 관련 변수들
    private bool canSpawn = true; // 개체 복사 쿨타임 플래그
    private float spawnCooldown = 600f; // 쿨타임 시간
    private const int collisionThreshold = 10; // 충돌 임계값
    protected Queue<int> recentAnimals = new Queue<int>(); // 최근 탐색된 10개의 개체를 저장할 큐
    protected Dictionary<int, int> animalCount = new Dictionary<int, int>(); // 탐색된 개체의 탐색 횟수를 저장할 딕셔너리

    // 컴포넌트 관련 변수들
    public GameObject heartObjectPrefab; // 하트 오브젝트 프리팹
    public GameObject shockObjectPrefab; // 충격 오브젝트 프리팹
    private Vector3 targetPosition; //목적지
    public Transform head; // 동물의 head 참조

    // 탐지 관련 변수들
    public float wanderRadius = 30f; // 동물이 배회할 수 있는 최대 반경입니다.

    // 상태 관련 변수들
    protected enum State { Walk, Jump, Idle, Run }
    protected State currentState;
    private Coroutine stateCoroutine;

    // 위치 변화 감지 관련 변수들
    private Vector3 lastPosition; //동물이 배회할 수 있는 최대 반경입니다.
    private float idleTime; //대기 시간을 추적하는 변수입니다.
    private float idleTimeLimit = 10f; // 10초 동안 위치가 변하지 않으면 이동
    private const float positionThreshold = 3f; // 위치 변화 허용 범위(10초동안 이 범위 안에만 있으면 네비메쉬목적지변경)

    private GameObject activeEffect;
    private GameObject overgrowthEffect; // overgrowth 마크를 위한 변수
    private bool isGrounded = true;

    protected override void Start()
    {

        base.Start();

        // Entity의 OnDeath 이벤트에 Die 메서드를 구독
        OnDeath += Die;

        //컴포넌트 초기화
        Collider col = GetComponent<Collider>();

        // 성장 관련 초기 설정
        if (gameObject.name.Contains("Baby"))
        {
            isAdult = false; // 새끼상태
        }
        else
        {
            isAdult = true; // 성인 상태
        }

        // 초기 이동 속도 설정
        currentSpeed = walkSpeed;

        // 배고픔 감소 루틴 시작
        StartCoroutine(HungerRoutine());

        // 초기 상태 설정
        ChangeState(State.Idle);

        // 위치 변화 감지 초기화
        lastPosition = transform.position;
        //동물들이 움직이지 않고 한곳에 멈춰있는지 확인하기 위한 대기시간 초기화
        idleTime = 0f;
    }

    void Update()
    {

        //성장 관련 업데이트
        //성인이 아니고 배부름상태가 growthTimeGaze 만큼 유지되면 성인으로 자람
        if (!isAdult && isFull)
        {
            growthTimeGaze += Time.deltaTime;
            if (growthTimeGaze >= growthTime)
            {
                GrowUp();
            }
        }

        // 배고픔 상태에 따른 성장 타이머 초기화
        if (!isFull)
        {
            growthTimeGaze = 0f;
        }

        // 위치 변화 감지
        CheckIdleTime();

    }

    private void FixedUpdate()
    {
        //몬스터가 뒤집혔을 경우 바로 세우기 
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            rb.AddTorque(Vector3.right * 10f); // 힘을 가해서 자연스럽게 스도록 유도하는 방법
        }

        // 현재 상태에 따라 행동 수행
        switch (currentState)
        {
            case State.Walk:
                Walk();
                break;
            case State.Jump:
                break;
            case State.Idle:
                break;
            case State.Run:
                Run();
                break;
        }
    }

    private void CheckIdleTime()
    {   //이동이 너무 오래 없을경우 목적지를 변경시킴
        // 현재 위치와 마지막 위치 사이의 거리를 계산합니다.
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        // 이동한 거리가 positionThreshold 이하인 경우 idleTime을 증가시킵니다.
        if (distanceMoved < positionThreshold)
        {
            idleTime += Time.deltaTime;
        }
        // 이동한 거리가 positionThreshold를 초과한 경우 idleTime을 초기화하고 마지막 위치를 업데이트합니다.
        else
        {
            idleTime = 0f;
            lastPosition = transform.position;
        }
        // idleTime이 idleTimeLimit을 초과한 경우 새로운 랜덤 목적지를 설정합니다.
        if (idleTime >= idleTimeLimit)
        {
            ChangeState(GetRandomState());
            idleTime = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // 자신과 같은 컴포넌트를 가진 오브젝트와의 충돌인지 확인
        if (collision.gameObject.GetComponent(GetType()) != null)
        {
            //최근 충돌한 동물 목록에 추가
            AddToRecentAnimals(collision.gameObject);
            int collisionAnimalId = collision.gameObject.GetInstanceID();
            if (animalCount.ContainsKey(collisionAnimalId) && animalCount[collisionAnimalId] >= collisionThreshold)
            {
                if (canSpawn && !isOvergrowth) // 과성장 상태가 아니고, 스폰 쿨타임이 지나야만 스폰
                {
                    SpawnNewAnimal();
                    animalCount[collisionAnimalId] = 0;
                }
            }
        }

        // Food 태그가 붙은 오브젝트를 먹는 처리
        if (collision.gameObject.CompareTag("Food"))
        {
            Eat("Food", hungerIncrease);
            Destroy(collision.gameObject); // 음식 오브젝트 제거
        }

        // Animal_Food 태그가 붙은 오브젝트를 먹는 처리
        if (collision.gameObject.CompareTag("Animal_Food"))
        {
            Eat("Animal_Food",1);
            Destroy(collision.gameObject); // 음식 오브젝트 제거
        }

        // 충돌을 통해 플레이어와 몬스터를 탐지
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerDetected();
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            OnMonsterDetected();
        }


        GameObject otherAnimal = collision.gameObject;
        AddToRecentAnimals(otherAnimal);

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    //같은 종의 동물이 10회 이상 충돌했을때 동물을 복사(번식) 하기 위해 확인하는 작업
    protected virtual void AddToRecentAnimals(GameObject animal)
    {
        // Baby가 포함된 오브젝트는 무시
        if (animal.name.Contains("Baby")) {
            return;
        }

        int animalId = animal.GetInstanceID();

        // recentAnimals 큐의 크기가 10 이상인지 확인
        if (recentAnimals.Count >= 10)
        {
            // 큐에서 가장 오래된 동물을 제거하고, 그 동물의 충돌 횟수를 감소시킴
            var removedAnimalId = recentAnimals.Dequeue();
            if (animalCount.ContainsKey(removedAnimalId))
            {
                animalCount[removedAnimalId]--;
                // 해당 동물의 충돌 횟수가 0 이하이면 딕셔너리에서 제거
                if (animalCount[removedAnimalId] <= 0)
                {
                    animalCount.Remove(removedAnimalId);
                }
            }
        }
        // 큐에 새로운 동물 추가
        recentAnimals.Enqueue(animalId);
        // 딕셔너리에 동물이 이미 있는지 확인
        if (animalCount.ContainsKey(animalId))
        {
            // 이미 있다면, 해당 동물의 충돌 횟수를 증가시킴
            animalCount[animalId]++;
        }
        else
        {
            // 없다면, 해당 동물을 추가하고 충돌 횟수를 1로 설정
            animalCount[animalId] = 1;
        }
    }

    private void SpawnNewAnimal()
    {
        // 현재 오브젝트의 크기와 성인 프리팹의 크기를 비교(성인만 복사(번식)할 수 있도록)
        if (transform.localScale == adultPrefab.transform.localScale)
        {
            // 현재 오브젝트를 복제
            GameObject newAnimal = Instantiate(gameObject, transform.position, transform.rotation);
            // 복제된 오브젝트의 크기를 1/2으로 설정
            newAnimal.transform.localScale = transform.localScale / 2;

            // 복제된 오브젝트의 충돌 카운트를 초기화
            Animal tracker = newAnimal.GetComponent<Animal>();
            if (tracker != null)
            {
                tracker.recentAnimals.Clear();
                tracker.animalCount.Clear();
            }
            // 복제된 오브젝트의 이름에 "Baby" 추가 (이미 "Baby"가 포함된 경우 추가하지 않음)
            if (!newAnimal.name.Contains("Baby")) {
                newAnimal.name = gameObject.name + "_Baby";
            }

            // 복제된 오브젝트에 "Animals" 태그 추가
            newAnimal.tag = "Animals";

            // 쿨타임 설정
            StartCoroutine(SpawnCooldown());
        }
    }

    protected virtual IEnumerator SpawnCooldown()
    {
        canSpawn = false; // 스폰 금지
        yield return new WaitForSeconds(spawnCooldown); // 쿨타임 대기
        // 충돌 횟수와 큐 초기화
        recentAnimals.Clear();
        animalCount.Clear();
        canSpawn = true; // 스폰 허용
    }

    private void GrowUp()
    {

        if (isAdult) return;// 이미 성인 상태이면 종료

        // 성인 상태로 성장
        GameObject newAdult = Instantiate(adultPrefab, transform.position, transform.rotation);
        newAdult.transform.localScale = transform.localScale * 2;

        // 성장한 오브젝트의 이름에서 "Baby" 제거
        newAdult.name = gameObject.name.Replace("_Baby", "");

        // "Animals" 태그 추가
        newAdult.tag = "Animals";

        // 새로운 성체 오브젝트 초기화
        Animal newAnimalComponent = newAdult.GetComponent<Animal>();
        if (newAnimalComponent != null)
        {
            newAnimalComponent.isAdult = true;
            newAnimalComponent.growthTimeGaze = 0; // 성체가 된 후 성장 타이머 초기화
            newAnimalComponent.isFull = false; // 성장 후 배부름 상태 초기화
        }
        // 기존 오브젝트 삭제
        Destroy(gameObject);
        Debug.Log($"{newAdult.name}가 성인으로 성장했습니다.");
    }

    private void Eat(string foodType, int hungerIncrease)
    {
        // 예상 hungerLevel이 maxHungerLevel을 초과하면 먹지 않음
        if (hungerLevel + hungerIncrease > maxHungerLevel)
        {
            Debug.Log($"{name}가 {foodType}을 먹을 수 없습니다. 배고픔 게이지가 최대치를 초과합니다.");
            return;
        }
        // 배고픔 레벨이 최대치 이상이면 먹지 않음
        if (hungerLevel >= maxHungerLevel)
        {
            Debug.Log($"{name}가 {foodType}을 먹을 수 없습니다. 배고픔 게이지: {hungerLevel} (최대치)");
            return;
        }
        hungerLevel += hungerIncrease;

        // Animal_Food를 먹을 때 추가 처리
        if (foodType == "Animal_Food")
        {
            if (!isOvergrowth && !name.Contains("Baby") && !isTamed) // Baby가 포함되지 않고, 과성장이 아니고, 조련되지 않은 상태일 때
            {
                foodGivenCount++;
                Debug.Log($"{name}가 Animal_Food를 먹었습니다. foodGivenCount: {foodGivenCount}");

                if (foodGivenCount >= OvergrowthThreshold) // 일정 횟수에 도달하면 isTamed 상태로 변경
                {
                    isTamed = true;
                    TamedMark();
                    Debug.Log($"{name}가 조련됐습니다!");
                    foodGivenCount = 0;
                }
            }
            else if (isTamed && !name.Contains("Baby")) // 조련된 상태이고 새끼가 아닐 때
            {
                foodGivenCount++;
                Debug.Log($"{name}foodGivenCount: {foodGivenCount}");

                if (foodGivenCount >= OvergrowthThreshold) // 일정 횟수에 도달하면 isOvergrowth 상태로 변경
                {
                    isOvergrowth = true;
                    Becomeovergrowth();
                    Debug.Log($"{name}가 과성장 상태가 되었습니다!");
                }
            }
        }

        // 배고픔 상태 관리
        if (hungerLevel > 6)
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
            yield return new WaitForSeconds(60f);

            if (hungerLevel > 0)
            {
                hungerLevel--;
            }

            // 배고픔 상태 관리
            if (hungerLevel > 6)
            {
                isHungry = false;
                isFull = true;
                currentSpeed = walkSpeed; // 배부름 상태에서는 기본 속도
            }
            else
            {
                isHungry = true;
                isFull = false;

                if (hungerLevel <= 3)
                {
                    currentSpeed = walkSpeed + speedIncrease; // 배고픔 상태에서는 속도 증가
                }
                else
                {
                    currentSpeed = walkSpeed; // 기본 속도
                }
            }
        }
    }

    private void Walk()
    {
        MoveTowardsTarget(walkSpeed); // 목표 위치로 걷기 속도로 이동합니다.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // 목표 위치에 도달했는지 확인합니다.
        {
            ChangeState(State.Idle); // Idle 상태로 변경
        }
    }

    private void Run()
    {
        MoveTowardsTarget(runSpeed); // 목표 위치로 달리기 속도로 이동합니다.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // 목표 위치에 도달했는지 확인합니다.
        {
            ChangeState(State.Idle); // Idle 상태로 변경
        }
    }

    private void MoveTowardsTarget(float speed) //몬스터를 targetPosition으로 이동시키는 기능
    {

        // targetPosition과 현재 위치의 차이를 구하고, 방향 벡터로 변환
        Vector3 direction = (targetPosition - transform.position).normalized;

        // direction 벡터를 사용하여 새로운 위치를 계산
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // 몬스터의 현재 위치를 newPosition으로 업데이트
        rb.MovePosition(newPosition);
        //Rigidbody를 사용하여 객체를 이동시키는 방법입니다. 이는 물리 엔진을 통해 이동을 처리하므로, 충돌 및 기타 물리적 상호작용을 고려하여 객체를 이동시킵니다. 

        // 몬스터가 이동할 때 바라보는 방향을 갱신
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        randomDirection.y = 0; // y축 고정
        return randomDirection;
    }

    protected virtual void OnPlayerDetected()
    {
        // 플레이어를 발견했을 때의 기본 동작
        // Debug.Log("Animal: OnPlayerDetected 호출됨");
        if (activeEffect == null || activeEffect.tag != "Shock")
        {
            StartCoroutine(DisplayHeartAndRun());
        }
    }

    protected virtual void OnMonsterDetected()
    {
        // Debug.Log("Animal: OnMonsterDetected 호출됨");
        if (activeEffect == null || activeEffect.tag != "Heart")
        {
            StartCoroutine(DisplayShockAndRun());
        }
    }

    private IEnumerator DisplayHeartAndRun()
    {
        ClearActiveEffect(); // 기존 효과 제거

        activeEffect = Instantiate(heartObjectPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        activeEffect.transform.SetParent(transform);
        activeEffect.tag = "Heart";

        ChangeState(State.Idle);
        yield return new WaitForSeconds(2f);

        transform.Rotate(0, 180, 0);

        ChangeState(State.Run);

        ClearActiveEffect(); // 효과 제거
    }

    private IEnumerator DisplayShockAndRun()
    {
        ClearActiveEffect(); // 기존 효과 제거

        activeEffect = Instantiate(shockObjectPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        activeEffect.transform.SetParent(transform);
        activeEffect.tag = "Shock";

        ChangeState(State.Idle);
        yield return new WaitForSeconds(2f);

        transform.Rotate(0, 180, 0);

        ChangeState(State.Run);
        Debug.Log($"도망도망.");
        ClearActiveEffect(); // 효과 제거
    }

    private void ClearActiveEffect()
    {
        if (activeEffect != null)
        {
            Destroy(activeEffect);
            activeEffect = null;
        }
    }

    //걷거나 달리기 상태일 때 랜덤 목적지로 변경되는 이유는 동물이 정해진 패턴 없이 무작위로 배회하거나 달리게 하기 위해서.
    protected virtual void ChangeState(State newState)
    {

        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
        }
        currentState = newState;

        switch (currentState)
        {
            case State.Walk:
                targetPosition = GetRandomPosition(); // 걷기 상태로 변경될 때 새로운 목표 위치를 설정합니다.
                stateCoroutine = StartCoroutine(StateDuration(State.Walk, Random.Range(minWalkTime, maxWalkTime))); // 걷기 상태의 지속 시간을 설정합니다.
                break;
            case State.Jump:
                if (isGrounded)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    isGrounded = false; // 점프를 시작했음을 표시
                }
                stateCoroutine = StartCoroutine(StateDuration(State.Jump, 3f));
                break;
            case State.Idle:
                animator.Play("Idle");
                stateCoroutine = StartCoroutine(StateDuration(State.Idle, Random.Range(5f, 10f))); // 멈춰 있는 상태의 지속 시간을 설정합니다.
                break;
            case State.Run:
                stateCoroutine = StartCoroutine(StateDuration(State.Run, Random.Range(minWalkTime, maxWalkTime)));
                break;
            default:
                stateCoroutine = StartCoroutine(StateDuration(State.Idle, Random.Range(4f, 10f)));
                break;
        }
    }

    protected virtual State GetRandomState()
    {
        int random = Random.Range(0, 100);
        if (random < 25)
        {
            return State.Jump; // 25% 확률로 점프 상태로 전환
        }
        else if (random < 50)
        {
            return State.Run; // 25% 확률로 달리기 상태로 전환
        }
        else if (random < 75)
        {
            return State.Idle; // 25% 확률로 대기 상태로 전환
        }
        else
        {
            return State.Walk; // 25% 확률로 걷기 상태로 전환
        }
    }

    protected virtual IEnumerator IdleThenWander()
    {
        yield return new WaitForSeconds(Random.Range(2, 5));
        ChangeState(State.Walk);
    }

    private IEnumerator StateDuration(State state, float duration)
    {
        yield return new WaitForSeconds(duration);
        ChangeState(GetRandomState());
    }

    public override void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log($"{name}은(는) 무적 상태입니다. 데미지를 받지 않습니다.");
            return;
        }

        base.TakeDamage(damage); // base.TakeDamage를 호출하여 Health 프로퍼티를 통해 값을 변경합니다.

        StartCoroutine(HandleDamage(damage)); // 데미지를 처리하는 코루틴 호출
    }

    private IEnumerator HandleDamage(int damage) //데미지 입은 방향을 쳐다보고, 반대로 돌고, 달리기로 
    {
        // 데미지를 입은 방향으로 head 회전
        Vector3 damageDirection = (transform.position - Camera.main.transform.position).normalized; // 예를 들어, 카메라 위치를 기준으로 데미지 방향 계산
        head.rotation = Quaternion.LookRotation(damageDirection);

        yield return new WaitForSeconds(1f); // 1초 대기

        // 180도 회전하고 Run 상태로 전환
        transform.Rotate(0, 180, 0);
        ChangeState(State.Run);

        StartCoroutine(DisplayShockAndRun()); // 데미지를 입었을 때 DisplayShockAndRun 코루틴 호출
    }

    protected override void Die()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        int meatCount = 1;
        string animalName = gameObject.name;

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

        if (animalName.Contains("Baby"))
        {
            meatCount = Mathf.CeilToInt(meatCount / 2.0f);
        }

        // overgrowth 상태일 경우 고기 생성 수 두 배
        if (isOvergrowth)
        {
            meatCount *= 2;
        }

        for (int i = 0; i < meatCount; i++)
        {
            Vector3 spawnPosition = position + new Vector3(i * 0.5f, 2, 0);
            Instantiate(beafPrefab, spawnPosition, rotation);
        // 고기 생성 후 일정 시간 동안 무적 상태로 설정
            SetInvincible(2.0f);
        }

        StartCoroutine(OnDie()); // 바로 OnDie 코루틴 호출
    }

    public void SetInvincible(float duration)
    {
        StartCoroutine(InvincibilityCoroutine(duration));
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    private void TamedMark()
    {
        gameObject.name = "Tamed_" + gameObject.name;
    }

    private void Becomeovergrowth()
    {
        // 과성장 프리팹을 생성
        Instantiate(overgrowthPrefab, transform.position, transform.rotation);
        // 기존 오브젝트 파괴
        Destroy(gameObject);
    }
}
