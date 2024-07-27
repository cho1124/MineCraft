using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Entity, IDamageable
{
    //������ �����Ӱ� �����ϸ� �ɰſ���

    /*
    
    �������� ����� ���¿��� �̼��� ������
    ������ foodGivenCount �� ������ ����.
    ���λ��¿��� 5�� Animal_Food�� ������ tamed �� �̸��� �ٰ�, �� ���¿��� 
    5�� Animal_Food �� �� ������ overgrowth��.
    overgrowth ��Ȳ�϶��� ��ġ�� Ŀ���� ��⸦ 2�� �� ��.
    ����Ǵ� ����� �������� �ƽ�����İ��������� Ŭ ��� ���� ����


     */

    private bool isInvincible = false; // ���� ���� ����(������ �¾�ڸ� ���Ϳ� �ε��� �״� ��찡 ���Ƽ�)
    // animalspawner ��ũ��Ʈ�� �������� 2�ʷ� �����Ͽ� �¾�� 2�ʵ����� ������ �ǵ���

    public GameObject beafPrefab; // beaf �������� �ν����Ϳ��� ����

    // ���� ���� ������
    public GameObject adultPrefab; //  ���� ������ ������
    public GameObject babyPrefab; // ���� ������ ������
    public GameObject overgrowthPrefab; // ���̹� ������Ʈ ������
    private bool isAdult; // ���� �������� ����
    private float growthTime = 60; // ���� ���� �ð� 
    private float growthTimeGaze = 0; // ��θ����������帣�½ð� :���� Ÿ�̸�

    // ����� ���� ������
    public int hungerLevel = 7; // ����� ������
    private const int maxHungerLevel = 10;
    private bool isHungry = false; // ����� ����
    private bool isFull = false; // ��θ� ����
    private float speedIncrease = 1f; // �߰� �̵� �ӵ�
    private int hungerIncrease;

    // �÷��̾ ������ �� Ƚ���� ������ ����
    private int foodGivenCount = 0;
    private const int OvergrowthThreshold = 5;
    private bool isOvergrowth = false;
    private bool isTamed = false;

    // �̵� �ӵ� ���� ������
    public float walkSpeed = 3; // �⺻ �̵� �ӵ�, �ν����Ϳ��� ����
    public float runSpeed = 4; // �⺻ �̵� �ӵ�, �ν����Ϳ��� ����
    private float currentSpeed; // ���� �̵� �ӵ�
    public float minWalkTime = 4f; //�ȱ� �ּ� �ð�
    public float maxWalkTime = 8f; //�ȱ� �ִ� �ð�
    public float jumpForce = 4f; //�����Ҷ� ��

    // ��ü ���� ���� ������
    private bool canSpawn = true; // ��ü ���� ��Ÿ�� �÷���
    private float spawnCooldown = 600f; // ��Ÿ�� �ð�
    private const int collisionThreshold = 10; // �浹 �Ӱ谪
    protected Queue<int> recentAnimals = new Queue<int>(); // �ֱ� Ž���� 10���� ��ü�� ������ ť
    protected Dictionary<int, int> animalCount = new Dictionary<int, int>(); // Ž���� ��ü�� Ž�� Ƚ���� ������ ��ųʸ�

    // ������Ʈ ���� ������
    public GameObject heartObjectPrefab; // ��Ʈ ������Ʈ ������
    public GameObject shockObjectPrefab; // ��� ������Ʈ ������
    private Vector3 targetPosition; //������
    public Transform head; // ������ head ����

    // Ž�� ���� ������
    public float wanderRadius = 30f; // ������ ��ȸ�� �� �ִ� �ִ� �ݰ��Դϴ�.

    // ���� ���� ������
    protected enum State { Walk, Jump, Idle, Run }
    protected State currentState;
    private Coroutine stateCoroutine;

    // ��ġ ��ȭ ���� ���� ������
    private Vector3 lastPosition; //������ ��ȸ�� �� �ִ� �ִ� �ݰ��Դϴ�.
    private float idleTime; //��� �ð��� �����ϴ� �����Դϴ�.
    private float idleTimeLimit = 10f; // 10�� ���� ��ġ�� ������ ������ �̵�
    private const float positionThreshold = 3f; // ��ġ ��ȭ ��� ����(10�ʵ��� �� ���� �ȿ��� ������ �׺�޽�����������)

    private GameObject activeEffect;
    private GameObject overgrowthEffect; // overgrowth ��ũ�� ���� ����
    private bool isGrounded = true;

    protected override void Start()
    {

        base.Start();

        // Entity�� OnDeath �̺�Ʈ�� Die �޼��带 ����
        OnDeath += Die;

        //������Ʈ �ʱ�ȭ
        Collider col = GetComponent<Collider>();

        // ���� ���� �ʱ� ����
        if (gameObject.name.Contains("Baby"))
        {
            isAdult = false; // ��������
        }
        else
        {
            isAdult = true; // ���� ����
        }

        // �ʱ� �̵� �ӵ� ����
        currentSpeed = walkSpeed;

        // ����� ���� ��ƾ ����
        StartCoroutine(HungerRoutine());

        // �ʱ� ���� ����
        ChangeState(State.Idle);

        // ��ġ ��ȭ ���� �ʱ�ȭ
        lastPosition = transform.position;
        //�������� �������� �ʰ� �Ѱ��� �����ִ��� Ȯ���ϱ� ���� ���ð� �ʱ�ȭ
        idleTime = 0f;
    }

    void Update()
    {

        //���� ���� ������Ʈ
        //������ �ƴϰ� ��θ����°� growthTimeGaze ��ŭ �����Ǹ� �������� �ڶ�
        if (!isAdult && isFull)
        {
            growthTimeGaze += Time.deltaTime;
            if (growthTimeGaze >= growthTime)
            {
                GrowUp();
            }
        }

        // ����� ���¿� ���� ���� Ÿ�̸� �ʱ�ȭ
        if (!isFull)
        {
            growthTimeGaze = 0f;
        }

        // ��ġ ��ȭ ����
        CheckIdleTime();

    }

    private void FixedUpdate()
    {
        //���Ͱ� �������� ��� �ٷ� ����� 
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            rb.AddTorque(Vector3.right * 10f); // ���� ���ؼ� �ڿ������� ������ �����ϴ� ���
        }

        // ���� ���¿� ���� �ൿ ����
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
    {   //�̵��� �ʹ� ���� ������� �������� �����Ŵ
        // ���� ��ġ�� ������ ��ġ ������ �Ÿ��� ����մϴ�.
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        // �̵��� �Ÿ��� positionThreshold ������ ��� idleTime�� ������ŵ�ϴ�.
        if (distanceMoved < positionThreshold)
        {
            idleTime += Time.deltaTime;
        }
        // �̵��� �Ÿ��� positionThreshold�� �ʰ��� ��� idleTime�� �ʱ�ȭ�ϰ� ������ ��ġ�� ������Ʈ�մϴ�.
        else
        {
            idleTime = 0f;
            lastPosition = transform.position;
        }
        // idleTime�� idleTimeLimit�� �ʰ��� ��� ���ο� ���� �������� �����մϴ�.
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

        // �ڽŰ� ���� ������Ʈ�� ���� ������Ʈ���� �浹���� Ȯ��
        if (collision.gameObject.GetComponent(GetType()) != null)
        {
            //�ֱ� �浹�� ���� ��Ͽ� �߰�
            AddToRecentAnimals(collision.gameObject);
            int collisionAnimalId = collision.gameObject.GetInstanceID();
            if (animalCount.ContainsKey(collisionAnimalId) && animalCount[collisionAnimalId] >= collisionThreshold)
            {
                if (canSpawn && !isOvergrowth) // ������ ���°� �ƴϰ�, ���� ��Ÿ���� �����߸� ����
                {
                    SpawnNewAnimal();
                    animalCount[collisionAnimalId] = 0;
                }
            }
        }

        // Food �±װ� ���� ������Ʈ�� �Դ� ó��
        if (collision.gameObject.CompareTag("Food"))
        {
            Eat("Food", hungerIncrease);
            Destroy(collision.gameObject); // ���� ������Ʈ ����
        }

        // Animal_Food �±װ� ���� ������Ʈ�� �Դ� ó��
        if (collision.gameObject.CompareTag("Animal_Food"))
        {
            Eat("Animal_Food",1);
            Destroy(collision.gameObject); // ���� ������Ʈ ����
        }

        // �浹�� ���� �÷��̾�� ���͸� Ž��
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

    //���� ���� ������ 10ȸ �̻� �浹������ ������ ����(����) �ϱ� ���� Ȯ���ϴ� �۾�
    protected virtual void AddToRecentAnimals(GameObject animal)
    {
        // Baby�� ���Ե� ������Ʈ�� ����
        if (animal.name.Contains("Baby")) {
            return;
        }

        int animalId = animal.GetInstanceID();

        // recentAnimals ť�� ũ�Ⱑ 10 �̻����� Ȯ��
        if (recentAnimals.Count >= 10)
        {
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
        if (animalCount.ContainsKey(animalId))
        {
            // �̹� �ִٸ�, �ش� ������ �浹 Ƚ���� ������Ŵ
            animalCount[animalId]++;
        }
        else
        {
            // ���ٸ�, �ش� ������ �߰��ϰ� �浹 Ƚ���� 1�� ����
            animalCount[animalId] = 1;
        }
    }

    private void SpawnNewAnimal()
    {
        // ���� ������Ʈ�� ũ��� ���� �������� ũ�⸦ ��(���θ� ����(����)�� �� �ֵ���)
        if (transform.localScale == adultPrefab.transform.localScale)
        {
            // ���� ������Ʈ�� ����
            GameObject newAnimal = Instantiate(gameObject, transform.position, transform.rotation);
            // ������ ������Ʈ�� ũ�⸦ 1/2���� ����
            newAnimal.transform.localScale = transform.localScale / 2;

            // ������ ������Ʈ�� �浹 ī��Ʈ�� �ʱ�ȭ
            Animal tracker = newAnimal.GetComponent<Animal>();
            if (tracker != null)
            {
                tracker.recentAnimals.Clear();
                tracker.animalCount.Clear();
            }
            // ������ ������Ʈ�� �̸��� "Baby" �߰� (�̹� "Baby"�� ���Ե� ��� �߰����� ����)
            if (!newAnimal.name.Contains("Baby")) {
                newAnimal.name = gameObject.name + "_Baby";
            }

            // ������ ������Ʈ�� "Animals" �±� �߰�
            newAnimal.tag = "Animals";

            // ��Ÿ�� ����
            StartCoroutine(SpawnCooldown());
        }
    }

    protected virtual IEnumerator SpawnCooldown()
    {
        canSpawn = false; // ���� ����
        yield return new WaitForSeconds(spawnCooldown); // ��Ÿ�� ���
        // �浹 Ƚ���� ť �ʱ�ȭ
        recentAnimals.Clear();
        animalCount.Clear();
        canSpawn = true; // ���� ���
    }

    private void GrowUp()
    {

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

    private void Eat(string foodType, int hungerIncrease)
    {
        // ���� hungerLevel�� maxHungerLevel�� �ʰ��ϸ� ���� ����
        if (hungerLevel + hungerIncrease > maxHungerLevel)
        {
            Debug.Log($"{name}�� {foodType}�� ���� �� �����ϴ�. ����� �������� �ִ�ġ�� �ʰ��մϴ�.");
            return;
        }
        // ����� ������ �ִ�ġ �̻��̸� ���� ����
        if (hungerLevel >= maxHungerLevel)
        {
            Debug.Log($"{name}�� {foodType}�� ���� �� �����ϴ�. ����� ������: {hungerLevel} (�ִ�ġ)");
            return;
        }
        hungerLevel += hungerIncrease;

        // Animal_Food�� ���� �� �߰� ó��
        if (foodType == "Animal_Food")
        {
            if (!isOvergrowth && !name.Contains("Baby") && !isTamed) // Baby�� ���Ե��� �ʰ�, �������� �ƴϰ�, ���õ��� ���� ������ ��
            {
                foodGivenCount++;
                Debug.Log($"{name}�� Animal_Food�� �Ծ����ϴ�. foodGivenCount: {foodGivenCount}");

                if (foodGivenCount >= OvergrowthThreshold) // ���� Ƚ���� �����ϸ� isTamed ���·� ����
                {
                    isTamed = true;
                    TamedMark();
                    Debug.Log($"{name}�� ���õƽ��ϴ�!");
                    foodGivenCount = 0;
                }
            }
            else if (isTamed && !name.Contains("Baby")) // ���õ� �����̰� ������ �ƴ� ��
            {
                foodGivenCount++;
                Debug.Log($"{name}foodGivenCount: {foodGivenCount}");

                if (foodGivenCount >= OvergrowthThreshold) // ���� Ƚ���� �����ϸ� isOvergrowth ���·� ����
                {
                    isOvergrowth = true;
                    Becomeovergrowth();
                    Debug.Log($"{name}�� ������ ���°� �Ǿ����ϴ�!");
                }
            }
        }

        // ����� ���� ����
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

            // ����� ���� ����
            if (hungerLevel > 6)
            {
                isHungry = false;
                isFull = true;
                currentSpeed = walkSpeed; // ��θ� ���¿����� �⺻ �ӵ�
            }
            else
            {
                isHungry = true;
                isFull = false;

                if (hungerLevel <= 3)
                {
                    currentSpeed = walkSpeed + speedIncrease; // ����� ���¿����� �ӵ� ����
                }
                else
                {
                    currentSpeed = walkSpeed; // �⺻ �ӵ�
                }
            }
        }
    }

    private void Walk()
    {
        MoveTowardsTarget(walkSpeed); // ��ǥ ��ġ�� �ȱ� �ӵ��� �̵��մϴ�.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
        {
            ChangeState(State.Idle); // Idle ���·� ����
        }
    }

    private void Run()
    {
        MoveTowardsTarget(runSpeed); // ��ǥ ��ġ�� �޸��� �ӵ��� �̵��մϴ�.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
        {
            ChangeState(State.Idle); // Idle ���·� ����
        }
    }

    private void MoveTowardsTarget(float speed) //���͸� targetPosition���� �̵���Ű�� ���
    {

        // targetPosition�� ���� ��ġ�� ���̸� ���ϰ�, ���� ���ͷ� ��ȯ
        Vector3 direction = (targetPosition - transform.position).normalized;

        // direction ���͸� ����Ͽ� ���ο� ��ġ�� ���
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // ������ ���� ��ġ�� newPosition���� ������Ʈ
        rb.MovePosition(newPosition);
        //Rigidbody�� ����Ͽ� ��ü�� �̵���Ű�� ����Դϴ�. �̴� ���� ������ ���� �̵��� ó���ϹǷ�, �浹 �� ��Ÿ ������ ��ȣ�ۿ��� ����Ͽ� ��ü�� �̵���ŵ�ϴ�. 

        // ���Ͱ� �̵��� �� �ٶ󺸴� ������ ����
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        randomDirection.y = 0; // y�� ����
        return randomDirection;
    }

    protected virtual void OnPlayerDetected()
    {
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
        Debug.Log($"��������.");
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

    //�Ȱų� �޸��� ������ �� ���� �������� ����Ǵ� ������ ������ ������ ���� ���� �������� ��ȸ�ϰų� �޸��� �ϱ� ���ؼ�.
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
                targetPosition = GetRandomPosition(); // �ȱ� ���·� ����� �� ���ο� ��ǥ ��ġ�� �����մϴ�.
                stateCoroutine = StartCoroutine(StateDuration(State.Walk, Random.Range(minWalkTime, maxWalkTime))); // �ȱ� ������ ���� �ð��� �����մϴ�.
                break;
            case State.Jump:
                if (isGrounded)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    isGrounded = false; // ������ ���������� ǥ��
                }
                stateCoroutine = StartCoroutine(StateDuration(State.Jump, 3f));
                break;
            case State.Idle:
                animator.Play("Idle");
                stateCoroutine = StartCoroutine(StateDuration(State.Idle, Random.Range(5f, 10f))); // ���� �ִ� ������ ���� �ð��� �����մϴ�.
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
            return State.Jump; // 25% Ȯ���� ���� ���·� ��ȯ
        }
        else if (random < 50)
        {
            return State.Run; // 25% Ȯ���� �޸��� ���·� ��ȯ
        }
        else if (random < 75)
        {
            return State.Idle; // 25% Ȯ���� ��� ���·� ��ȯ
        }
        else
        {
            return State.Walk; // 25% Ȯ���� �ȱ� ���·� ��ȯ
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
            Debug.Log($"{name}��(��) ���� �����Դϴ�. �������� ���� �ʽ��ϴ�.");
            return;
        }

        base.TakeDamage(damage); // base.TakeDamage�� ȣ���Ͽ� Health ������Ƽ�� ���� ���� �����մϴ�.

        StartCoroutine(HandleDamage(damage)); // �������� ó���ϴ� �ڷ�ƾ ȣ��
    }

    private IEnumerator HandleDamage(int damage) //������ ���� ������ �Ĵٺ���, �ݴ�� ����, �޸���� 
    {
        // �������� ���� �������� head ȸ��
        Vector3 damageDirection = (transform.position - Camera.main.transform.position).normalized; // ���� ���, ī�޶� ��ġ�� �������� ������ ���� ���
        head.rotation = Quaternion.LookRotation(damageDirection);

        yield return new WaitForSeconds(1f); // 1�� ���

        // 180�� ȸ���ϰ� Run ���·� ��ȯ
        transform.Rotate(0, 180, 0);
        ChangeState(State.Run);

        StartCoroutine(DisplayShockAndRun()); // �������� �Ծ��� �� DisplayShockAndRun �ڷ�ƾ ȣ��
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

        // overgrowth ������ ��� ��� ���� �� �� ��
        if (isOvergrowth)
        {
            meatCount *= 2;
        }

        for (int i = 0; i < meatCount; i++)
        {
            Vector3 spawnPosition = position + new Vector3(i * 0.5f, 2, 0);
            Instantiate(beafPrefab, spawnPosition, rotation);
        // ��� ���� �� ���� �ð� ���� ���� ���·� ����
            SetInvincible(2.0f);
        }

        StartCoroutine(OnDie()); // �ٷ� OnDie �ڷ�ƾ ȣ��
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
        // ������ �������� ����
        Instantiate(overgrowthPrefab, transform.position, transform.rotation);
        // ���� ������Ʈ �ı�
        Destroy(gameObject);
    }
}
