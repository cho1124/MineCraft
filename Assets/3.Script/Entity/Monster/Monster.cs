using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity
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
    ====> 플레이어 감지하면 플레이어에게 raycast 쏘면서 한동안 chase(7초) 함 
    =====> 플레이어 움직임 좀더 자연스럽게 움직이도록 일정 시간마다 랜덤 방향으로 회전하고 이동하고 또 일정 이후 상태 변경하도록


    확인할것
    ★ 시작하고 일정시간 지나면 갑자기 다들 제자리에서....발광함...(매우큰문제..)
    GetRandomPosition()  에 있는 목적지 생성 로그 무한으로 찎힘


    확인한것
    1. 왠지 모르겠는데 시작하자마자 볼링공처럼 한번씩 엎어짐
    => 시작 상태 walk인거를 idle로 고쳤더니 괜찮아짐 
 
    3. 플레이어랑 오브젝트들 인지하지 못하는 것 같음 
    => 플레이어는 정확하게 인식하고 있음
 
    */

    //상태 및 동작 관련 변수
    private enum MonsterState { Idle, Walk, Run, Jump, Chase, TurnLeft, TurnRight }
    private MonsterState currentState; // 몬스터의 현재상태 

    //참조를 위한 변수
    private Vector3 targetPosition; //몬스터 목적지
    private Rigidbody rb;
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

    protected override void Start()
    {
        base.Start();  // Entity 클래스의 Start 메서드 호출
        rb = GetComponent<Rigidbody>();
        obstacleDetector = GetComponentInChildren<ObstacleDetector>();
        ChangeState(MonsterState.Idle);
        lastPosition = transform.position;
        lastPositionCheckTime = Time.time;

    }

    private void Update()
    {
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
        }
    }

    private void FixedUpdate()
    {
        //몬스터가 뒤집혔을 경우 바로 세우기 
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            rb.AddTorque(Vector3.right * 10f); // 힘을 가해서 자연스럽게 스도록 유도하는 방법

            /*
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
              오브젝트를 멈추고 각도를 재설정하여 본래 상태로 만들음, 빠르나 부자연스러울 수 있음                   
             
             */
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
                StartCoroutine(StateDuration(MonsterState.Walk, Random.Range(minWalkTime, maxWalkTime))); // 걷기 상태의 지속 시간을 설정합니다.
                break;
            case MonsterState.Idle:
                StartCoroutine(StateDuration(MonsterState.Idle, Random.Range(4f, 10f))); // 멈춰 있는 상태의 지속 시간을 설정합니다.
                break;
            case MonsterState.Run:
                StartCoroutine(StateDuration(MonsterState.Run, Random.Range(minWalkTime, maxWalkTime)));
                break;
            case MonsterState.Jump:
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(StateDuration(MonsterState.Jump, 3f));
                break;
            case MonsterState.Chase:
              //  StartCoroutine(StateDuration(MonsterState.Chase, ChaseDuration));
                break;
            case MonsterState.TurnLeft:
                StartCoroutine(StateDuration(MonsterState.TurnLeft, turnDuration));
                break;
            case MonsterState.TurnRight:
                StartCoroutine(StateDuration(MonsterState.TurnRight, turnDuration));
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
        if (Vector3.Distance(transform.position,targetPosition)<0.1f) // 목표 위치에 도달했는지 확인합니다.
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
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // 목표 위치에 도달했는지 확인합니다.
        {
            EndChaseAndWander(); // 추적 종료 후 다른 상태로 전환
        }
    }

    private void MoveTowardsTarget(float speed) //몬스터를 targetPosition으로 이동시키는 기능
    {
        // targetPosition과 현재 위치의 차이를 구하고, 방향 벡터로 변환
        Vector3 direction = (targetPosition - transform.position).normalized;

        // direction 벡터를 사용하여 새로운 위치를 계산
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // 몬스터의 현재 위치를 newPosition으로 업데이트
        transform.position = newPosition;

        // 몬스터가 이동할 때 바라보는 방향을 갱신
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private Vector3 GetRandomPosition() //몬스터가 이동할 새로운 랜덤 위치를 생성
    {
        Debug.Log($"{name}새로운 목적지를 생성하겠습니다.");
        float randomAngle = Random.Range(0, 360);// 랜덤 각도를 생성합니다.
        Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward; // 랜덤 방향을 생성합니다.
        Vector3 randomPosition;
        do
        {
            randomPosition = transform.position + randomDirection * walkRadius; // 현재 위치에서 랜덤 방향으로 walkRadius만큼 떨어진 위치를 반환합니다.
        } while (Vector3.Distance(transform.position, randomPosition) < 3f); // 최소 거리 조건 추가
        return randomPosition;
    }

    private IEnumerator StateDuration(MonsterState state, float duration) //몬스터가 특정 상태를 일정 시간 동안 유지하게
    {
        // 지정된 시간(duration)만큼 대기
        yield return new WaitForSeconds(duration+5f);
        // 다음 상태를 랜덤하게 선택
        MonsterState nextState = GetRandomState();

        // 새로운 상태로 변경
        ChangeState(nextState);
    }

    private MonsterState GetRandomState()  // 랜덤 상태를 선택하는 메서드입니다.
    {
            int randomIndex = Random.Range(0, 5); // jump 상태를 제외한 상태 선택
            switch (randomIndex) {
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
    
    private void CheckPositionChange()  // 위치 변화를 체크하는 메서드입니다.
    {
        if (Time.time - lastPositionCheckTime >= positionCheckInterval) // 마지막으로 위치를 체크한 시간에서 일정 시간이 지났는지 확인합니다.
        {
            float distance = Vector3.Distance(transform.position, lastPosition);// 현재 위치와 마지막 위치 사이의 거리를 계산합니다.
            if (distance < minPositionChange)// 최소 위치 변화보다 적으면
            {
                transform.Rotate(0, 180, 0); // 몬스터를 180도 회전시킵니다.
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

    public void JumpAndChangeState()
    {
        ChangeState(MonsterState.Jump);
        ChangeState(GetRandomState());
    }

   

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Tool"))
        {
            TakeDamage(10); // 플레이어와 충돌 시 10의 데미지를 입음
        }
    }
}
    
