using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  �ڡ������հ� �̸��� ���߱����� ����̶� ������� �����Դϴ�!!!�ڡ�
public class Penguin : MonoBehaviour
{
    private enum State { Wander, Wait, Run, Jump, DoubleJump }
    private State currentState;
    private Vector3 targetPosition;
    private Animator ani;
    private Rigidbody rb;

    public Transform head; // head ������Ʈ�� ������ �ʵ� �߰�
    public Transform player; // �÷��̾ ������ �ʵ� �߰�

    public float wanderRadius = 10f;
    public float wanderSpeed = 2f;
    public float runSpeed = 8f;
    public float waitTime = 3f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 6f;
    public float jumpForce = 4f;
    public float detectionDistance = 1f;
    public float playerDetectionRadius = 1f; // �÷��̾� ���� ����
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
            case State.DoubleJump:
                // Double Jump logic handled in EnterState
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
                ani.Play("PenguinWalk");
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(State.Wander, Random.Range(minWanderTime, maxWanderTime)));
                break;
            case State.Wait:
                ani.Play("PenguinIdle");
                StartCoroutine(StateDuration(State.Wait, Random.Range(2f, 10f)));
                break;
            case State.Run:
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(State.Run, Random.Range(minWanderTime / 2, maxWanderTime / 2)));
                break;
            case State.Jump:
                ani.Play("PenguinJump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(StateDuration(State.Jump, 1f)); // ���� �� �ٷ� �ٸ� ���·� ��ȯ
                break;
            case State.DoubleJump:
                StartCoroutine(DoubleJumpRoutine());
                break;
        }
    }

    private IEnumerator DoubleJumpRoutine()
    {
        ani.Play("PenguinJump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f); // ù ���� �� ��� ���
        ani.Play("PenguinJump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f); // �� ��° ���� �� ��� ���
        ChangeState(State.Wander); // �� ��° ���� �� Wander ���·� ��ȯ
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
        int randomIndex = Random.Range(0, 9);
        switch (randomIndex)
        {
            case 0:
                return State.Wander;
            case 1:
                return State.Run;
            case 2:
            case 3:
            case 4:
                return State.Jump;
            case 5:
                return State.Wait;
            case 6:
            case 7:
            case 8:
                return State.DoubleJump;
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
                player = hit.transform;
                Debug.Log("Player detected!");
                JumpAndRunFromPlayer();
                StartCoroutine(PlayerDetectionCooldown()); // �÷��̾� ���� �� ��ٿ� ����
                return; // �÷��̾� ���� �� �ٸ� ������Ʈ�� ó������ ����
            }
            else if (!hit.collider.CompareTag("Plane") && !hit.collider.CompareTag("Animals"))
            {
                Debug.Log($"Obstacle detected:{this.name} : {hit.collider.name}");
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

    private void JumpAndRunFromPlayer()
    {
        if (currentState != State.Jump && currentState != State.DoubleJump)
        {
            StartCoroutine(JumpThenRun());
        }
    }
    private IEnumerator JumpThenRun()
    {
        for (int i = 0; i < 2; i++)
        {
            ChangeState(State.Jump);
            ani.Play("PenguinJump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            yield return new WaitForSeconds(0.5f); // �� ���� �� ��� ���
        }

        // 180�� ȸ��
        transform.Rotate(0, 180f, 0);

        // ����
        float runTime = Random.Range(2f, 5f); // 2-5�� ���� ����
        float startTime = Time.time;

        ChangeState(State.Run);
        while (Time.time < startTime + runTime)
        {
            if (player != null)
            {
                Vector3 runDirection = (transform.position - player.position).normalized;
                transform.position += runDirection * runSpeed * Time.deltaTime;
            }
            yield return null; // ���� �����ӱ��� ���
        }

        ChangeState(State.Wander); // ���� �Ŀ� �ٽ� Wander ���·� ��ȯ
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
