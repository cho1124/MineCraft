using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    // 성장 관련 변수들
    public GameObject adultPrefab; //  성인 상태의 프리팹
    public GameObject babyPrefab; // 새끼 상태의 프리팹
    private bool isAdult; // 성인 상태인지 여부
    private float growthTime = 60; // 성장 기준 시간 
    private float growthTimeGaze = 0; // 배부름총족이후흐르는시간 

    // 배고픔 관련 변수들
    public int hungerLevel = 6; // 배고픔 게이지
    private const int maxHungerLevel = 10;
    private bool isHungry = false; // 배고픔 상태
    private bool isFull = false; // 배부름 상태
    private float speedIncrease = 1f; // 추가 이동 속도

    // 이동 속도 관련 변수들
    public float baseSpeed; // 기본 이동 속도, 인스펙터에서 설정
    private float currentSpeed; // 현재 이동 속도

    // 개체 복사 관련 변수들
    private bool canSpawn = true; // 개체 복사 쿨타임 플래그
    private float spawnCooldown = 30f; // 쿨타임 시간
    private const int collisionThreshold = 7; // 충돌 임계값
    protected Queue<GameObject> recentAnimals = new Queue<GameObject>(); // 최근 탐색된 10개의 개체를 저장할 큐
    protected Dictionary<GameObject, int> animalCount = new Dictionary<GameObject, int>(); // 탐색된 개체의 탐색 횟수를 저장할 딕셔너리

    // 컴포넌트 관련 변수들
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Rigidbody rb;
    public DynamicNavMesh dynamicNavMesh;

    // 탐지 관련 변수들
    public float detectionDistance = 3f;
    public int detectionAngle = 45;
    public int detectionRays = 5;
    public float wanderRadius = 30f;
    private float defaultDetectionDistance; // 기본 탐지 거리

    // 플레이어 탐지 관련 변수들
    protected bool canDetectPlayer = true;
    protected float playerDetectionCooldown = 20f;

    // 상태 관련 변수들
    protected enum State { Wander, Jump, Idle, Run, DoubleJump, Follow }
    protected State currentState;

    // 위치 변화 감지 관련 변수들
    private Vector3 lastPosition;
    private float idleTime;
    private float idleTimeLimit = 10f; // 10초 동안 위치가 변하지 않으면 이동
    private const float positionThreshold = 3f; // 위치 변화 허용 범위

    protected virtual void Start() {
        //컴포넌트 초기화
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Collider col = GetComponent<Collider>();

        // NavMesh 위에 있는지 확인
        if (!agent.isOnNavMesh)
        {
            MoveToNearestNavMesh();
        }

        // 성장 관련 초기 설정
        if (gameObject.name.Contains("Baby")) {
            isAdult = false; // 새끼상태
        }
        else {
            isAdult = true; // 성인 상태
        }

        // 초기 이동 속도 설정
        currentSpeed = baseSpeed = 2;

        // 배고픔 감소 루틴 시작
        StartCoroutine(HungerRoutine());

        // 오브젝트의 크기에 맞게 NavMeshAgent의 radius 설정
        if (col != null) {
            agent.radius = Mathf.Max(col.bounds.size.x, col.bounds.size.z) / 2;
            //agent.radius가 반지름을 가리키는거기때문에 /2 함
            agent.height = col.bounds.size.y;
        }
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        // 탐지 거리 초기화
        defaultDetectionDistance = detectionDistance;

        // 초기 상태 설정
        ChangeState(State.Wander);

        // 위치 변화 감지 초기화
        lastPosition = transform.position;
        idleTime = 0f;
    }

    protected virtual void Update() {

        // y축 높이 체크
        if (transform.position.y >= 2)
        {
            MoveToNearestNavMesh();
        }

        //성장 관련 업데이트
        if (!isAdult && isFull) {
            growthTimeGaze += Time.deltaTime;
            if (growthTimeGaze >= growthTime) {
                GrowUp();
            }
        }

        // 배고픔 상태에 따른 성장 타이머 초기화
        if (!isFull) {
            growthTimeGaze = 0f;
        }

        if (hungerLevel <= 3) {
            SearchForFood();
        }

        // NavMesh 상에 있는지 확인
        if (!agent.isOnNavMesh) {
            Debug.LogWarning("Agent 가 NavMesh위에 없습니다.!");
            MoveToNearestNavMesh();
            return;
        }

      //  // 실시간 맵 변동을 반영하여 NavMesh 업데이트
      //  if (dynamicNavMesh != null) {
      //      dynamicNavMesh.UpdateNavMesh();
      //  }

        if (DetectPlayer()) {
            OnPlayerDetected();
            return;
        }

        // 현재 상태에 따라 행동 수행
        switch (currentState) {
            case State.Wander:
                if (!agent.hasPath) {
                    SetRandomDestination();
                }
                break;
            case State.Jump:
                break;
            case State.Idle:
                break;
            case State.Run:
                break;
        }

        //목적지 도착시 상태 변경
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) {
            ChangeState(GetRandomState());
        }

        // 위치 변화 감지
        CheckIdleTime();
    }

    private void CheckIdleTime() {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (distanceMoved < positionThreshold) {
            idleTime += Time.deltaTime;
        }
        else {
            idleTime = 0f;
            lastPosition = transform.position;
        }

        if (idleTime >= idleTimeLimit) {
            SetRandomDestination();
            Debug.Log($"{name}가 10초동안 위치 변화가 없으므로 랜덤목적지를 변경합니다.");
            idleTime = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log($"{name}과 {collision.gameObject.name}이 충돌했습니다.");
        // 같은 컴포넌트를 가진 오브젝트와의 충돌인지 확인
        if (collision.gameObject.GetComponent<Animal>() != null) {
            AddToRecentAnimals(collision.gameObject);

            if (animalCount[collision.gameObject] >= collisionThreshold) {
                SpawnNewAnimal();
                animalCount[collision.gameObject] = 0; // 충돌 횟수 초기화
            }
        }

        // Food 태그가 붙은 오브젝트를 먹는 처리
        if (collision.gameObject.CompareTag("Food") && !isFull) {
            Eat();
            Destroy(collision.gameObject); // 음식 오브젝트 제거
        }
    }

    protected virtual void AddToRecentAnimals(GameObject animal) {
        if (recentAnimals.Count >= 10) {
            var removedAnimal = recentAnimals.Dequeue();
            animalCount[removedAnimal]--;
            if (animalCount[removedAnimal] <= 0) {
                animalCount.Remove(removedAnimal);
            }
        }

        recentAnimals.Enqueue(animal);
        if (animalCount.ContainsKey(animal)) {
            animalCount[animal]++;
        }
        else {
            animalCount[animal] = 1;
        }
    }

    private void SpawnNewAnimal() {
        // 현재 오브젝트의 크기와 프리팹의 크기를 비교
        if (transform.localScale == adultPrefab.transform.localScale) {
            // 현재 오브젝트를 복제
            GameObject newAnimal = Instantiate(gameObject, transform.position, transform.rotation);
            // 복제된 오브젝트의 크기를 1/2으로 설정
            newAnimal.transform.localScale = transform.localScale / 2;

            // 복제된 오브젝트의 충돌 카운트를 초기화
            Animal tracker = newAnimal.GetComponent<Animal>();
            if (tracker != null) {
                tracker.recentAnimals.Clear();
                tracker.animalCount.Clear();
            }

            // 복제된 오브젝트의 이름에 "Baby" 추가
            newAnimal.name = gameObject.name + "_Baby";

            // 복제된 오브젝트에 "Animals" 태그 추가
            newAnimal.tag = "Animals";

            // 쿨타임 설정
            StartCoroutine(SpawnCooldown());
        }
    }

    protected virtual IEnumerator SpawnCooldown() {
        canSpawn = false; // 스폰 금지
        yield return new WaitForSeconds(spawnCooldown); // 쿨타임 대기
        canSpawn = true; // 스폰 허용
    }

    private void GrowUp() {

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

    private void Eat() {
        if (hungerLevel < maxHungerLevel) {
            hungerLevel++;
            Debug.Log($"{name}가 사료를 먹었습니다. 배고픔 게이지: {hungerLevel}");
        }

        // 배고픔 상태 관리
        if (hungerLevel > 5) {
            isHungry = false;
            isFull = true;
            detectionDistance = defaultDetectionDistance; // 배부름 상태일 때 탐지 거리 복원
        }
        else {
            isHungry = true;
            isFull = false;
            detectionDistance = defaultDetectionDistance * 2; // 배고픔 상태일 때 탐지 거리 두 배
        }
    }

    private IEnumerator HungerRoutine() {
        while (true) {
            yield return new WaitForSeconds(20f);

            if (hungerLevel > 0) {
                hungerLevel--;
                Debug.Log($"{name}의 배고픔 게이지 감소: {hungerLevel}");
            }

            // 배고픔 상태 관리
            if (hungerLevel > 5) {
                isHungry = false;
                isFull = true;
                detectionDistance = defaultDetectionDistance; // 배부름 상태일 때 탐지 거리 복원
                currentSpeed = baseSpeed; // 배부름 상태에서는 기본 속도
            }
            else {
                isHungry = true;
                isFull = false;
                detectionDistance = defaultDetectionDistance * 2; // 배고픔 상태일 때 탐지 거리 두 배

                if (hungerLevel <= 3) {
                    currentSpeed = baseSpeed + speedIncrease; // 배고픔 상태에서는 속도 증가
                }
                else {
                    currentSpeed = baseSpeed; // 기본 속도
                }
            }
        }
    }

    private void SearchForFood() {
        for (int i = 0; i < detectionRays; i++) {
            float angle = (-detectionAngle / 2) + (detectionAngle / (detectionRays - 1)) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, detectionDistance)) {
                if (hitInfo.collider.CompareTag("Food")) {
                    MoveTowards(hitInfo.transform.position);
                    break;
                }
            }
        }
    }

    private void MoveTowards(Vector3 targetPosition) {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(targetPosition); // NavMeshAgent를 사용하여 이동
        }
    }
    protected virtual void MoveToNearestNavMesh() {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) {
            agent.Warp(hit.position);
            ChangeState(State.Wander);
        }
    }

    protected virtual bool DetectPlayer() {
        for (int i = 0; i < detectionRays; i++) {
            float angle = (-detectionAngle / 2) + (detectionAngle / (detectionRays - 1)) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, detectionDistance)) {
                if (hitInfo.collider.CompareTag("Player")) {
                    Debug.Log("플레이어를 발견했습니다!");
                    return true;
                }
            }
        }
        return false;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        // Y축으로 1만큼 위의 위치에서 레이캐스트 시작
        Vector3 startPosition = transform.position + Vector3.up * 3;

        for (int i = 0; i < detectionRays; i++) {
            float angle = (-detectionAngle / 2) + (detectionAngle / (detectionRays - 1)) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Gizmos.DrawRay(transform.position, direction * detectionDistance);
        }
    }

    protected virtual void OnPlayerDetected() {
        // 플레이어를 발견했을 때의 기본 동작
        Debug.Log("Animal: OnPlayerDetected 호출됨");
    }

    protected IEnumerator PlayerDetectionCooldown() {
        yield return new WaitForSeconds(playerDetectionCooldown);
        canDetectPlayer = true; // 쿨타임 종료 후 탐지 활성화
    }

    protected virtual void ChangeState(State newState) {
        currentState = newState;

        switch (currentState) {
            case State.Wander:
                agent.speed = baseSpeed;
                animator.SetInteger("Walk", 1);
                SetRandomDestination();
                break;
            case State.Jump:
                animator.Play("Jump");
                StartCoroutine(JumpThenIdle());
                break;
            case State.Idle:
                agent.ResetPath();
                animator.Play("Idle");
                StartCoroutine(IdleThenWander());
                break;
            case State.Run:
                agent.speed = baseSpeed * 2;
                animator.SetInteger("Walk", 1);
                SetRandomDestination();
                break;
        }
    }

    protected virtual State GetRandomState() {
        int random = Random.Range(0, 100);
        if (random < 25) {
            return State.Jump; // 25% 확률로 점프 상태로 전환
        }
        else if (random < 50) {
            return State.Run; // 25% 확률로 달리기 상태로 전환
        }
        else if (random < 75) {
            return State.Idle; // 25% 확률로 대기 상태로 전환
        }
        else {
            return State.Wander; // 25% 확률로 배회 상태로 전환
        }
    }

    protected virtual void SetRandomDestination() {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas)) {
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.LogWarning("SetRandomDestination 호출 전에 NavMesh 위에 에이전트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("Failed to find a valid NavMesh position!");
        }
    }

    protected virtual IEnumerator JumpThenIdle() {

        // NavMeshAgent 비활성화
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.isStopped = true;

        // Rigidbody 사용하여 점프
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);

        // 점프 지속 시간 동안 대기
        yield return new WaitForSeconds(3f);

        // Rigidbody를 다시 비활성화하고 NavMeshAgent를 활성화
        rb.isKinematic = true;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;

        // 상태를 Idle로 변경
        ChangeState(State.Idle);
    }

    protected virtual IEnumerator IdleThenWander() {
        yield return new WaitForSeconds(Random.Range(2, 5));
        ChangeState(State.Wander);
    }
}
