using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  ★★프리팹과 이름을 맞추기위해 사자라 써놨지만 맘모스(초식동물)입니다!!!★★
public class Lion : MonoBehaviour
{
    private enum State { Wander, Wait, Run, Jump, Follow }
    private State currentState;
    private Vector3 targetPosition;
    private Animator ani;
    private Rigidbody rb;

    public Transform head; // head 오브젝트를 참조할 필드 추가
    public Transform player; // 플레이어를 참조할 필드 추가

    public float wanderRadius = 5f;
    public float wanderSpeed = 1f;
    public float runSpeed = 3f;
    public float followSpeed = 2f;
    public float waitTime = 2f;
    public float minWanderTime = 5f;
    public float maxWanderTime = 8f;
    public float jumpForce = 2f;
    public float detectionDistance = 1.5f;
    public float playerDetectionRadius = 1.5f; // 플레이어 감지 범위

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
                StartCoroutine(StateDuration(State.Jump, 1f)); // 점프 후 바로 다른 상태로 전환
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
        // head 오브젝트의 forward 방향으로 이동
        Vector3 direction = head.forward;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private Vector3 GetRandomPosition()
    {
        // 무작위 방향에서 z축만 변경하고 x와 y는 현재 위치를 유지
        float randomZ = Random.Range(0, 100) < 15 ? Random.Range(-wanderRadius, 0) : Random.Range(0, wanderRadius);
        // 고양이가 z축 방향으로 이동할 때 15% 확률로 음수 방향, 85% 확률로 양수 방향으로 이동
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
                player = hit.transform; // 플레이어를 참조로 설정
                Debug.Log("Player detected!");
                StartCoroutine(JumpAndFollowPlayer());
                return; // 플레이어 감지 시 다른 오브젝트는 처리하지 않음
            }
            else if (!hit.collider.CompareTag("Plane") && !hit.collider.CompareTag("Animals"))
            {
                Debug.Log($"Obstacle detected: {this.name} : {hit.collider.name}");
                // 장애물이 감지되면 방향을 변경
                float angle = Random.Range(0, 2) == 0 ? -90f : 90f;
                transform.Rotate(0, angle, 0);
                return; // 장애물 감지 시 방향 변경 후 종료
            }
        }
    }

    private IEnumerator JumpAndFollowPlayer()
    {
        for (int i = 0; i < 2; i++)
        {
            ChangeState(State.Jump);
            ani.Play("LionJump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            yield return new WaitForSeconds(1f); // 두 번 점프 사이에 약간의 대기 시간
        }

        // 일정 시간 동안 플레이어를 따라다니기
        float followTime = Random.Range(3f, 6f); // 2-5초 동안 따라다님
        ChangeState(State.Follow); // Follow 상태로 전환
        yield return new WaitForSeconds(followTime);

        // 무작위 상태로 전환
        ChangeState(GetRandomState());
    }
}
