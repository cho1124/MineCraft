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
    (Ŀ�ٶ� ť�긦 �����ϰ� ��ġ�ؼ� ���� ��:trigger)
    =>��ֹ��� ������ �����ϰų�, �������� ȸ���ϰų�, ���������� ȸ���ϸ� ���� �������� �پ�ȭ �� �� ������ ���Ƽ� ����
    ==>�������� �ʹ� �յڷθ� �������� �翷���� ȸ���ϴ� ������ �߰�
    ===>(Ȥ�� ��� ���� �������� ���ϰ� �ִ� ��Ȳ ���) 15�ʰ� 3f�̻��� ��ǥ ������ ������ 180�� ȸ�� �ϵ��� �ص� 
    ====> �÷��̾� �����ϸ� �÷��̾�� raycast ��鼭 �ѵ��� chase(7��) �� 
    =====> �÷��̾� ������ ���� �ڿ������� �����̵��� ���� �ð����� ���� �������� ȸ���ϰ� �̵��ϰ� �� ���� ���� ���� �����ϵ���


    Ȯ���Ұ�
    �� �����ϰ� �����ð� ������ ���ڱ� �ٵ� ���ڸ�����....�߱���...(�ſ�ū����..)
    GetRandomPosition()  �� �ִ� ������ ���� �α� �������� ����


    Ȯ���Ѱ�
    1. ���� �𸣰ڴµ� �������ڸ��� ������ó�� �ѹ��� ������
    => ���� ���� walk�ΰŸ� idle�� ���ƴ��� �������� 
 
    3. �÷��̾�� ������Ʈ�� �������� ���ϴ� �� ���� 
    => �÷��̾�� ��Ȯ�ϰ� �ν��ϰ� ����
 
    */

    //���� �� ���� ���� ����
    private enum MonsterState { Idle, Walk, Run, Jump, Chase, TurnLeft, TurnRight }
    private MonsterState currentState; // ������ ������� 

    //������ ���� ����
    private Vector3 targetPosition; //���� ������
    private Rigidbody rb;
    private ObstacleDetector obstacleDetector; //��ֹ����� ť��
    public Transform head; //������ �Ӹ�(head) ����

    //�̵� �� �ൿ �Ӽ� ����
    public float walkRadius = 5f; //���Ͱ� �̵��� �� �ִ� ��Ȳ �ݰ�
    public float walkSpeed = 3f; //���� �⺻ ���� �ӵ�
    public float runSpeed = 4f; //���� �޸��� �ӵ�
    public float idleTime = 4f; //���� idle �ð�
    public float minWalkTime = 6f; //���� �ȱ� �ּ� �ð�
    public float maxWalkTime = 12f; //���� �ȱ� �ִ� �ð�
    public float jumpForce = 4f; //���� �����Ҷ� ��
    public float detectionDistance = 2f; //���� Ž�� �Ÿ�
    public float ChaseDuration = 5f; // �i�ƿ��� ���� �ð�
    public float turnDuration = 4f; // ȸ�� ���� �ð�

    // ��ġ ��ȭ ������ ���� ����
    private Vector3 lastPosition;
    private float lastPositionCheckTime;
    public float positionCheckInterval = 15f; // ��ġ�� üũ�� ����
    public float minPositionChange = 3f; // �ּ� ��ġ ��ȭ

    public bool IsChasingPlayer { get; set; } = false; // �÷��̾ ���� ������ ���θ� ����

    private Coroutine stateCoroutine;

    protected override void Start()
    {
        base.Start();  // Entity Ŭ������ Start �޼��� ȣ��
        rb = GetComponent<Rigidbody>();
        obstacleDetector = GetComponentInChildren<ObstacleDetector>();
        ChangeState(MonsterState.Idle);
        lastPosition = transform.position;
        lastPositionCheckTime = Time.time;

    }

    private void Update()
    {
        CheckPositionChange(); // ���� �������� ��ġ ��ȭ�� üũ�մϴ�.
        switch (currentState) // ���� ���¿� ���� �ٸ� �ൿ�� �����մϴ�.
        { 
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
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            rb.AddTorque(Vector3.right * 10f); // ���� ���ؼ� �ڿ������� ������ �����ϴ� ���

            /*
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
              ������Ʈ�� ���߰� ������ �缳���Ͽ� ���� ���·� ������, ������ ���ڿ������� �� ����                   
             
             */
        }
    }

    private void ChangeState(MonsterState NewState) // ���¸� �����ϴ� �޼����Դϴ�.
    {
        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
        }

        currentState = NewState; // ���� ���¸� ���ο� ���·� �����մϴ�.
        switch (currentState)
        {
            case MonsterState.Walk:
               targetPosition = GetRandomPosition(); // �ȱ� ���·� ����� �� ���ο� ��ǥ ��ġ�� �����մϴ�.
                StartCoroutine(StateDuration(MonsterState.Walk, Random.Range(minWalkTime, maxWalkTime))); // �ȱ� ������ ���� �ð��� �����մϴ�.
                break;
            case MonsterState.Idle:
                StartCoroutine(StateDuration(MonsterState.Idle, Random.Range(4f, 10f))); // ���� �ִ� ������ ���� �ð��� �����մϴ�.
                break;
            case MonsterState.Run:
                StartCoroutine(StateDuration(MonsterState.Run, Random.Range(minWalkTime, maxWalkTime)));
                break;
            case MonsterState.Jump:
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(StateDuration(MonsterState.Jump, 3f));
                break;
            case MonsterState.Chase:
              //  StartCoroutine(StateDuration(MonsterState.Chase, ChaseDuration));
                break;
            case MonsterState.TurnLeft:
                StartCoroutine(StateDuration(MonsterState.TurnLeft, turnDuration));
                break;
            case MonsterState.TurnRight:
                StartCoroutine(StateDuration(MonsterState.TurnRight, turnDuration));
                break;
        }
    }

    private void Walk() // �ȱ� ���¿��� ������ �ൿ�� �����ϴ� �޼����Դϴ�.
    {
        MoveTowardsTarget(walkSpeed); // ��ǥ ��ġ�� �ȱ� �ӵ��� �̵��մϴ�.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
        {
            ChangeState(MonsterState.Idle); // Idle ���·� ����
        }
    }

    private void Run()  // �޸��� ���¿��� ������ �ൿ�� �����ϴ� �޼����Դϴ�.
    {
        MoveTowardsTarget(runSpeed); // ��ǥ ��ġ�� �޸��� �ӵ��� �̵��մϴ�.
        if (Vector3.Distance(transform.position,targetPosition)<0.1f) // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
        {
            ChangeState(MonsterState.Idle); // Idle ���·� ����
        }
    }

    private void TurnLeft() 
    {
        // ���͸� �������� ȸ��
        transform.Rotate(0, -90, 0);
        ChangeState(MonsterState.Walk);
    }

    private void TurnRight() 
    {
        // ���͸� ���������� ȸ��
        transform.Rotate(0, 90, 0);
        ChangeState(MonsterState.Walk);
    }

    private void Chase() 
    {
        MoveTowardsTarget(runSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
        {
            EndChaseAndWander(); // ���� ���� �� �ٸ� ���·� ��ȯ
        }
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

    private Vector3 GetRandomPosition() //���Ͱ� �̵��� ���ο� ���� ��ġ�� ����
    {
        Debug.Log($"{name}���ο� �������� �����ϰڽ��ϴ�.");
        float randomAngle = Random.Range(0, 360);// ���� ������ �����մϴ�.
        Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward; // ���� ������ �����մϴ�.
        Vector3 randomPosition;
        do
        {
            randomPosition = transform.position + randomDirection * walkRadius; // ���� ��ġ���� ���� �������� walkRadius��ŭ ������ ��ġ�� ��ȯ�մϴ�.
        } while (Vector3.Distance(transform.position, randomPosition) < 3f); // �ּ� �Ÿ� ���� �߰�
        return randomPosition;
    }

    private IEnumerator StateDuration(MonsterState state, float duration) //���Ͱ� Ư�� ���¸� ���� �ð� ���� �����ϰ�
    {
        // ������ �ð�(duration)��ŭ ���
        yield return new WaitForSeconds(duration+5f);
        // ���� ���¸� �����ϰ� ����
        MonsterState nextState = GetRandomState();

        // ���ο� ���·� ����
        ChangeState(nextState);
    }

    private MonsterState GetRandomState()  // ���� ���¸� �����ϴ� �޼����Դϴ�.
    {
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
    
    private void CheckPositionChange()  // ��ġ ��ȭ�� üũ�ϴ� �޼����Դϴ�.
    {
        if (Time.time - lastPositionCheckTime >= positionCheckInterval) // ���������� ��ġ�� üũ�� �ð����� ���� �ð��� �������� Ȯ���մϴ�.
        {
            float distance = Vector3.Distance(transform.position, lastPosition);// ���� ��ġ�� ������ ��ġ ������ �Ÿ��� ����մϴ�.
            if (distance < minPositionChange)// �ּ� ��ġ ��ȭ���� ������
            {
                transform.Rotate(0, 180, 0); // ���͸� 180�� ȸ����ŵ�ϴ�.
                targetPosition = transform.position + transform.forward * walkRadius; // ���ο� ��ǥ ��ġ�� �����մϴ�.
                ChangeState(MonsterState.Walk); // �ȱ� ���·� �����մϴ�.
            }
            lastPosition = transform.position; // ���� ��ġ�� ������ ��ġ�� ������Ʈ�մϴ�.
            lastPositionCheckTime = Time.time; // ���� �ð��� ������ ��ġ üũ �ð����� ������Ʈ�մϴ�.
        }
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        ChangeState(MonsterState.Chase);
    }

    public void EndChaseAndWander()
    {
        IsChasingPlayer = false;
        ChangeState(GetRandomState());
    }

    public void JumpAndChangeState()
    {
        ChangeState(MonsterState.Jump);
        ChangeState(GetRandomState());
    }

   

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Tool"))
        {
            TakeDamage(10); // �÷��̾�� �浹 �� 10�� �������� ����
        }
    }
}
    
