using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : Entity
{
    //동물도 자유롭게 구현하면 될거에용

    /*
      ★현재 문제점 
     -   네비메쉬와 리지드바디 충돌문제로 player 태그를 인식하면 하늘로 솟구쳐오르는 문제가 있음
     -   갑자기 스케이트 타듯 미끄러지는 듯한 움직임이 있음
     -   다이나믹네비메쉬(네비메쉬를 계속 업데이트하여 변화를 탐지하는 기능) 키면 너무 느려져서 꺼둠. 
         -> 동물들이 지면의 변화를 인지할수가 없음 -> 3f이내의 장소에서 10f동안 변화가 없으면 네비메쉬상 목적지를
             바꾸게 해둠(방향을 바꿀 수 있게)


     => 점프모션만 하고, y축 이동 제외
     
     */

    // 성장 관련 변수들
    public GameObject adultPrefab; //  성인 상태의 프리팹
    public GameObject babyPrefab; // 새끼 상태의 프리팹
    private bool isAdult; // 성인 상태인지 여부
    private float growthTime = 60; // 성장 기준 시간 
    private float growthTimeGaze = 0; // 배부름총족이후흐르는시간 :성장 타이머

    // 배고픔 관련 변수들
    public int hungerLevel = 7; // 배고픔 게이지
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
    private const int collisionThreshold = 5; // 충돌 임계값
    protected Queue<int> recentAnimals = new Queue<int>(); // 최근 탐색된 10개의 개체를 저장할 큐
    protected Dictionary<int, int> animalCount = new Dictionary<int, int>(); // 탐색된 개체의 탐색 횟수를 저장할 딕셔너리

    // 컴포넌트 관련 변수들
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    public DynamicNavMesh dynamicNavMesh;//동적인 NavMesh 업데이트를 관리하는 컴포넌트입니다.
    public GameObject heartObjectPrefab; // 하트 오브젝트 프리팹
    public GameObject shockObjectPrefab; // 충격 오브젝트 프리팹

    // 탐지 관련 변수들
    public float detectionDistance = 3f; //동물이 탐지할 수 있는 최대 거리입니다.
    public int detectionAngle = 45; //동물이 탐지할 수 있는 각도 범위입니다.
    public int detectionRays = 5; //동물이 탐지할 때 사용하는 레이의 수입니다.
    public float wanderRadius = 30f; //동물이 배회할 수 있는 최대 반경입니다.
    private float defaultDetectionDistance; // 기본 탐지 거리

    // 플레이어 탐지 관련 변수들
    protected bool canDetectPlayer = true;
    protected float playerDetectionCooldown = 20f; 
    //플레이어를 중복으로 감지하면 플레이어 감지시 행동이 과도하게 이루어져서 쿨타임을 만들었음

    // 상태 관련 변수들
    protected enum State { Wander, Jump, Idle, Run, DoubleJump, Follow }
    protected State currentState;

    // 위치 변화 감지 관련 변수들
    private Vector3 lastPosition; //동물이 배회할 수 있는 최대 반경입니다.
    private float idleTime; //대기 시간을 추적하는 변수입니다.
    private float idleTimeLimit = 10f; // 10초 동안 위치가 변하지 않으면 이동
    private const float positionThreshold = 3f; // 위치 변화 허용 범위(10초동안 이 범위 안에만 있으면 네비메쉬목적지변경)

    private GameObject activeEffect;

    protected override void Start() {

        base.Start();

        //컴포넌트 초기화
        agent = GetComponent<NavMeshAgent>();
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
        //동물들이 움직이지 않고 한곳에 멈춰있는지 확인하기 위한 대기시간 초기화
        idleTime = 0f;
    }

    protected virtual void Update() {

        // y축 높이 체크
        if (transform.position.y >= 2|| transform.position.y<-5)
        {
            MoveToNearestNavMesh();
        }

        //성장 관련 업데이트
        //성인이 아니고 배부름상태가 growthTimeGaze 만큼 유지되면 성인으로 자람
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

        // NavMesh 상에 있는지 확인
        if (!agent.isOnNavMesh) {
            Debug.LogWarning($"Agent{agent.name} 가 NavMesh위에 없습니다.!");
            MoveToNearestNavMesh();
            return;
        }

      //  // 실시간 맵 변동을 반영하여 NavMesh 업데이트
      //  if (dynamicNavMesh != null) {
      //      dynamicNavMesh.UpdateNavMesh();
      //  }

        if (Detect()) {
          //  OnPlayerDetected();
          //  OnMonsterDetected();
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
            SetRandomDestination();
        }

        // 위치 변화 감지
        CheckIdleTime();
    }

    private void CheckIdleTime() {
        // 현재 위치와 마지막 위치 사이의 거리를 계산합니다.
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        // 이동한 거리가 positionThreshold 이하인 경우 idleTime을 증가시킵니다.
        if (distanceMoved < positionThreshold) {
            idleTime += Time.deltaTime;
        }
        // 이동한 거리가 positionThreshold를 초과한 경우 idleTime을 초기화하고 마지막 위치를 업데이트합니다.
        else
        {
            idleTime = 0f;
            lastPosition = transform.position;
        }
        // idleTime이 idleTimeLimit을 초과한 경우 새로운 랜덤 목적지를 설정합니다.
        if (idleTime >= idleTimeLimit) {
            SetRandomDestination();
            Debug.Log($"{name}가 10초동안 위치 변화가 없으므로 랜덤목적지를 변경합니다.");
            idleTime = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision) {

        // 자신과 같은 컴포넌트를 가진 오브젝트와의 충돌인지 확인
        if (collision.gameObject.GetComponent(GetType()) != null) {
            //최근 충돌한 동물 목록에 추가
            AddToRecentAnimals(collision.gameObject);
            int collisionAnimalId = collision.gameObject.GetInstanceID();
            if (animalCount.ContainsKey(collisionAnimalId) && animalCount[collisionAnimalId] >= collisionThreshold)
            {
                SpawnNewAnimal();
                animalCount[collisionAnimalId] = 0;
            }
        }

        // Food 태그가 붙은 오브젝트를 먹는 처리
        if (collision.gameObject.CompareTag("Food") && !isFull) {
            Eat();
            Destroy(collision.gameObject); // 음식 오브젝트 제거
        }

        GameObject otherAnimal = collision.gameObject;
        AddToRecentAnimals(otherAnimal);
    }

    //같은 종의 동물이 5회 이상 충돌했을때 동물을 복사(번식) 하기 위해 확인하는 작업
    protected virtual void AddToRecentAnimals(GameObject animal) {

        int animalId = animal.GetInstanceID();

        // recentAnimals 큐의 크기가 10 이상인지 확인
        if (recentAnimals.Count >= 10) {
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
        if (animalCount.ContainsKey(animalId)) {
            // 이미 있다면, 해당 동물의 충돌 횟수를 증가시킴
            animalCount[animalId]++;
        }
        else {
            // 없다면, 해당 동물을 추가하고 충돌 횟수를 1로 설정
            animalCount[animalId] = 1;
        }
    }

    private void SpawnNewAnimal() {
        // 현재 오브젝트의 크기와 성인 프리팹의 크기를 비교(성인만 복사(번식)할 수 있도록)
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
            Debug.Log($"{newAnimal.name} 태어남! ");

            // 복제된 오브젝트에 "Animals" 태그 추가
            newAnimal.tag = "Animals";

            // 쿨타임 설정
            StartCoroutine(SpawnCooldown());
        }
    }

    protected virtual IEnumerator SpawnCooldown() {
        canSpawn = false; // 스폰 금지
        yield return new WaitForSeconds(spawnCooldown); // 쿨타임 대기
        // 충돌 횟수와 큐 초기화
        recentAnimals.Clear();
        animalCount.Clear();
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
        if (hungerLevel > 6) {
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
            yield return new WaitForSeconds(60f);

            if (hungerLevel > 0) {
                hungerLevel--;
            }

            // 배고픔 상태 관리
            if (hungerLevel > 6) {
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
                    SearchForFood();
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
            MoveTowards(hit.position);
            ChangeState(State.Wander);
        }
    }

    protected virtual bool Detect()
    {
        bool detected = false;

        for (int i = 0; i < detectionRays; i++)
        {
            float angle = (-detectionAngle / 2) + (detectionAngle / (detectionRays - 1)) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, detectionDistance))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                  //  Debug.Log("플레이어를 발견했습니다!");
                    if (activeEffect == null || activeEffect.tag != "Heart")
                    {
                        OnPlayerDetected();
                        detected = true;
                    }
                }
                else if (hitInfo.collider.CompareTag("Monster"))
                {
                  //  Debug.Log("몬스터를 발견했습니다!");
                    if (activeEffect == null || activeEffect.tag != "Shock")
                    {
                        OnMonsterDetected();
                        detected = true;
                    }
                }
            }
        }

        return detected;
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

    protected IEnumerator PlayerDetectionCooldown() {
        yield return new WaitForSeconds(playerDetectionCooldown);
        canDetectPlayer = true; // 쿨타임 종료 후 탐지 활성화
    }

    //걷거나 달리기 상태일 때 랜덤 목적지로 변경되는 이유는 동물이 정해진 패턴 없이 무작위로 배회하거나 달리게 하기 위해서.
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
              //  StartCoroutine(JumpThenIdle());
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

  //  protected virtual IEnumerator JumpThenIdle() {
  //
  //     // // NavMeshAgent 비활성화
  //     // agent.updatePosition = false;
  //     // agent.updateRotation = false;
  //     // agent.isStopped = true;
  //
  //      // Rigidbody 사용하여 점프
  //    //  rb.isKinematic = false;
  //    //  rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
  //
  //    //  // 점프 지속 시간 동안 대기
  //    //  yield return new WaitForSeconds(3f);
  //
  //    //  // Rigidbody를 다시 비활성화하고 NavMeshAgent를 활성화
  //    //  rb.isKinematic = true;
  //    //  agent.updatePosition = true;
  //    //  agent.updateRotation = true;
  //    //  agent.isStopped = false;
  //
  //      // 상태를 Idle로 변경
  //      ChangeState(State.Idle);
  //  }

    protected virtual IEnumerator IdleThenWander() {
        yield return new WaitForSeconds(Random.Range(2, 5));
        ChangeState(State.Wander);
    }
}
