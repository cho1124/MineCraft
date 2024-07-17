using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity
{
    //�����Ӱ� ���� ���� �غ��ʼ�
    /*
    1. ���ʹ� z�����θ� ������ (���ٰ� ������ȯ ����)
    2. �ȱ� / �޸��� / ����(�տ� ��ֹ��� ��������) / �����ֱ� ���� ����
    3. �÷��̾ Ư�� �ݰ� �ȿ� ������ ���� �Ϸ� �ٰ��;���
    (raycast �� ���� üũ�ص� ����, ���鿡 Ŀ�ٶ� ������Ʈ �����ϰ� ��ġ�ؼ� �����ص�)
    =>��ֹ��� ������ �����ϰų�, �������� ȸ���ϰų�, ���������� ȸ���ϸ� ���� �������� �پ�ȭ �� �� ������ ���Ƽ� ����
    ==>�������� �ʹ� �յڷθ� �������� �翷���� ȸ���ϴ� ������ �߰�
    ===>(Ȥ�� ��� ���� �������� ���ϰ� �ִ� ��Ȳ ���) 15�ʰ� 3f�̻��� ��ǥ ������ ������ 180�� ȸ�� �ϵ��� �ص� 
    */

    /*

    ���� ���ľ� �ϴ°�
    1. ���� �𸣰ڴµ� �������ڸ��� ������ó�� �ѹ��� ������
    2. �ڷΰ�
    3. �÷��̾�� ������Ʈ�� �������� ���ϴ� �� ���� 
    Ȯ���Ұ�


    */

    //���� �� ���� ���� ����
    private enum MonsterState { Idle, Walk, Run, Jump,Chase, TurnLeft, TurnRight }
    private MonsterState currentState; // ������ ������� 

    //������ ���� ����
    private Vector3 targetPosition; //���� ������
    private Rigidbody rb;
    private ObstacleDetector obstacleDetector; //��ֹ����� ť��
    public Transform head; //������ �Ӹ�(head) ����

    //�̵� �� �ൿ �Ӽ� ����
    public float walkRadius = 10f; //���Ͱ� �̵��� �� �ִ� ��Ȳ �ݰ�
    public float walkSpeed = 3f; //���� �⺻ ���� �ӵ�
    public float runSpeed = 4f; //���� �޸��� �ӵ�
    public float idleTime = 4f; //���� idle �ð�
    public float minWalkTime = 4f; //���� �ȱ� �ּ� �ð�
    public float maxWalkTime = 6f; //���� �ȱ� �ִ� �ð�
    public float jumpForce = 4f; //���� �����Ҷ� ��
    public float detectionDistance = 2f; //���� Ž�� �Ÿ�
    public float ChaseDuration = 7f; // �i�ƿ��� ���� �ð�
    public float turnDuration = 1f; // ȸ�� ���� �ð�

    // ��ġ ��ȭ ������ ���� ����
    private Vector3 lastPosition;
    private float lastPositionCheckTime;
    public float positionCheckInterval = 15f; // ��ġ�� üũ�� ����
    public float minPositionChange = 3f; // �ּ� ��ġ ��ȭ

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        obstacleDetector = GetComponentInChildren<ObstacleDetector>();
        ChangeState(MonsterState.Walk);
        lastPosition = transform.position;
        lastPositionCheckTime = Time.time;

    }
    private void Update()
    {
        CheckPositionChange();
        switch (currentState) {
            case MonsterState.Walk:
                Walk();
                break;
            case MonsterState.Idle:
                break;
            case MonsterState.Run:
                Run();
                break;
            case MonsterState.Jump:
                break;
            case MonsterState.Chase:
                Chase();
                break;
            case MonsterState.TurnLeft:
                TurnLeft();
                break;
            case MonsterState.TurnRight:
                TurnRight();
                break;
        }
    }

    private void FixedUpdate()
    {
        //���Ͱ� �������� ��� �ٷ� ����� 
        if(Vector3.Dot(transform.up,Vector3.down)>0.5f)
        {
            rb.AddTorque(Vector3.right * 10f);
        }
    }

    private void ChangeState(MonsterState NewState)
    {
        currentState = NewState;
        switch(currentState)
        {
            case MonsterState.Walk:
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(MonsterState.Walk, Random.Range(minWalkTime, maxWalkTime)));
                break;
            case MonsterState.Idle:
                StartCoroutine(StateDuration(MonsterState.Idle, Random.Range(2f, 10f)));
                break;
            case MonsterState.Run:
                targetPosition= GetRandomPosition();
                StartCoroutine(StateDuration(MonsterState.Run, Random.Range(minWalkTime, maxWalkTime)));
                break;
            case MonsterState.Jump:
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(StateDuration(MonsterState.Jump, 3f));
                break;
            case MonsterState.Chase:
                StartCoroutine(StateDuration(MonsterState.Chase, ChaseDuration));
                break;
            case MonsterState.TurnLeft:
                targetPosition = transform.position + Quaternion.Euler(0, -90, 0) * transform.forward * walkRadius;
                StartCoroutine(StateDuration(MonsterState.TurnLeft, turnDuration));
                break;
            case MonsterState.TurnRight:
                targetPosition = transform.position + Quaternion.Euler(0, 90, 0) * transform.forward * walkRadius;
                StartCoroutine(StateDuration(MonsterState.TurnRight, turnDuration));
                break;
        }
    }
    private void Walk()
    {
        MoveTowardsTarget(walkSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ChangeState(MonsterState.Idle);
        }
    }

    private void Run()
    {
        MoveTowardsTarget(runSpeed);
        if(Vector3.Distance(transform.position,targetPosition)<0.1f)
        {
            ChangeState(MonsterState.Idle);
        }
    }
    private void TurnLeft() {
        // ���͸� �������� ȸ��
        transform.Rotate(0, -90, 0);
        ChangeState(MonsterState.Walk);
    }

    private void TurnRight() {
        // ���͸� ���������� ȸ��
        transform.Rotate(0, 90, 0);
        ChangeState(MonsterState.Walk);
    }

    private void Chase() {
        //�÷��̾ ������ �����ϸ� �������� �̵�
        if (obstacleDetector.isPlayerDeteched) {
            // Player�� ��ġ�� targetPosition���� ����
            targetPosition = obstacleDetector.isPlayerDeteched ? GameObject.FindWithTag("Player").transform.position : targetPosition;
        }
        else if (obstacleDetector.isAnimalDeteched) {
            // Animal�� ��ġ�� targetPosition���� ����
            targetPosition = obstacleDetector.isAnimalDeteched ? GameObject.FindWithTag("Animals").transform.position : targetPosition;
        }
        MoveTowardsTarget(runSpeed);
    }

    private void MoveTowardsTarget(float speed) //���͸� targetPosition���� �̵���Ű�� ���
    {
        // targetPosition�� ���� ��ġ�� ���̸� ���ϰ�, ���� ���ͷ� ��ȯ
        Vector3 direction = (targetPosition - transform.position).normalized;

        // direction ���͸� ����Ͽ� ���ο� ��ġ�� ���
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // ������ ���� ��ġ�� newPosition���� ������Ʈ
        transform.position = newPosition;

        // ���Ͱ� �̵��� �� �ٶ󺸴� ������ ����
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private Vector3 GetRandomPosition() //���Ͱ� �̵��� ���ο� ���� ��ġ�� ���� (z�� ��ǥ�� �������� ����)
    {
        //������ ���⿡�� z�ุ �����ϰ� x�� y�� ���� ��ġ�� ����
        // 20% Ȯ���� ���� ����, 80% Ȯ���� ��� �������� �̵�
        float randomZ = Random.Range(0, 100) < 20 ? Random.Range(-walkRadius, 0) : Random.Range(0, walkRadius);

        // ���� ��ġ���� z�ุ randomZ��ŭ ����� ��ġ�� ��ȯ
        return new Vector3(transform.position.x, transform.position.y, transform.position.z + randomZ);
    }

    private IEnumerator StateDuration(MonsterState state, float duration) //���Ͱ� Ư�� ���¸� ���� �ð� ���� �����ϰ�
    {
        // ������ �ð�(duration)��ŭ ���
        yield return new WaitForSeconds(duration);

        // ���� ���¸� �����ϰ� ����
        MonsterState nextState = GetRandomState();

        // ���ο� ���·� ����
        ChangeState(nextState);
    }

    private MonsterState GetRandomState() {
        if (obstacleDetector.isPlayerDeteched
            || obstacleDetector.isAnimalDeteched) {
            return MonsterState.Chase;
        }
        else if (obstacleDetector.isObstacleDeteched) {
            int randomAction = Random.Range(0, 3); // ����, ��ȸ��, ��ȸ�� �� �ϳ� ����
            switch (randomAction) {
                case 0:
                    return MonsterState.Jump;
                case 1:
                    return MonsterState.TurnLeft;
                case 2:
                    return MonsterState.TurnRight;
                default:
                    return MonsterState.Jump;
            }
        }
        else {
            int randomIndex = Random.Range(0, 5); // jump ���¸� ������ ���� ����
            switch (randomIndex) {
                case 0:
                    return MonsterState.Walk;
                case 1:
                    return MonsterState.Run;
                case 2:
                    return MonsterState.Idle;
                case 3:
                    return MonsterState.TurnLeft;
                case 4:
                    return MonsterState.TurnRight;
                default:
                    return MonsterState.Walk;
            }
        }
    }

    private void CheckPositionChange() {
        if (Time.time - lastPositionCheckTime >= positionCheckInterval) {
            float distance = Vector3.Distance(transform.position, lastPosition);
            if (distance < minPositionChange) {
                // 15�ʰ� 3f �̻��� ��ġ ��ȭ�� ���� ��� 180�� ȸ��
                transform.Rotate(0, 180, 0);
                targetPosition = transform.position + transform.forward * walkRadius;
                ChangeState(MonsterState.Walk);
            }
            lastPosition = transform.position;
            lastPositionCheckTime = Time.time;
        }
    }
}
    
