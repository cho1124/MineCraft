using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity, IDamageable
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

    =====> �÷��̾� ������ ���� �ڿ������� �����̵��� ���� �ð����� ���� �������� ȸ���ϰ� �̵��ϰ� �� ���� ���� ���� �����ϵ���
 
    <����>
    ���ʹ� ObstacleDetector ��ũ��Ʈ(���Ϳ��� �ִ� ������Ʈ�� ������ ť�꿡 ����)�� 
    ����Ͽ� �÷��̾�� ������ ����
    �÷��̾����� �������� Ȯ���ϰ� chaseTarget�ڷ�ƾ�� ������
    chaseTarget�ڷ�ƾ������ raycast�� ���� �����ð� ��ǥ�� �����ϰ�,
    ��ǥ�� �����Ǹ� ��ǥ�� ��ġ�� settarget �޼��带 Ȱ���� ���Ϳ� ����->���ʹ� chase ���°� ��->��ǥ��ġ�� �̵��ϰ� ���� ���� ���� attack����
    �ִ������Ÿ� ���� �ִ� ���� ��� ����
    ��ǥ�� ������ ����ų� �ð��� ����ϸ� ������ �����ϰ�, EndChaseAndWander �޼��带 ȣ���Ͽ� �ٽ� ���� ���·� ��ȯ

    �ڸ��͵� ���� ��ũ��Ʈ ���� �ȿ����̰� ���׽��ϴ�...��

    */

    //���� �� ���� ���� ����
    private enum MonsterState { Idle, Walk, Run, Jump, Chase, TurnLeft, TurnRight, Attack }
    private MonsterState currentState; // ������ ������� 

    //������ ���� ����
    private Vector3 targetPosition; //���� ������
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
    private bool isGrounded = true; // ���Ͱ� �ٴڿ� �ִ��� ���θ� ����

    public float attackRange = 1.5f; // ���� ����

    private Vector3 originalColliderSize;
    private Vector3 expandedColliderSize;

    private BoxCollider boxCollider;

    //�÷��̾�� ���� ���� ���� ����
    private float lastPlayerDetectionTime;
    private float lastAnimalDetectionTime;
    public float detectionCooldown = 2f; // ���� ��ٿ� �ð�
    private Coroutine chaseCoroutine;

    protected override void Start()
    {
        base.Start();  // Entity Ŭ������ Start �޼��� ȣ��
        obstacleDetector = GetComponentInChildren<ObstacleDetector>();
        originalColliderSize = boxCollider.size;
        expandedColliderSize = originalColliderSize * 2; //���Ϳ� �ֺ� �繰�� �� �浹�� ���Ͼ�°Ű��Ƽ� �����ø� �ݶ��̴� 2��ũ���
        ChangeState(MonsterState.Idle);
        lastPosition = transform.position;
        lastPositionCheckTime = Time.time;

    }

    private void FixedUpdate()
    {
        //���Ͱ� �������� ��� �ٷ� ����� 
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            rb.AddTorque(Vector3.right * 10f); // ���� ���ؼ� �ڿ������� ������ �����ϴ� ���

            /*
            �������� ���� �� �ִ� �ٸ� �ڵ�
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
              ������Ʈ�� ���߰� ������ �缳���Ͽ� ���� ���·� ������, ������ ���ڿ������� �� ����                   
             
             */
        }

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
            case MonsterState.Attack:
                AttackTarget();
                break;
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // �ٴڿ� ���������� ǥ��
        }

        if (collision.gameObject.CompareTag("Weapon"))
        {
            TakeDamage(10); // �÷��̾��� �浹 �� 10�� �������� ����
            Debug.Log($"{collision}���� {transform.name}���ݹ���~~");
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
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.Walk, Random.Range(minWalkTime, maxWalkTime))); // �ȱ� ������ ���� �ð��� �����մϴ�.
                break;
            case MonsterState.Idle:
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.Idle, Random.Range(4f, 10f))); // ���� �ִ� ������ ���� �ð��� �����մϴ�.
                break;
            case MonsterState.Run:
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.Run, Random.Range(minWalkTime, maxWalkTime)));
                break;
            case MonsterState.Jump:
                if (isGrounded)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    isGrounded = false; // ������ ���������� ǥ��
                }
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.Jump, 3f));
                break;
            case MonsterState.Chase:
                //  StartCoroutine(StateDuration(MonsterState.Chase, ChaseDuration));
                break;
            case MonsterState.TurnLeft:
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.TurnLeft, turnDuration));
                break;
            case MonsterState.TurnRight:
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.TurnRight, turnDuration));
                break;
            case MonsterState.Attack:
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.Attack, 2f));
                break;
                default:
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.Idle, Random.Range(4f, 10f)));
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
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
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
        if (Vector3.Distance(transform.position, targetPosition) < attackRange)
        {
            ChangeState(MonsterState.Attack);
        }
        else if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            EndChaseAndWander();
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
        Vector3 randomPosition;
        int attempt = 0;
        do
        {
            float randomDistance = Random.Range(1f, walkRadius);
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = new Vector3(Mathf.Sin(randomAngle), 0, Mathf.Cos(randomAngle)).normalized;
            randomPosition = transform.position + randomDirection * randomDistance;

            attempt++;
            if (attempt > 10)
            {
                // �ʹ� ���� �õ� �Ŀ��� ��ȿ�� ��ġ�� ã�� ���ϸ� �ּ����� �̵��� ��
                randomPosition = transform.position + new Vector3(1f, 0, 1f).normalized * walkRadius;
                break;
            }
        } while (Vector3.Distance(transform.position, randomPosition) < 1f);

        return randomPosition;
    }

    private IEnumerator StateDuration(MonsterState state, float duration)
    {
        yield return new WaitForSeconds(duration);
        ChangeState(GetRandomState());
    }

    private MonsterState GetRandomState()  // ���� ���¸� �����ϴ� �޼����Դϴ�.
    {
        int randomIndex = Random.Range(0, 5); // jump ���¸� ������ ���� ����
        switch (randomIndex)
        {
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

    private void CheckPositionChange()  // ��ġ ��ȭ�� üũ�ϴ� �޼����Դϴ�. (�Ѱ����� ��ð� �ִ°��� �����ϱ� ����)
    {
        if (Time.time - lastPositionCheckTime >= positionCheckInterval) // ���������� ��ġ�� üũ�� �ð����� ���� �ð��� �������� Ȯ���մϴ�.
        {
            float distance = Vector3.Distance(transform.position, lastPosition);// ���� ��ġ�� ������ ��ġ ������ �Ÿ��� ����մϴ�.
            if (distance < minPositionChange)// �ּ� ��ġ ��ȭ���� ������
            {
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

    private void AttackTarget()
    {
        // ���Ϳ� ��ǥ ��ġ ������ �Ÿ��� ���� ���� ���� �ִ��� Ȯ���մϴ�.
        if (Vector3.Distance(transform.position, targetPosition) < attackRange)
        {
            Debug.Log($"Agent{name} �� ������.!");
            animator.SetBool("Fight", true); // ���� �ִϸ��̼� bool �Ķ���� ����
                                             // �Ӹ� ��ġ���� �� �������� ����ĳ��Ʈ�� �߻��Ͽ� ������ �õ��մϴ�.

            // �Ͻ������� �ݶ��̴� ũ�⸦ �ø��ϴ�.
            StartCoroutine(ExpandCollider());
        }
        EndChaseAndWander();
    }

    private IEnumerator ExpandCollider()
    {
        boxCollider.size = expandedColliderSize;
        yield return new WaitForSeconds(1f); // �ݶ��̴��� Ȯ��� ���·� ������ �ð�
        boxCollider.size = originalColliderSize;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null && other.transform != this.transform && !other.CompareTag("Monster"))
        {
            other.GetComponent<IDamageable>().TakeDamage
                (damage);
        }
    }

    public void JumpAndChangeState()
    {
        ChangeState(MonsterState.Jump);

        if (stateCoroutine != null) {
            StopCoroutine(stateCoroutine);
        }
        stateCoroutine = StartCoroutine(StateDuration(GetRandomState(), 0.5f));
    }

    protected override void Die()
    {
        base.Die(); // �⺻ Die ���� ȣ��
        // ���� ������ �߰� ������ �ʿ��ϸ� ���⿡ �߰�
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // Entity�� TakeDamage ȣ��
        // ���� ������ �߰� ������ �ʿ��ϸ� ���⿡ �߰�
    }
}

