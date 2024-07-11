using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Animal
{
    private enum State { Wander, Wait, Run, Jump, Follow, TurnLeft, TurnRight, TurnAround }
    private State currentState;
    private Vector3 targetPosition;
    private Animator ani;
    private Rigidbody rb;

    public Transform head; // head 오브젝트를 참조할 필드 추가
    public Transform player; // 플레이어를 참조할 필드 추가

    public float wanderRadius = 10f;
    public float wanderSpeed = 2f;
    public float runSpeed = 6f;
    public float followSpeed = 3f;
    public float waitTime = 2f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 6f;
    public float jumpForce = 1.5f;
    public float detectionDistance = 1f;
    public float playerDetectionRadius = 1f; // 플레이어 감지 범위
    public float detectionCooldown = 10f; // 플레이어 감지 후 쿨다운 시간

    private bool canDetectPlayer = true; // 플레이어 감지 가능 여부
    private bool isOrbiting = false; // 한 바퀴 도는 중인지 확인하기 위한 변수
    private float obstacleDetectionRadius = 0.5f; // 같은종끼리의 감지 범위

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
                StartCoroutine(StateDuration(State.Jump, 1f)); // 점프 후 바로 다른 상태로 전환
                break;
            case State.TurnLeft:
                StartCoroutine(Turn(-90f));
                break;
            case State.TurnRight:
                StartCoroutine(Turn(90f));
                break;
            case State.TurnAround:
                StartCoroutine(Turn(180f));
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
        int randomIndex = Random.Range(0, 12);
        switch (randomIndex)
        {
            case 0:
            case 1:
            case 8:
                return State.Wander;
            case 2:
            case 3:
            case 9:
                return State.Run;
            case 4:
            case 10:
                return State.Jump;
            case 5:
            case 11:
                return State.Wait;
            case 6:
                return State.TurnLeft;
            case 7:
                return State.TurnRight;
            case 12:
                return State.TurnAround;
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
            if (hit.collider.gameObject == this.gameObject) continue;
            if (hit.collider.CompareTag("Plane")) continue;

            if (hit.collider.CompareTag("Player"))
            {
                player = hit.transform;
                Debug.Log("Player detected!");
                JumpAndTurnAround();
                StartCoroutine(PlayerDetectionCooldown()); // 플레이어 감지 후 쿨다운 시작
                return; // 플레이어 감지 시 다른 오브젝트는 처리하지 않음
            }
            else if (hit.collider.CompareTag("Animals"))
            {
                if (Vector3.Distance(hit.collider.transform.position, transform.position) > obstacleDetectionRadius)
                {
                    continue; // 일정 거리 내에 있을 때만 회피
                }

                Debug.Log($"Obstacle detected: {this.name} : {hit.collider.name}");
                // 장애물이 감지되면 방향을 변경
                float angle = GetRandomAngle();
                transform.Rotate(0, angle, 0);
                return; // 장애물 감지 시 방향 변경 후 종료
            }
            else if (!hit.collider.CompareTag("Food"))
            {
                Debug.Log($"Obstacle detected: {this.name} : {hit.collider.name}");
                // 장애물이 감지되면 방향을 변경
                float angle;
                if (Vector3.Dot(transform.forward, hit.transform.position - transform.position) > 0) // 마주쳤을 때
                {
                    angle = Random.Range(0, 2) == 0 ? 180f : 270f;
                    transform.Rotate(0, angle, 0);
                    return; // 장애물 감지 시 방향 변경 후 종료
                }
            }
        }
    }

    private float GetRandomAngle()
    {
        int randomValue = Random.Range(0, 3); // 0, 1, 2 중 하나를 선택
        switch (randomValue)
        {
            case 0:
                return 0f; // 회전하지 않음
            case 1:
                return 90f; // 90도 회전
            case 2:
                return -90f; // -90도 회전
            default:
                return 0f; // 기본값으로 회전하지 않음
        }
    }

    private IEnumerator PlayerDetectionCooldown()
    {
        canDetectPlayer = false; // 플레이어 감지 비활성화
        yield return new WaitForSeconds(detectionCooldown); // 쿨다운 시간 대기
        canDetectPlayer = true; // 플레이어 감지 재활성화
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
        isOrbiting = true; // 한 바퀴 도는 중임을 표시
                           // 점프 동작 수행
        ChangeState(State.Jump);
        ani.Play("DogJump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f); // 점프 후 잠시 대기

        // 플레이어 주위를 도는 동작 수행
        float orbitRadius = 2f;
        int fullCircleSteps = 30; // 한 바퀴를 도는 동안의 스텝 수
        float stepAngle = 360f / fullCircleSteps; // 각 스텝마다 회전할 각도

        // 초기 위치 설정
        Vector3 initialPosition = player.position + (transform.position - player.position).normalized * orbitRadius;
        transform.position = initialPosition;

        // 플레이어 주변을 한 바퀴 돔
        for (int i = 0; i < fullCircleSteps; i++)
        {
            transform.RotateAround(player.position, Vector3.up, stepAngle);

            // 플레이어로부터 일정 반경 유지
            Vector3 directionFromPlayer = (transform.position - player.position).normalized;
            transform.position = player.position + directionFromPlayer * orbitRadius;

            yield return new WaitForSeconds(0.05f); // 각 스텝마다 잠시 대기
        }

        isOrbiting = false; // 한 바퀴 도는 작업이 끝났음을 표시

        // 플레이어를 따라다니기
        float followTime = Random.Range(2f, 5f); // 2-5초 동안 따라다님
        ChangeState(State.Follow); // Follow 상태로 전환
        yield return new WaitForSeconds(followTime);

        // Wander 상태로 전환
        GetRandomState();
    }

    private IEnumerator Turn(float angle)
    {
        float rotationSpeed = 180f; // 회전 속도 (도/초)
        float targetAngle = transform.eulerAngles.y + angle;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.1f)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetAngle, 0), step);
            yield return null; // 다음 프레임까지 대기
        }
        transform.eulerAngles = new Vector3(0, targetAngle, 0); // 정확히 목표 각도로 설정
        ChangeState(State.Wander); // 회전 후에 다시 Wander 상태로 전환
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