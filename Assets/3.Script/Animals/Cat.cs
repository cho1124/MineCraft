using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    private enum State { Wander, Wait, Run, Jump }
    private State currentState;
    private Vector3 targetPosition;
    private Animator ani;
    private Rigidbody rb;

    public Transform head; // head ������Ʈ�� ������ �ʵ� �߰�
    public Transform player; // �÷��̾ ������ �ʵ� �߰�

    public float wanderRadius = 10f;
    public float wanderSpeed = 2f;
    public float runSpeed = 6f;
    public float waitTime = 2f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 6f;
    public float jumpForce = 3f;
    public float detectionDistance = 1f;
    public float playerDetectionRadius = 1f; // �÷��̾� ���� ����

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
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.Wander:
                ani.Play("CatWalk");
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(State.Wander, Random.Range(minWanderTime, maxWanderTime)));
                break;
            case State.Wait:
                ani.Play("CatIdle");
                StartCoroutine(StateDuration(State.Wait, Random.Range(2f, 10f)));
                break;
            case State.Run:
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(State.Run, Random.Range(minWanderTime / 2, maxWanderTime / 2)));
                break;
            case State.Jump:
                ani.Play("CatJump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
        float randomZ = Random.Range(0, 100) < 20 ? Random.Range(-wanderRadius, 0) : Random.Range(0, wanderRadius);
        // ����̰� z�� �������� �̵��� �� 20% Ȯ���� ���� ����, 80% Ȯ���� ��� �������� �̵�
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
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionDistance, transform.forward, detectionDistance);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                player = hit.transform;
                Debug.Log("Player detected!");
                JumpAndRunFromPlayer();
                return; // �÷��̾� ���� �� �ٸ� ������Ʈ�� ó������ ����
            }
            else if (!hit.collider.CompareTag("Plane")&& !hit.collider.CompareTag("Animals"))
            {
                Debug.Log($"Obstacle detected:{this.name} : {hit.collider.name}");
                // ��ֹ��� �����Ǹ� ������ ����
                float angle = Random.Range(0, 2) == 0 ? -90f : 90f;
                transform.Rotate(0, angle, 0);
                return; // ��ֹ� ���� �� ���� ���� �� ����
            }
        }
    }

    private void JumpAndRunFromPlayer()
    {
        if (currentState != State.Jump)
        {
            StartCoroutine(JumpThenRun());
        }
    }
    private IEnumerator JumpThenRun()
    {
        // ����
        ChangeState(State.Jump);
        ani.Play("CatJump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f); // ���� �� ��� ���

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
}