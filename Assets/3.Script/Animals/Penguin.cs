using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  �ڡ������հ� �̸��� ���߱����� ����̶� ������� �����Դϴ�!!!�ڡ�
// ������ �� ������ �������� ������ ���Ƽ�... �׳� ������ �����ͳ׿�....

public class Penguin : MonoBehaviour
{
    private enum State { Idle, Walk, Run, Jump, Chase, TurnLeft, TurnRight, DoubleJump }
    private State currentState; // ������ ������� 

    //������ ���� ����
    private Vector3 targetPosition; //���� ������
    private Rigidbody rb;

    //�̵� �� �ൿ �Ӽ� ����
    public float walkRadius = 10f; //������ �̵��� �� �ִ� ��Ȳ �ݰ�
    public float walkSpeed = 3f; //���� �⺻ ���� �ӵ�
    public float runSpeed = 4f; //���� �޸��� �ӵ�
    public float idleTime = 3f; //���� idle �ð�
    public float minWalkTime = 5f; //���� �ȱ� �ּ� �ð�
    public float maxWalkTime = 8f; //���� �ȱ� �ִ� �ð�
    public float jumpForce = 4f; //���� �����Ҷ� ��
    public float detectionDistance = 2f; //���� Ž�� �Ÿ�
    public float ChaseDuration = 2f; // �i�ƿ��� ���� �ð�

    // ��ġ ��ȭ ������ ���� ����
    private Vector3 lastPosition;
    private float lastPositionCheckTime;
    public float positionCheckInterval = 15f; // ��ġ�� üũ�� ����
    public float minPositionChange = 3f; // �ּ� ��ġ ��ȭ

    public bool IsChasingPlayer { get; set; } = false; // �÷��̾ ���� ������ ���θ� ����

    private Coroutine stateCoroutine;
    private int jumpCount = 0; // ���� Ƚ�� ����

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        lastPositionCheckTime = Time.time;
        ChangeState(State.Idle); // �ʱ� ���¸� Idle�� ����

    }

    private void Update()
    {
        switch (currentState) // ���� ���¿� ���� �ٸ� �ൿ�� �����մϴ�.
        {
            case State.Walk:
                Walk();
                break;
            case State.Idle:
                break;
            case State.Run:
                Run();
                break;
            case State.Jump:
                Jump();
                break;
            case State.DoubleJump:
                DoubleJump();
                break;
            case State.Chase:
                Chase();
                break;
            case State.TurnLeft:
                TurnLeft();
                break;
            case State.TurnRight:
                TurnRight();
                break;
        }
        CheckPositionChange(); // ��ġ ��ȭ�� üũ�մϴ�.
    }

    private void FixedUpdate()
    {
        //������ �������� ��� �ٷ� ����� 
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

    private void ChangeState(State NewState) // ���¸� �����ϴ� �޼����Դϴ�.
    {
        currentState = NewState; // ���� ���¸� ���ο� ���·� �����մϴ�.
        switch (currentState)
        {
            case State.Walk:
                targetPosition = GetRandomPosition(); // �ȱ� ���·� ����� �� ���ο� ��ǥ ��ġ�� �����մϴ�.
                StartCoroutine(StateDuration(State.Walk, Random.Range(minWalkTime, maxWalkTime))); // �ȱ� ������ ���� �ð��� �����մϴ�.
                break;
            case State.Idle:
                StartCoroutine(StateDuration(State.Idle, Random.Range(4f, 10f))); // ���� �ִ� ������ ���� �ð��� �����մϴ�.
                break;
            case State.Run:
                StartCoroutine(StateDuration(State.Run, Random.Range(minWalkTime, maxWalkTime)));
                break;
            case State.Jump:
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(StateDuration(State.Jump, 3f));
                break;
            case State.DoubleJump:
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(StateDuration(State.DoubleJump, 1f));
                break;
            case State.Chase:
                //  StartCoroutine(StateDuration(MonsterState.Chase, ChaseDuration));
                break;
            case State.TurnLeft:
                StartCoroutine(StateDuration(State.TurnLeft, 1f));
                break;
            case State.TurnRight:
                StartCoroutine(StateDuration(State.TurnRight, 1f));
                break;
        }
    }

    private void Walk() // �ȱ� ���¿��� ������ �ൿ�� �����ϴ� �޼����Դϴ�.
    {
        MoveTowardsTarget(walkSpeed); // ��ǥ ��ġ�� �ȱ� �ӵ��� �̵��մϴ�.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
        {
            ChangeState(State.Idle); // Idle ���·� ����
        }
    }

    private void Run()  // �޸��� ���¿��� ������ �ൿ�� �����ϴ� �޼����Դϴ�.
    {
        MoveTowardsTarget(runSpeed); // ��ǥ ��ġ�� �޸��� �ӵ��� �̵��մϴ�.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
        {
            ChangeState(State.Idle); // Idle ���·� ����
        }
    }

    private void TurnLeft()
    {
        // ������ �������� ȸ��
        transform.Rotate(0, -90, 0);
        ChangeState(State.Walk);
    }

    private void TurnRight()
    {
        // ������ ���������� ȸ��
        transform.Rotate(0, 90, 0);
        ChangeState(State.Walk);
    }

    private void Chase()
    {
        MoveTowardsTarget(runSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // ��ǥ ��ġ�� �����ߴ��� Ȯ���մϴ�.
        {
            EndChaseAndWander(); // ���� ���� �� �ٸ� ���·� ��ȯ
        }
    }

    private void Jump()
    {
        if (jumpCount < 1) // ù ��° ������ ���
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
            ChangeState(State.Jump);
        }
        else // �� ��° ������ ��� (���� ����)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount = 0;
            ChangeState(State.DoubleJump);
        }
    }

    private void DoubleJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        ChangeState(State.Idle);
    }

    private void MoveTowardsTarget(float speed) //������ targetPosition���� �̵���Ű�� ���
    {
        // targetPosition�� ���� ��ġ�� ���̸� ���ϰ�, ���� ���ͷ� ��ȯ
        Vector3 direction = (targetPosition - transform.position).normalized;

        // direction ���͸� ����Ͽ� ���ο� ��ġ�� ���
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // ������ ���� ��ġ�� newPosition���� ������Ʈ
        transform.position = newPosition;

        // ������ �̵��� �� �ٶ󺸴� ������ ����
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private Vector3 GetRandomPosition() //������ �̵��� ���ο� ���� ��ġ�� ����
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

    private IEnumerator StateDuration(State state, float duration) //������ Ư�� ���¸� ���� �ð� ���� �����ϰ�
    {
        // ������ �ð�(duration)��ŭ ���
        yield return new WaitForSeconds(duration);
        // ���� ���¸� �����ϰ� ����
        State nextState = GetRandomState();

        // ���ο� ���·� ����
        ChangeState(nextState);
    }

    private State GetRandomState()  // ���� ���¸� �����ϴ� �޼����Դϴ�.
    {
        int randomIndex = Random.Range(0, 9); // jump ���¸� ������ ���� ����
        switch (randomIndex)
        {
            case 0:
                return State.Walk;
            case 1:
                return State.Run;
            case 2:
                return State.Idle;
            case 3:
            case 7:
            case 4:
            case 8:
                if (jumpCount < 1)
                {
                    return State.Jump;
                }
                else
                {
                    return State.DoubleJump;
                }
            case 5:
                return State.TurnLeft;
            case 6:
                return State.TurnRight;
            default:
                return State.Jump;
        }
    }

    private void CheckPositionChange()  // ��ġ ��ȭ�� üũ�ϴ� �޼����Դϴ�.
    {
        if (Time.time - lastPositionCheckTime >= positionCheckInterval) // ���������� ��ġ�� üũ�� �ð����� ���� �ð��� �������� Ȯ���մϴ�.
        {
            float distance = Vector3.Distance(transform.position, lastPosition);// ���� ��ġ�� ������ ��ġ ������ �Ÿ��� ����մϴ�.
            if (distance < minPositionChange)// �ּ� ��ġ ��ȭ���� ������
            {
                transform.Rotate(0, 180, 0); // ������ 180�� ȸ����ŵ�ϴ�.
                targetPosition = transform.position + transform.forward * walkRadius; // ���ο� ��ǥ ��ġ�� �����մϴ�.
                ChangeState(State.Walk); // �ȱ� ���·� �����մϴ�.
            }
            lastPosition = transform.position; // ���� ��ġ�� ������ ��ġ�� ������Ʈ�մϴ�.
            lastPositionCheckTime = Time.time; // ���� �ð��� ������ ��ġ üũ �ð����� ������Ʈ�մϴ�.
        }
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        ChangeState(State.Chase);
    }

    public void EndChaseAndWander()
    {
        IsChasingPlayer = false;
        ChangeState(GetRandomState());
    }
}
