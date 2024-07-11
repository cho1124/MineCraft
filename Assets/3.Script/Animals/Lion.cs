using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  �ڡ������հ� �̸��� ���߱����� ���ڶ� ������� ����(�ʽĵ���)�Դϴ�!!!�ڡ�
public class Lion : Animal
{
    private enum State { Wander, Wait, Run, Jump, Follow }
    private State currentState;
    private Vector3 targetPosition;
    private Animator ani;
    private Rigidbody rb;

    public Transform head; // head ������Ʈ�� ������ �ʵ� �߰�
    public Transform player; // �÷��̾ ������ �ʵ� �߰�

    public float wanderRadius = 5f;
    public float wanderSpeed = 1f;
    public float runSpeed = 3f;
    public float followSpeed = 2f;
    public float waitTime = 2f;
    public float minWanderTime = 5f;
    public float maxWanderTime = 8f;
    public float jumpForce = 1f;
    public float detectionDistance = 3f;
    public float playerDetectionRadius = 3f; // �÷��̾� ���� ����
    public float detectionCooldown = 10f; // �÷��̾� ���� �� ��ٿ� �ð�

    private bool canDetectPlayer = true; // �÷��̾� ���� ���� ����

    private void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ChangeState(State.Wander);
    }

    private void Update()
    {
            DetectObstaclesAndPlayer();
        switch (currentState)
        {
            case State.Wander:
                Wander();
                break;
            case State.Wait:
                // Do nothing, waiting
                break;
            case State.Run:
                Run();
                break;
            case State.Jump:
                // Jump logic handled in EnterState
                break;
            case State.Follow:
                FollowPlayer();
                break;
        }
    }

    private void FixedUpdate()
    {
        // Check if the cat is upside down and correct its orientation
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            // If the cat is upside down, apply a torque to flip it upright
            rb.AddTorque(Vector3.right * 10f);
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.Wander:
                ani.Play("LionWalk");
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(State.Wander, Random.Range(minWanderTime, maxWanderTime)));
                break;
            case State.Wait:
                ani.Play("LionIdle");
                StartCoroutine(StateDuration(State.Wait, Random.Range(4f, 10f)));
                break;
            case State.Run:
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(State.Run, Random.Range(minWanderTime / 2, maxWanderTime / 2)));
                break;
            case State.Jump:
                ani.Play("LionJump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(StateDuration(State.Jump, 1f)); // ���� �� �ٷ� �ٸ� ���·� ��ȯ
                break;
            case State.Follow:
                ani.Play("LionWalk");
                break;

        }
    }

    private void Wander()
    {
        MoveTowardsTarget(wanderSpeed);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ChangeState(State.Wait);
        }
    }

    private void Run()
    {
        MoveTowardsTarget(runSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ChangeState(State.Wait);
        }
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * followSpeed * Time.deltaTime;
        }
    }

    private void MoveTowardsTarget(float speed)
    {
        // head ������Ʈ�� forward �������� �̵�
        Vector3 direction = head.forward;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private Vector3 GetRandomPosition()
    {
        // ������ ���⿡�� z�ุ �����ϰ� x�� y�� ���� ��ġ�� ����
        float randomZ = Random.Range(0, 100) < 15 ? Random.Range(-wanderRadius, 0) : Random.Range(0, wanderRadius);
        // ����̰� z�� �������� �̵��� �� 15% Ȯ���� ���� ����, 85% Ȯ���� ��� �������� �̵�
        return new Vector3(transform.position.x, transform.position.y, transform.position.z + randomZ);
    }

    private IEnumerator StateDuration(State state, float duration)
    {
        yield return new WaitForSeconds(duration);

        State nextState = GetRandomState();
        ChangeState(nextState);
    }

    private State GetRandomState()
    {
        int randomIndex = Random.Range(0, 4);
        switch (randomIndex)
        {
            case 0:
                return State.Wander;
            case 1:
                return State.Run;
            case 2:
                return State.Jump;
            case 3:
                return State.Wait;
            default:
                return State.Wander;
        }
    }

    private void DetectObstaclesAndPlayer()
    {
        if (!canDetectPlayer) return; // �÷��̾� ���� ��Ȱ��ȭ �� �������� ����

        // ���� �������� ����ĳ��Ʈ�� ����Ͽ� ���� ���� Ȯ��
        Vector3[] directions = {
        transform.forward,
        (transform.forward + transform.right).normalized,
        (transform.forward - transform.right).normalized
    };

        foreach (var direction in directions)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionDistance, direction, detectionDistance);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == this.gameObject) continue;

                if (hit.collider.CompareTag("Player"))
                {
                    player = hit.transform; // �÷��̾ ������ ����
                    Debug.Log("Player detected!");
                    StartCoroutine(JumpAndFollowPlayer());
                    StartCoroutine(PlayerDetectionCooldown()); // �÷��̾� ���� �� ��ٿ� ����
                    return; // �÷��̾� ���� �� �ٸ� ������Ʈ�� ó������ ����
                }
                else if (hit.collider.CompareTag("Animals"))
                {
                    Debug.Log($"Obstacle detected: {this.name} : {hit.collider.name}");
                    // ��ֹ��� �����Ǹ� ������ ����
                    float angle = Random.Range(0, 2) == 0 ? -90f : 90f;
                    transform.Rotate(0, angle, 0);
                    return; // ��ֹ� ���� �� ���� ���� �� ����
                }
                else if (!hit.collider.CompareTag("Plane"))
                {
                    Debug.Log($"Obstacle detected: {this.name} : {hit.collider.name}");
                    // ��ֹ��� �����Ǹ� ������ ����
                    float angle;
                    if (Vector3.Dot(transform.forward, hit.transform.position - transform.position) > 0) // �������� ��
                    {
                        angle = Random.Range(0, 2) == 0 ? 180f : 270f;
                        transform.Rotate(0, angle, 0);
                        return; // ��ֹ� ���� �� ���� ���� �� ����
                    }
                }
            }
        }
        }

    private IEnumerator PlayerDetectionCooldown()
    {
        canDetectPlayer = false; // �÷��̾� ���� ��Ȱ��ȭ
        yield return new WaitForSeconds(detectionCooldown); // ��ٿ� �ð� ���
        canDetectPlayer = true; // �÷��̾� ���� ��Ȱ��ȭ
    }

    private IEnumerator JumpAndFollowPlayer()
    {
        for (int i = 0; i < 2; i++)
        {
            ChangeState(State.Jump);
            ani.Play("LionJump");
            yield return new WaitForSeconds(1f); // �� �� ���� ���̿� �ణ�� ��� �ð�
        }

        // ���� �ð� ���� �÷��̾ ����ٴϱ�
        float followTime = Random.Range(3f, 6f); // 2-5�� ���� ����ٴ�
        ChangeState(State.Follow); // Follow ���·� ��ȯ
        yield return new WaitForSeconds(followTime);

        // ������ ���·� ��ȯ
        ChangeState(GetRandomState());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Plane"))
        {
            // �浹 �� ȸ���� �ʱ�ȭ�Ͽ� �ٷ� �����
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
        else if (!collision.collider.CompareTag("Plane"))
        {
            // ��ֹ��� �����Ǹ� ������ ����
            float angle= Random.Range(0, 2) == 0 ? 180f : 270f;
                transform.Rotate(0, angle, 0);
                return; // ��ֹ� ���� �� ���� ���� �� ����
        }
    }
}
