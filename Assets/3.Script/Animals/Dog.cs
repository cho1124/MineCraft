using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    private enum State { Wander, Wait, Run, Jump, Follow }
    private State currentState;
    private Vector3 targetPosition;
    private Animator ani;
    private Rigidbody rb;

    public Transform head; // head ������Ʈ�� ������ �ʵ� �߰�
    public Transform player; // �÷��̾ ������ �ʵ� �߰�

    public float wanderRadius = 10f;
    public float wanderSpeed = 2f;
    public float runSpeed = 6f;
    public float followSpeed = 3f;
    public float waitTime = 2f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 6f;
    public float jumpForce = 1.5f;
    public float detectionDistance = 1f;
    public float playerDetectionRadius = 1f; // �÷��̾� ���� ����
    public float detectionCooldown = 10f; // �÷��̾� ���� �� ��ٿ� �ð�

    private bool canDetectPlayer = true; // �÷��̾� ���� ���� ����
    private bool isOrbiting = false; // �� ���� ���� ������ Ȯ���ϱ� ���� ����

    private void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ChangeState(State.Wander);
    }

    private void Update()
    {
        if (!isOrbiting && canDetectPlayer)
        {
            DetectObstaclesAndPlayer();
        }
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
                ani.Play("DogWalk");
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(State.Wander, Random.Range(minWanderTime, maxWanderTime)));
                break;
            case State.Wait:
                ani.Play("DogIdle");
                StartCoroutine(StateDuration(State.Wait, Random.Range(2f, 10f)));
                break;
            case State.Run:
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(State.Run, Random.Range(minWanderTime / 2, maxWanderTime / 2)));
                break;
            case State.Jump:
                ani.Play("DogJump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
                StartCoroutine(StateDuration(State.Jump, 1f)); // ���� �� �ٷ� �ٸ� ���·� ��ȯ
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
        float randomZ = Random.Range(0, 100) < 30 ? Random.Range(-wanderRadius, 0) : Random.Range(0, wanderRadius);
        // ����̰� z�� �������� �̵��� �� 30% Ȯ���� ���� ����, 70% Ȯ���� ��� �������� �̵�
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
        int randomIndex = Random.Range(0, 6);
        switch (randomIndex)
        {
            case 0:
            case 1:
                return State.Wander;
            case 2:
            case 3:
                return State.Run;
            case 4:
                return State.Jump;
            case 5:
                return State.Wait;
            default:
                return State.Wander;
        }
    }

    private void DetectObstaclesAndPlayer()
    {
        if (!canDetectPlayer) return; // �÷��̾� ���� ��Ȱ��ȭ �� �������� ����

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionDistance, transform.forward, detectionDistance);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                player = hit.transform; // �÷��̾ ������ ����
                Debug.Log("Player detected!");
                JumpAndTurnAround();
                StartCoroutine(PlayerDetectionCooldown()); // �÷��̾� ���� �� ��ٿ� ����
                return; // �÷��̾� ���� �� �ٸ� ������Ʈ�� ó������ ����
            }
            else if (!hit.collider.CompareTag("Plane")&& !hit.collider.CompareTag("Animals"))
            {
                Debug.Log($"Obstacle detected: {this.name} : {hit.collider.name}");
                // ��ֹ��� �����Ǹ� ������ ����
                float angle = Random.Range(0, 2) == 0 ? -90f : 90f;
                transform.Rotate(0, angle, 0);
                return; // ��ֹ� ���� �� ���� ���� �� ����
            }
        }
    }

    private IEnumerator PlayerDetectionCooldown()
    {
        canDetectPlayer = false; // �÷��̾� ���� ��Ȱ��ȭ
        yield return new WaitForSeconds(detectionCooldown); // ��ٿ� �ð� ���
        canDetectPlayer = true; // �÷��̾� ���� ��Ȱ��ȭ
    }

    private void JumpAndTurnAround()
    {
        if (currentState != State.Jump)
        {
            ChangeState(State.Jump);
            StartCoroutine(WalkAroundPlayer());
        }
    }

    private IEnumerator WalkAroundPlayer()
    {
        isOrbiting = true; // �� ���� ���� ������ ǥ��
                           // ���� ���� ����
        ChangeState(State.Jump);
        ani.Play("DogJump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f); // ���� �� ��� ���

        // �÷��̾� ������ ���� ���� ����
        float orbitRadius = 2f;
        int fullCircleSteps = 30; // �� ������ ���� ������ ���� ��
        float stepAngle = 360f / fullCircleSteps; // �� ���ܸ��� ȸ���� ����

        // �ʱ� ��ġ ����
        Vector3 initialPosition = player.position + (transform.position - player.position).normalized * orbitRadius;
        transform.position = initialPosition;

        // �÷��̾� �ֺ��� �� ���� ��
        for (int i = 0; i < fullCircleSteps; i++)
        {
            transform.RotateAround(player.position, Vector3.up, stepAngle);

            // �÷��̾�κ��� ���� �ݰ� ����
            Vector3 directionFromPlayer = (transform.position - player.position).normalized;
            transform.position = player.position + directionFromPlayer * orbitRadius;

            yield return new WaitForSeconds(0.05f); // �� ���ܸ��� ��� ���
        }

        isOrbiting = false; // �� ���� ���� �۾��� �������� ǥ��

        // �÷��̾ ����ٴϱ�
        float followTime = Random.Range(2f, 5f); // 2-5�� ���� ����ٴ�
        ChangeState(State.Follow); // Follow ���·� ��ȯ
        yield return new WaitForSeconds(followTime);

        // Wander ���·� ��ȯ
        GetRandomState();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Plane"))
        {
            // �浹 �� ������� ȸ���� �ʱ�ȭ�Ͽ� �ٷ� �����
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}