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

    public float wanderRadius = 10f;
    public float wanderSpeed = 2f;
    public float runSpeed = 6f;
    public float waitTime = 2f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 6f;
    public float jumpForce = 3f;
    public float detectionDistance = 1f;
    public Transform rayOriginTransform;

    private void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ChangeState(State.Wander);
    }

    private void Update()
    {
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

        switch (newState)
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
        DetectObstacle();
        MoveTowardsTarget(wanderSpeed);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ChangeState(State.Wait);
        }
    }

    private void Run()
    {
        DetectObstacle();
        MoveTowardsTarget(runSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ChangeState(State.Wait);
        }
    }

    private void MoveTowardsTarget(float speed)
    {
        // ���� ��ġ�� ��ǥ ��ġ�� y���� �����ϰ� z�ุ �̵��ϵ��� ����
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, targetPosition.z);
        transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
    }

    private Vector3 GetRandomPosition()
    {
        // ������ ���⿡�� z�ุ �����ϰ� x�� y�� ���� ��ġ�� ����
        float randomZ = Random.Range(0, 100) < 30 ? Random.Range(-wanderRadius, 0) : Random.Range(0, wanderRadius);
        //����̰� z�� �������� �̵��� �� 30% Ȯ���� ���� ����, 70% Ȯ���� ��� �������� �̵�
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

    private void DetectObstacle()
    {
        if (rayOriginTransform == null)
        {
            Debug.LogError("rayOriginTransform�� �������� �ʾҽ��ϴ�.");
            return;
        }

        RaycastHit hit;
        Vector3 rayOrigin = rayOriginTransform.position;
        Vector3 rayDirection = rayOriginTransform.forward * detectionDistance;

        // ����ĳ��Ʈ ����
        if (Physics.Raycast(rayOrigin, rayOriginTransform.forward, out hit, detectionDistance))
        {
            // ��ֹ��� �����Ǹ� ���带 ����� ���̸� �׸���
            Debug.DrawRay(rayOrigin, rayDirection, Color.red);

            // ��ֹ��� �����Ǹ� ������ ����
            float angle = Random.Range(0, 2) == 0 ? -90f : 90f;
            rayOriginTransform.Rotate(0, angle, 0);
        }
        else
        {
            // ��ֹ��� �������� ������ �׸��� ����� ���̸� �׸���
            Debug.DrawRay(rayOrigin, rayDirection, Color.green);
        }
    }
}