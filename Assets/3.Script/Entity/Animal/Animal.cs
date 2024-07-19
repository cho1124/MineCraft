using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : Entity
{
    //������ �����Ӱ� �����ϸ� �ɰſ���

    /*
      ������ ������ 
     -   �׺�޽��� ������ٵ� �浹������ player �±׸� �ν��ϸ� �ϴ÷� �ڱ��Ŀ����� ������ ����
     -   ���ڱ� ������Ʈ Ÿ�� �̲������� ���� �������� ����
     -   ���̳��ͳ׺�޽�(�׺�޽��� ��� ������Ʈ�Ͽ� ��ȭ�� Ž���ϴ� ���) Ű�� �ʹ� �������� ����. 
         -> �������� ������ ��ȭ�� �����Ҽ��� ���� -> 3f�̳��� ��ҿ��� 10f���� ��ȭ�� ������ �׺�޽��� ��������
             �ٲٰ� �ص�(������ �ٲ� �� �ְ�)


     => ������Ǹ� �ϰ�, y�� �̵� ����
     
     */

    // ���� ���� ������
    public GameObject adultPrefab; //  ���� ������ ������
    public GameObject babyPrefab; // ���� ������ ������
    private bool isAdult; // ���� �������� ����
    private float growthTime = 60; // ���� ���� �ð� 
    private float growthTimeGaze = 0; // ��θ����������帣�½ð� :���� Ÿ�̸�

    // ����� ���� ������
    public int hungerLevel = 7; // ����� ������
    private const int maxHungerLevel = 10;
    private bool isHungry = false; // ����� ����
    private bool isFull = false; // ��θ� ����
    private float speedIncrease = 1f; // �߰� �̵� �ӵ�

    // �̵� �ӵ� ���� ������
    public float baseSpeed; // �⺻ �̵� �ӵ�, �ν����Ϳ��� ����
    private float currentSpeed; // ���� �̵� �ӵ�

    // ��ü ���� ���� ������
    private bool canSpawn = true; // ��ü ���� ��Ÿ�� �÷���
    private float spawnCooldown = 30f; // ��Ÿ�� �ð�
    private const int collisionThreshold = 5; // �浹 �Ӱ谪
    protected Queue<int> recentAnimals = new Queue<int>(); // �ֱ� Ž���� 10���� ��ü�� ������ ť
    protected Dictionary<int, int> animalCount = new Dictionary<int, int>(); // Ž���� ��ü�� Ž�� Ƚ���� ������ ��ųʸ�

    // ������Ʈ ���� ������
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    public DynamicNavMesh dynamicNavMesh;//������ NavMesh ������Ʈ�� �����ϴ� ������Ʈ�Դϴ�.
    public GameObject heartObjectPrefab; // ��Ʈ ������Ʈ ������
    public GameObject shockObjectPrefab; // ��� ������Ʈ ������

    // Ž�� ���� ������
    public float detectionDistance = 3f; //������ Ž���� �� �ִ� �ִ� �Ÿ��Դϴ�.
    public int detectionAngle = 45; //������ Ž���� �� �ִ� ���� �����Դϴ�.
    public int detectionRays = 5; //������ Ž���� �� ����ϴ� ������ ���Դϴ�.
    public float wanderRadius = 30f; //������ ��ȸ�� �� �ִ� �ִ� �ݰ��Դϴ�.
    private float defaultDetectionDistance; // �⺻ Ž�� �Ÿ�

    // �÷��̾� Ž�� ���� ������
    protected bool canDetectPlayer = true;
    protected float playerDetectionCooldown = 20f; 
    //�÷��̾ �ߺ����� �����ϸ� �÷��̾� ������ �ൿ�� �����ϰ� �̷������ ��Ÿ���� �������

    // ���� ���� ������
    protected enum State { Wander, Jump, Idle, Run, DoubleJump, Follow }
    protected State currentState;

    // ��ġ ��ȭ ���� ���� ������
    private Vector3 lastPosition; //������ ��ȸ�� �� �ִ� �ִ� �ݰ��Դϴ�.
    private float idleTime; //��� �ð��� �����ϴ� �����Դϴ�.
    private float idleTimeLimit = 10f; // 10�� ���� ��ġ�� ������ ������ �̵�
    private const float positionThreshold = 3f; // ��ġ ��ȭ ��� ����(10�ʵ��� �� ���� �ȿ��� ������ �׺�޽�����������)

    private GameObject activeEffect;

    protected override void Start() {

        base.Start();

        //������Ʈ �ʱ�ȭ
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        Collider col = GetComponent<Collider>();

        // NavMesh ���� �ִ��� Ȯ��
        if (!agent.isOnNavMesh)
        {
            MoveToNearestNavMesh();
        }

        // ���� ���� �ʱ� ����
        if (gameObject.name.Contains("Baby")) {
            isAdult = false; // ��������
        }
        else {
            isAdult = true; // ���� ����
        }

        // �ʱ� �̵� �ӵ� ����
        currentSpeed = baseSpeed = 2;

        // ����� ���� ��ƾ ����
        StartCoroutine(HungerRoutine());

        // ������Ʈ�� ũ�⿡ �°� NavMeshAgent�� radius ����
        if (col != null) {
            agent.radius = Mathf.Max(col.bounds.size.x, col.bounds.size.z) / 2;
            //agent.radius�� �������� ����Ű�°ű⶧���� /2 ��
            agent.height = col.bounds.size.y;
        }
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        // Ž�� �Ÿ� �ʱ�ȭ
        defaultDetectionDistance = detectionDistance;

        // �ʱ� ���� ����
        ChangeState(State.Wander);

        // ��ġ ��ȭ ���� �ʱ�ȭ
        lastPosition = transform.position;
        //�������� �������� �ʰ� �Ѱ��� �����ִ��� Ȯ���ϱ� ���� ���ð� �ʱ�ȭ
        idleTime = 0f;
    }

    protected virtual void Update() {

        // y�� ���� üũ
        if (transform.position.y >= 2|| transform.position.y<-5)
        {
            MoveToNearestNavMesh();
        }

        //���� ���� ������Ʈ
        //������ �ƴϰ� ��θ����°� growthTimeGaze ��ŭ �����Ǹ� �������� �ڶ�
        if (!isAdult && isFull) {
            growthTimeGaze += Time.deltaTime;
            if (growthTimeGaze >= growthTime) {
                GrowUp();
            }
        }

        // ����� ���¿� ���� ���� Ÿ�̸� �ʱ�ȭ
        if (!isFull) {
            growthTimeGaze = 0f;
        }

        // NavMesh �� �ִ��� Ȯ��
        if (!agent.isOnNavMesh) {
            Debug.LogWarning($"Agent{agent.name} �� NavMesh���� �����ϴ�.!");
            MoveToNearestNavMesh();
            return;
        }

      //  // �ǽð� �� ������ �ݿ��Ͽ� NavMesh ������Ʈ
      //  if (dynamicNavMesh != null) {
      //      dynamicNavMesh.UpdateNavMesh();
      //  }

        if (Detect()) {
          //  OnPlayerDetected();
          //  OnMonsterDetected();
            return;
        }

        // ���� ���¿� ���� �ൿ ����
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

        //������ ������ ���� ����
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) {
            ChangeState(GetRandomState());
            SetRandomDestination();
        }

        // ��ġ ��ȭ ����
        CheckIdleTime();
    }

    private void CheckIdleTime() {
        // ���� ��ġ�� ������ ��ġ ������ �Ÿ��� ����մϴ�.
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        // �̵��� �Ÿ��� positionThreshold ������ ��� idleTime�� ������ŵ�ϴ�.
        if (distanceMoved < positionThreshold) {
            idleTime += Time.deltaTime;
        }
        // �̵��� �Ÿ��� positionThreshold�� �ʰ��� ��� idleTime�� �ʱ�ȭ�ϰ� ������ ��ġ�� ������Ʈ�մϴ�.
        else
        {
            idleTime = 0f;
            lastPosition = transform.position;
        }
        // idleTime�� idleTimeLimit�� �ʰ��� ��� ���ο� ���� �������� �����մϴ�.
        if (idleTime >= idleTimeLimit) {
            SetRandomDestination();
            Debug.Log($"{name}�� 10�ʵ��� ��ġ ��ȭ�� �����Ƿ� ������������ �����մϴ�.");
            idleTime = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision) {

        // �ڽŰ� ���� ������Ʈ�� ���� ������Ʈ���� �浹���� Ȯ��
        if (collision.gameObject.GetComponent(GetType()) != null) {
            //�ֱ� �浹�� ���� ��Ͽ� �߰�
            AddToRecentAnimals(collision.gameObject);
            int collisionAnimalId = collision.gameObject.GetInstanceID();
            if (animalCount.ContainsKey(collisionAnimalId) && animalCount[collisionAnimalId] >= collisionThreshold)
            {
                SpawnNewAnimal();
                animalCount[collisionAnimalId] = 0;
            }
        }

        // Food �±װ� ���� ������Ʈ�� �Դ� ó��
        if (collision.gameObject.CompareTag("Food") && !isFull) {
            Eat();
            Destroy(collision.gameObject); // ���� ������Ʈ ����
        }

        GameObject otherAnimal = collision.gameObject;
        AddToRecentAnimals(otherAnimal);
    }

    //���� ���� ������ 5ȸ �̻� �浹������ ������ ����(����) �ϱ� ���� Ȯ���ϴ� �۾�
    protected virtual void AddToRecentAnimals(GameObject animal) {

        int animalId = animal.GetInstanceID();

        // recentAnimals ť�� ũ�Ⱑ 10 �̻����� Ȯ��
        if (recentAnimals.Count >= 10) {
            // ť���� ���� ������ ������ �����ϰ�, �� ������ �浹 Ƚ���� ���ҽ�Ŵ
            var removedAnimalId = recentAnimals.Dequeue();
            if (animalCount.ContainsKey(removedAnimalId))
            {
                animalCount[removedAnimalId]--;
                // �ش� ������ �浹 Ƚ���� 0 �����̸� ��ųʸ����� ����
                if (animalCount[removedAnimalId] <= 0)
                {
                    animalCount.Remove(removedAnimalId);
                }
            }
        }
        // ť�� ���ο� ���� �߰�
        recentAnimals.Enqueue(animalId);
        // ��ųʸ��� ������ �̹� �ִ��� Ȯ��
        if (animalCount.ContainsKey(animalId)) {
            // �̹� �ִٸ�, �ش� ������ �浹 Ƚ���� ������Ŵ
            animalCount[animalId]++;
        }
        else {
            // ���ٸ�, �ش� ������ �߰��ϰ� �浹 Ƚ���� 1�� ����
            animalCount[animalId] = 1;
        }
    }

    private void SpawnNewAnimal() {
        // ���� ������Ʈ�� ũ��� ���� �������� ũ�⸦ ��(���θ� ����(����)�� �� �ֵ���)
        if (transform.localScale == adultPrefab.transform.localScale) {
            // ���� ������Ʈ�� ����
            GameObject newAnimal = Instantiate(gameObject, transform.position, transform.rotation);
            // ������ ������Ʈ�� ũ�⸦ 1/2���� ����
            newAnimal.transform.localScale = transform.localScale / 2;

            // ������ ������Ʈ�� �浹 ī��Ʈ�� �ʱ�ȭ
            Animal tracker = newAnimal.GetComponent<Animal>();
            if (tracker != null) {
                tracker.recentAnimals.Clear();
                tracker.animalCount.Clear();
            }

            // ������ ������Ʈ�� �̸��� "Baby" �߰�
            newAnimal.name = gameObject.name + "_Baby";
            Debug.Log($"{newAnimal.name} �¾! ");

            // ������ ������Ʈ�� "Animals" �±� �߰�
            newAnimal.tag = "Animals";

            // ��Ÿ�� ����
            StartCoroutine(SpawnCooldown());
        }
    }

    protected virtual IEnumerator SpawnCooldown() {
        canSpawn = false; // ���� ����
        yield return new WaitForSeconds(spawnCooldown); // ��Ÿ�� ���
        // �浹 Ƚ���� ť �ʱ�ȭ
        recentAnimals.Clear();
        animalCount.Clear();
        canSpawn = true; // ���� ���
    }

    private void GrowUp() {

        if (isAdult) return;// �̹� ���� �����̸� ����

        // ���� ���·� ����
        GameObject newAdult = Instantiate(adultPrefab, transform.position, transform.rotation);
        newAdult.transform.localScale = transform.localScale * 2;

        // ������ ������Ʈ�� �̸����� "Baby" ����
        newAdult.name = gameObject.name.Replace("_Baby", "");

        // "Animals" �±� �߰�
        newAdult.tag = "Animals";

        // ���ο� ��ü ������Ʈ �ʱ�ȭ
        Animal newAnimalComponent = newAdult.GetComponent<Animal>();
        if (newAnimalComponent != null)
        {
            newAnimalComponent.isAdult = true;
            newAnimalComponent.growthTimeGaze = 0; // ��ü�� �� �� ���� Ÿ�̸� �ʱ�ȭ
            newAnimalComponent.isFull = false; // ���� �� ��θ� ���� �ʱ�ȭ
        }
        // ���� ������Ʈ ����
        Destroy(gameObject);
        Debug.Log($"{newAdult.name}�� �������� �����߽��ϴ�.");
    }

    private void Eat() {
        if (hungerLevel < maxHungerLevel) {
            hungerLevel++;
            Debug.Log($"{name}�� ��Ḧ �Ծ����ϴ�. ����� ������: {hungerLevel}");
        }

        // ����� ���� ����
        if (hungerLevel > 6) {
            isHungry = false;
            isFull = true;
            detectionDistance = defaultDetectionDistance; // ��θ� ������ �� Ž�� �Ÿ� ����
        }
        else {
            isHungry = true;
            isFull = false;
            detectionDistance = defaultDetectionDistance * 2; // ����� ������ �� Ž�� �Ÿ� �� ��
        }
    }

    private IEnumerator HungerRoutine() {
        while (true) {
            yield return new WaitForSeconds(60f);

            if (hungerLevel > 0) {
                hungerLevel--;
            }

            // ����� ���� ����
            if (hungerLevel > 6) {
                isHungry = false;
                isFull = true;
                detectionDistance = defaultDetectionDistance; // ��θ� ������ �� Ž�� �Ÿ� ����
                currentSpeed = baseSpeed; // ��θ� ���¿����� �⺻ �ӵ�
            }
            else {
                isHungry = true;
                isFull = false;
                detectionDistance = defaultDetectionDistance * 2; // ����� ������ �� Ž�� �Ÿ� �� ��

                if (hungerLevel <= 3) {
                    currentSpeed = baseSpeed + speedIncrease; // ����� ���¿����� �ӵ� ����
                    SearchForFood();
                }
                else {
                    currentSpeed = baseSpeed; // �⺻ �ӵ�
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
            agent.SetDestination(targetPosition); // NavMeshAgent�� ����Ͽ� �̵�
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
                  //  Debug.Log("�÷��̾ �߰��߽��ϴ�!");
                    if (activeEffect == null || activeEffect.tag != "Heart")
                    {
                        OnPlayerDetected();
                        detected = true;
                    }
                }
                else if (hitInfo.collider.CompareTag("Monster"))
                {
                  //  Debug.Log("���͸� �߰��߽��ϴ�!");
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

        // Y������ 1��ŭ ���� ��ġ���� ����ĳ��Ʈ ����
        Vector3 startPosition = transform.position + Vector3.up * 3;

        for (int i = 0; i < detectionRays; i++) {
            float angle = (-detectionAngle / 2) + (detectionAngle / (detectionRays - 1)) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Gizmos.DrawRay(transform.position, direction * detectionDistance);
        }
    }

    protected virtual void OnPlayerDetected() {
        // �÷��̾ �߰����� ���� �⺻ ����
       // Debug.Log("Animal: OnPlayerDetected ȣ���");
        if (activeEffect == null || activeEffect.tag != "Shock")
        {
            StartCoroutine(DisplayHeartAndRun());
        }
    }

    protected virtual void OnMonsterDetected()
    {
       // Debug.Log("Animal: OnMonsterDetected ȣ���");
        if (activeEffect == null || activeEffect.tag != "Heart")
        {
            StartCoroutine(DisplayShockAndRun());
        }
    }

    private IEnumerator DisplayHeartAndRun()
    {
        ClearActiveEffect(); // ���� ȿ�� ����

        activeEffect = Instantiate(heartObjectPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        activeEffect.transform.SetParent(transform);
        activeEffect.tag = "Heart";

        ChangeState(State.Idle);
        yield return new WaitForSeconds(2f);

        transform.Rotate(0, 180, 0);

        ChangeState(State.Run);

        ClearActiveEffect(); // ȿ�� ����
    }

    private IEnumerator DisplayShockAndRun()
    {
        ClearActiveEffect(); // ���� ȿ�� ����

        activeEffect = Instantiate(shockObjectPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        activeEffect.transform.SetParent(transform);
        activeEffect.tag = "Shock";

        ChangeState(State.Idle);
        yield return new WaitForSeconds(2f);

        transform.Rotate(0, 180, 0);

        ChangeState(State.Run);

        ClearActiveEffect(); // ȿ�� ����
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
        canDetectPlayer = true; // ��Ÿ�� ���� �� Ž�� Ȱ��ȭ
    }

    //�Ȱų� �޸��� ������ �� ���� �������� ����Ǵ� ������ ������ ������ ���� ���� �������� ��ȸ�ϰų� �޸��� �ϱ� ���ؼ�.
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
            return State.Jump; // 25% Ȯ���� ���� ���·� ��ȯ
        }
        else if (random < 50) {
            return State.Run; // 25% Ȯ���� �޸��� ���·� ��ȯ
        }
        else if (random < 75) {
            return State.Idle; // 25% Ȯ���� ��� ���·� ��ȯ
        }
        else {
            return State.Wander; // 25% Ȯ���� ��ȸ ���·� ��ȯ
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
                Debug.LogWarning("SetRandomDestination ȣ�� ���� NavMesh ���� ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("Failed to find a valid NavMesh position!");
        }
    }

  //  protected virtual IEnumerator JumpThenIdle() {
  //
  //     // // NavMeshAgent ��Ȱ��ȭ
  //     // agent.updatePosition = false;
  //     // agent.updateRotation = false;
  //     // agent.isStopped = true;
  //
  //      // Rigidbody ����Ͽ� ����
  //    //  rb.isKinematic = false;
  //    //  rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
  //
  //    //  // ���� ���� �ð� ���� ���
  //    //  yield return new WaitForSeconds(3f);
  //
  //    //  // Rigidbody�� �ٽ� ��Ȱ��ȭ�ϰ� NavMeshAgent�� Ȱ��ȭ
  //    //  rb.isKinematic = true;
  //    //  agent.updatePosition = true;
  //    //  agent.updateRotation = true;
  //    //  agent.isStopped = false;
  //
  //      // ���¸� Idle�� ����
  //      ChangeState(State.Idle);
  //  }

    protected virtual IEnumerator IdleThenWander() {
        yield return new WaitForSeconds(Random.Range(2, 5));
        ChangeState(State.Wander);
    }
}
