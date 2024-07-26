using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity, IDamageable
{
    //자유롭게 몬스터 구현 해보십쇼
    /*
    1. 몬스터는 z축으로만 움직임 (가다가 방향전환 가능)
    2. 걷기 / 달리기 / 점프(앞에 장애물이 있을때만) / 멈춰있기 상태 존재
    3. 플레이어가 특정 반경 안에 들어오면 공격 하러 다가와야함
    (커다란 큐브를 투명하게 설치해서 감지 중:trigger)
    =>장애물이 있을때 점프하거나, 왼쪽으로 회전하거나, 오른쪽으로 회전하면 좀더 움직임을 다양화 할 수 있을거 같아서 수정
    ==>움직임이 너무 앞뒤로만 움직여서 양옆으로 회전하는 움직임 추가
    ===>(혹시 어딘가 끼어 움직이지 못하고 있는 상황 대비) 15초간 3f이상의 좌표 변동이 없으면 180도 회전 하도록 해둠 

    =====> 플레이어 움직임 좀더 자연스럽게 움직이도록 일정 시간마다 랜덤 방향으로 회전하고 이동하고 또 일정 이후 상태 변경하도록
 
    <공격>
    몬스터는 ObstacleDetector 스크립트(몬스터에게 있는 오브젝트인 투명한 큐브에 있음)를 
    사용하여 플레이어와 동물을 감지
    플레이어인지 동물인지 확인하고 chaseTarget코루틴을 시작함
    chaseTarget코루틴에서는 raycast를 통해 일정시간 목표를 추적하고,
    목표가 감지되면 목표의 위치를 settarget 메서드를 활용해 몬스터에 전달->몬스터는 chase 상태가 됨->목표위치로 이동하고 공격 범위 내면 attack상태
    최대추적거리 내에 있는 동안 계속 추적
    목표가 범위를 벗어나거나 시간이 경과하면 추적을 종료하고, EndChaseAndWander 메서드를 호출하여 다시 랜덤 상태로 전환

    ★몬스터들 몬스터 스크립트 빼면 안움직이고 안죽습니다...★

    */

    //상태 및 동작 관련 변수
    private enum MonsterState { Idle, Walk, Run, Jump, Chase, TurnLeft, TurnRight, Attack }
    private MonsterState currentState; // 몬스터의 현재상태 

    //참조를 위한 변수
    private Vector3 targetPosition; //몬스터 목적지
    private ObstacleDetector obstacleDetector; //장애물감지 큐브
    public Transform head; //몬스터의 머리(head) 참조

    //이동 및 행동 속성 변수
    public float walkRadius = 5f; //몬스터가 이동할 수 있는 방황 반경
    public float walkSpeed = 3f; //몬스터 기본 걸음 속도
    public float runSpeed = 4f; //몬스터 달릴때 속도
    public float idleTime = 4f; //몬스터 idle 시간
    public float minWalkTime = 6f; //몬스터 걷기 최소 시간
    public float maxWalkTime = 12f; //몬스터 걷기 최대 시간
    public float jumpForce = 4f; //몬스터 점프할때 힘
    public float detectionDistance = 2f; //몬스터 탐지 거리
    public float ChaseDuration = 5f; // 쫒아오기 지속 시간
    public float turnDuration = 4f; // 회전 지속 시간

    // 위치 변화 추적을 위한 변수
    private Vector3 lastPosition;
    private float lastPositionCheckTime;
    public float positionCheckInterval = 15f; // 위치를 체크할 간격
    public float minPositionChange = 3f; // 최소 위치 변화

    public bool IsChasingPlayer { get; set; } = false; // 플레이어를 추적 중인지 여부를 저장

    private Coroutine stateCoroutine;
    private bool isGrounded = true; // 몬스터가 바닥에 있는지 여부를 저장

    public float attackRange = 1.5f; // 공격 범위

    private Vector3 originalColliderSize;
    private Vector3 expandedColliderSize;

    private BoxCollider boxCollider;

    //플레이어와 동물 감지 추적 관련
    private float lastPlayerDetectionTime;
    private float lastAnimalDetectionTime;
    public float detectionCooldown = 2f; // 감지 쿨다운 시간
    private Coroutine chaseCoroutine;

    protected override void Start()
    {
        base.Start();  // Entity 클래스의 Start 메서드 호출
        obstacleDetector = GetComponentInChildren<ObstacleDetector>();
        originalColliderSize = boxCollider.size;
        expandedColliderSize = originalColliderSize * 2; //몬스터와 주변 사물이 잘 충돌이 안일어나는거같아서 전투시만 콜라이더 2배크기로
        ChangeState(MonsterState.Idle);
        lastPosition = transform.position;
        lastPositionCheckTime = Time.time;

    }

    private void FixedUpdate()
    {
        //몬스터가 뒤집혔을 경우 바로 세우기 
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            rb.AddTorque(Vector3.right * 10f); // 힘을 가해서 자연스럽게 스도록 유도하는 방법

            /*
            뒤집힌걸 세울 수 있는 다른 코드
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
              오브젝트를 멈추고 각도를 재설정하여 본래 상태로 만들음, 빠르나 부자연스러울 수 있음                   
             
             */
        }

        CheckPositionChange(); // 일정 간격으로 위치 변화를 체크합니다.
        switch (currentState) // 현재 상태에 따라 다른 행동을 수행합니다.
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
            isGrounded = true; // 바닥에 착지했음을 표시
        }

        if (collision.gameObject.CompareTag("Weapon"))
        {
            TakeDamage(10); // 플레이어무기와 충돌 시 10의 데미지를 입음
            Debug.Log($"{collision}에게 {transform.name}공격받음~~");
        }
    }

    private void ChangeState(MonsterState NewState) // 상태를 변경하는 메서드입니다.
    {
        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
        }
        currentState = NewState; // 현재 상태를 새로운 상태로 변경합니다.
        switch (currentState)
        {
            case MonsterState.Walk:
                targetPosition = GetRandomPosition(); // 걷기 상태로 변경될 때 새로운 목표 위치를 설정합니다.
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.Walk, Random.Range(minWalkTime, maxWalkTime))); // 걷기 상태의 지속 시간을 설정합니다.
                break;
            case MonsterState.Idle:
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.Idle, Random.Range(4f, 10f))); // 멈춰 있는 상태의 지속 시간을 설정합니다.
                break;
            case MonsterState.Run:
                stateCoroutine = StartCoroutine(StateDuration(MonsterState.Run, Random.Range(minWalkTime, maxWalkTime)));
                break;
            case MonsterState.Jump:
                if (isGrounded)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    isGrounded = false; // 점프를 시작했음을 표시
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

    private void Walk() // 걷기 상태에서 수행할 행동을 정의하는 메서드입니다.
    {
        MoveTowardsTarget(walkSpeed); // 목표 위치로 걷기 속도로 이동합니다.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // 목표 위치에 도달했는지 확인합니다.
        {
            ChangeState(MonsterState.Idle); // Idle 상태로 변경
        }
    }

    private void Run()  // 달리기 상태에서 수행할 행동을 정의하는 메서드입니다.
    {
        MoveTowardsTarget(runSpeed); // 목표 위치로 달리기 속도로 이동합니다.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // 목표 위치에 도달했는지 확인합니다.
        {
            ChangeState(MonsterState.Idle); // Idle 상태로 변경
        }
    }

    private void TurnLeft()
    {
        // 몬스터를 왼쪽으로 회전
        transform.Rotate(0, -90, 0);
        ChangeState(MonsterState.Walk);
    }

    private void TurnRight()
    {
        // 몬스터를 오른쪽으로 회전
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

    private void MoveTowardsTarget(float speed) //몬스터를 targetPosition으로 이동시키는 기능
    {
        // targetPosition과 현재 위치의 차이를 구하고, 방향 벡터로 변환
        Vector3 direction = (targetPosition - transform.position).normalized;

        // direction 벡터를 사용하여 새로운 위치를 계산
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // 몬스터의 현재 위치를 newPosition으로 업데이트
        rb.MovePosition(newPosition);
        //Rigidbody를 사용하여 객체를 이동시키는 방법입니다. 이는 물리 엔진을 통해 이동을 처리하므로, 충돌 및 기타 물리적 상호작용을 고려하여 객체를 이동시킵니다. 

        // 몬스터가 이동할 때 바라보는 방향을 갱신
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
                // 너무 많은 시도 후에도 유효한 위치를 찾지 못하면 최소한의 이동을 함
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

    private MonsterState GetRandomState()  // 랜덤 상태를 선택하는 메서드입니다.
    {
        int randomIndex = Random.Range(0, 5); // jump 상태를 제외한 상태 선택
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

    private void CheckPositionChange()  // 위치 변화를 체크하는 메서드입니다. (한곳에만 장시간 있는것을 방지하기 위해)
    {
        if (Time.time - lastPositionCheckTime >= positionCheckInterval) // 마지막으로 위치를 체크한 시간에서 일정 시간이 지났는지 확인합니다.
        {
            float distance = Vector3.Distance(transform.position, lastPosition);// 현재 위치와 마지막 위치 사이의 거리를 계산합니다.
            if (distance < minPositionChange)// 최소 위치 변화보다 적으면
            {
                targetPosition = transform.position + transform.forward * walkRadius; // 새로운 목표 위치를 설정합니다.
                ChangeState(MonsterState.Walk); // 걷기 상태로 변경합니다.
            }
            lastPosition = transform.position; // 현재 위치를 마지막 위치로 업데이트합니다.
            lastPositionCheckTime = Time.time; // 현재 시간을 마지막 위치 체크 시간으로 업데이트합니다.
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
        // 몬스터와 목표 위치 사이의 거리가 공격 범위 내에 있는지 확인합니다.
        if (Vector3.Distance(transform.position, targetPosition) < attackRange)
        {
            Debug.Log($"Agent{name} 가 공격중.!");
            animator.SetBool("Fight", true); // 공격 애니메이션 bool 파라미터 설정
                                             // 머리 위치에서 앞 방향으로 레이캐스트를 발사하여 공격을 시도합니다.

            // 일시적으로 콜라이더 크기를 늘립니다.
            StartCoroutine(ExpandCollider());
        }
        EndChaseAndWander();
    }

    private IEnumerator ExpandCollider()
    {
        boxCollider.size = expandedColliderSize;
        yield return new WaitForSeconds(1f); // 콜라이더를 확장된 상태로 유지할 시간
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
        base.Die(); // 기본 Die 로직 호출
        // 몬스터 고유의 추가 로직이 필요하면 여기에 추가
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // Entity의 TakeDamage 호출
        // 몬스터 고유의 추가 로직이 필요하면 여기에 추가
    }
}

