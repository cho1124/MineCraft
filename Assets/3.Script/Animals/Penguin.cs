using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  ★★프리팹과 이름을 맞추기위해 펭귄이라 써놨지만 참새입니다!!!★★
public class Penguin : MonoBehaviour
{
    private enum State { Wander, Wait, Run, Jump, DoubleJump }
    private State currentState;
    private Vector3 targetPosition;
    private Animator ani;
    private Rigidbody rb;

    public Transform head; // head 오브젝트를 참조할 필드 추가
    public Transform player; // 플레이어를 참조할 필드 추가

    public float wanderRadius = 10f;
    public float wanderSpeed = 2f;
    public float runSpeed = 8f;
    public float waitTime = 3f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 6f;
    public float jumpForce = 4f;
    public float detectionDistance = 1f;
    public float playerDetectionRadius = 1f; // 플레이어 감지 범위
    public float detectionCooldown = 10f; // 플레이어 감지 후 쿨다운 시간

    private bool canDetectPlayer = true; // 플레이어 감지 가능 여부

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
                StartCoroutine(StateDuration(State.Jump, 1f)); // 점프 후 바로 다른 상태로 전환
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
        yield return new WaitForSeconds(0.5f); // 첫 점프 후 잠시 대기
        ani.Play("PenguinJump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f); // 두 번째 점프 후 잠시 대기
        ChangeState(State.Wander); // 두 번째 점프 후 Wander 상태로 전환
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
        // head 오브젝트의 forward 방향으로 이동
        Vector3 direction = head.forward;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private Vector3 GetRandomPosition()
    {
        // 무작위 방향에서 z축만 변경하고 x와 y는 현재 위치를 유지
        float randomZ = Random.Range(0, 100) < 30 ? Random.Range(-wanderRadius, 0) : Random.Range(0, wanderRadius);
        // 고양이가 z축 방향으로 이동할 때 30% 확률로 음수 방향, 70% 확률로 양수 방향으로 이동
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

        if (!canDetectPlayer) return; // 플레이어 감지 비활성화 시 감지하지 않음

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionDistance, transform.forward, detectionDistance);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                player = hit.transform;
                Debug.Log("Player detected!");
                JumpAndRunFromPlayer();
                StartCoroutine(PlayerDetectionCooldown()); // 플레이어 감지 후 쿨다운 시작
                return; // 플레이어 감지 시 다른 오브젝트는 처리하지 않음
            }
            else if (!hit.collider.CompareTag("Plane") && !hit.collider.CompareTag("Animals"))
            {
                Debug.Log($"Obstacle detected:{this.name} : {hit.collider.name}");
                // 장애물이 감지되면 방향을 변경
                float angle = Random.Range(0, 2) == 0 ? -90f : 90f;
                transform.Rotate(0, angle, 0);
                return; // 장애물 감지 시 방향 변경 후 종료
            }
        }
    }

    private IEnumerator PlayerDetectionCooldown()
    {
        canDetectPlayer = false; // 플레이어 감지 비활성화
        yield return new WaitForSeconds(detectionCooldown); // 쿨다운 시간 대기
        canDetectPlayer = true; // 플레이어 감지 재활성화
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
            yield return new WaitForSeconds(0.5f); // 각 점프 후 잠시 대기
        }

        // 180도 회전
        transform.Rotate(0, 180f, 0);

        // 도망
        float runTime = Random.Range(2f, 5f); // 2-5초 동안 도망
        float startTime = Time.time;

        ChangeState(State.Run);
        while (Time.time < startTime + runTime)
        {
            if (player != null)
            {
                Vector3 runDirection = (transform.position - player.position).normalized;
                transform.position += runDirection * runSpeed * Time.deltaTime;
            }
            yield return null; // 다음 프레임까지 대기
        }

        ChangeState(State.Wander); // 도망 후에 다시 Wander 상태로 전환
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Plane"))
        {
            // 충돌 시 고양이의 회전을 초기화하여 바로 세우기
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}
