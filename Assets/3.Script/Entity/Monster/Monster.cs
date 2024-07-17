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

    */

    //���� �� ���� ���� ����
    private enum MonsterState { Idle, Walk, Run, Jump,Chase }
    private MonsterState currentState; // ������ ������� 

    //������ ���� ����
    private Vector3 targetPosition; //���Ͱ� ������ ��ü ��ǥ ��ġ
    private Rigidbody rb;
    private ObstacleDetector obstacleDetector; //��ֹ����� ť��
    public Transform head; //������ �Ӹ�(head) ����
    public Transform player; //�÷��̾� ������Ʈ
    public Transform animal; // ���� ������Ʈ ���� �߰�

    //�̵� �� �ൿ �Ӽ� ����
    public float walkRadius = 10; //���Ͱ� �̵��� �� �ִ� ��Ȳ �ݰ�
    public float walkSpeed = 3; //���� �⺻ ���� �ӵ�
    public float runSpeed = 4; //���� �޸��� �ӵ�
    public float idleTime = 4f; //���� idle �ð�
    public float minWalkTime = 4; //���� �ȱ� �ּ� �ð�
    public float maxWalkTime = 6; //���� �ȱ� �ִ� �ð�
    public float jumpForce = 4f; //���� �����Ҷ� ��
    public float detectionDistance = 2f; //���� Ž�� �Ÿ�
    public float ChaseDuration = 7f; // �i�ƿ��� ���� �ð�

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        obstacleDetector = GetComponentInChildren<ObstacleDetector>();
        ChangeState(MonsterState.Walk);

    }
    private void Update()
    {
        switch(currentState)
        {
            case MonsterState.Walk:
                Walk();
                break;
            case MonsterState.Idle:
                //�����ִ� �ڵ带 ����
                break;
            case MonsterState.Run:
                Run();
                break;
            case MonsterState.Jump:
                //����
                break;
            case MonsterState.Chase:
                Chase();
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

    private void Chase()
    {
        //�÷��̾ ������ �����ϸ� �������� �̵�
        if(obstacleDetector.isPlayerDeteched)
        {
            targetPosition = player.position;
        }
        else if(obstacleDetector.isAnimalDeteched)
        {
            targetPosition = animal.position;
        }
        MoveTowardsTarget(runSpeed);
    }

    private void MoveTowardsTarget(float speed)
    {
        // head ������Ʈ�� forward �������� �̵�
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private Vector3 GetRandomPosition()
    {
        //������ ���⿡�� z�ุ �����ϰ� x�� y�� ���� ��ġ�� ����
        float randomZ = Random.Range(0, 100) < 30 ? Random.Range(-walkRadius, 0) : Random.Range(0, walkRadius);
        return new Vector3(transform.position.x, transform.position.y, transform.position.z + randomZ);
    }

    private IEnumerator StateDuration(MonsterState state, float duration)
    {
        yield return new WaitForSeconds(duration);

        MonsterState nextState = GetRandomState();
        ChangeState(nextState);
    }

    private MonsterState GetRandomState()
    {
        if (obstacleDetector.isPlayerDeteched || obstacleDetector.isAnimalDeteched)
        {
            return MonsterState.Chase;
        }
        else if (obstacleDetector.isObstacleDeteched)
        {
            return MonsterState.Jump;
        }
        else
        {
            int randomIndex = Random.Range(0, 3); //jump ���¸� ������ ���� ����
            switch (randomIndex)
            {
                case 0:
                    return MonsterState.Walk;
                case 1:
                    return MonsterState.Run;
                case 2:
                    return MonsterState.Idle;
                default:
                    return MonsterState.Walk;
            }
        }
    }
}
    
