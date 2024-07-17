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
    (raycast 로 정면 체크해도 좋고, 정면에 커다란 오브젝트 투명하게 설치해서 감지해도)
    =>장애물이 있을때 점프하거나, 왼쪽으로 회전하거나, 오른쪽으로 회전하면 좀더 움직임을 다양화 할 수 있을거 같아서 수정
    ==>움직임이 너무 앞뒤로만 움직여서 양옆으로 회전하는 움직임 추가
    ===>(혹시 어딘가 끼어 움직이지 못하고 있는 상황 대비) 15초간 3f이상의 좌표 변동이 없으면 180도 회전 하도록 해둠 
    */

    /*

    지금 고쳐야 하는거
    1. 왠지 모르겠는데 시작하자마자 볼링공처럼 한번씩 엎어짐
    2. 뒤로감
    3. 플레이어랑 오브젝트들 인지하지 못하는 것 같음 
    확인할것


    */

    //상태 및 동작 관련 변수
    private enum MonsterState { Idle, Walk, Run, Jump,Chase, TurnLeft, TurnRight }
    private MonsterState currentState; // 몬스터의 현재상태 

    //참조를 위한 변수
    private Vector3 targetPosition; //몬스터 목적지
    private Rigidbody rb;
    private ObstacleDetector obstacleDetector; //장애물감지 큐브
    public Transform head; //몬스터의 머리(head) 참조

    //이동 및 행동 속성 변수
    public float walkRadius = 10f; //몬스터가 이동할 수 있는 방황 반경
    public float walkSpeed = 3f; //몬스터 기본 걸음 속도
    public float runSpeed = 4f; //몬스터 달릴때 속도
    public float idleTime = 4f; //몬스터 idle 시간
    public float minWalkTime = 4f; //몬스터 걷기 최소 시간
    public float maxWalkTime = 6f; //몬스터 걷기 최대 시간
    public float jumpForce = 4f; //몬스터 점프할때 힘
    public float detectionDistance = 2f; //몬스터 탐지 거리
    public float ChaseDuration = 7f; // 쫒아오기 지속 시간
    public float turnDuration = 1f; // 회전 지속 시간

    // 위치 변화 추적을 위한 변수
    private Vector3 lastPosition;
    private float lastPositionCheckTime;
    public float positionCheckInterval = 15f; // 위치를 체크할 간격
    public float minPositionChange = 3f; // 최소 위치 변화

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        obstacleDetector = GetComponentInChildren<ObstacleDetector>();
        ChangeState(MonsterState.Walk);
        lastPosition = transform.position;
        lastPositionCheckTime = Time.time;

    }
    private void Update()
    {
        CheckPositionChange();
        switch (currentState) {
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
        if(Vector3.Dot(transform.up,Vector3.down)>0.5f)
        {
            rb.AddTorque(Vector3.right * 10f);
        }
    }

    private void ChangeState(MonsterState NewState)
    {
        currentState = NewState;
        switch(currentState)
        {
            case MonsterState.Walk:
                targetPosition = GetRandomPosition();
                StartCoroutine(StateDuration(MonsterState.Walk, Random.Range(minWalkTime, maxWalkTime)));
                break;
            case MonsterState.Idle:
                StartCoroutine(StateDuration(MonsterState.Idle, Random.Range(2f, 10f)));
                break;
            case MonsterState.Run:
                targetPosition= GetRandomPosition();
                StartCoroutine(StateDuration(MonsterState.Run, Random.Range(minWalkTime, maxWalkTime)));
                break;
            case MonsterState.Jump:
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(StateDuration(MonsterState.Jump, 3f));
                break;
            case MonsterState.Chase:
                StartCoroutine(StateDuration(MonsterState.Chase, ChaseDuration));
                break;
            case MonsterState.TurnLeft:
                targetPosition = transform.position + Quaternion.Euler(0, -90, 0) * transform.forward * walkRadius;
                StartCoroutine(StateDuration(MonsterState.TurnLeft, turnDuration));
                break;
            case MonsterState.TurnRight:
                targetPosition = transform.position + Quaternion.Euler(0, 90, 0) * transform.forward * walkRadius;
                StartCoroutine(StateDuration(MonsterState.TurnRight, turnDuration));
                break;
        }
    }
    private void Walk()
    {
        MoveTowardsTarget(walkSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ChangeState(MonsterState.Idle);
        }
    }

    private void Run()
    {
        MoveTowardsTarget(runSpeed);
        if(Vector3.Distance(transform.position,targetPosition)<0.1f)
        {
            ChangeState(MonsterState.Idle);
        }
    }
    private void TurnLeft() {
        // 몬스터를 왼쪽으로 회전
        transform.Rotate(0, -90, 0);
        ChangeState(MonsterState.Walk);
    }

    private void TurnRight() {
        // 몬스터를 오른쪽으로 회전
        transform.Rotate(0, 90, 0);
        ChangeState(MonsterState.Walk);
    }

    private void Chase() {
        //플레이어나 동물을 감지하면 그쪽으로 이동
        if (obstacleDetector.isPlayerDeteched) {
            // Player의 위치를 targetPosition으로 설정
            targetPosition = obstacleDetector.isPlayerDeteched ? GameObject.FindWithTag("Player").transform.position : targetPosition;
        }
        else if (obstacleDetector.isAnimalDeteched) {
            // Animal의 위치를 targetPosition으로 설정
            targetPosition = obstacleDetector.isAnimalDeteched ? GameObject.FindWithTag("Animals").transform.position : targetPosition;
        }
        MoveTowardsTarget(runSpeed);
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

    private Vector3 GetRandomPosition() //몬스터가 이동할 새로운 랜덤 위치를 생성 (z축 좌표만 무작위로 변경)
    {
        //무작위 방향에서 z축만 변경하고 x와 y는 현재 위치를 유지
        // 20% 확률로 음수 방향, 80% 확률로 양수 방향으로 이동
        float randomZ = Random.Range(0, 100) < 20 ? Random.Range(-walkRadius, 0) : Random.Range(0, walkRadius);

        // 현재 위치에서 z축만 randomZ만큼 변경된 위치를 반환
        return new Vector3(transform.position.x, transform.position.y, transform.position.z + randomZ);
    }

    private IEnumerator StateDuration(MonsterState state, float duration) //몬스터가 특정 상태를 일정 시간 동안 유지하게
    {
        // 지정된 시간(duration)만큼 대기
        yield return new WaitForSeconds(duration);

        // 다음 상태를 랜덤하게 선택
        MonsterState nextState = GetRandomState();

        // 새로운 상태로 변경
        ChangeState(nextState);
    }

    private MonsterState GetRandomState() {
        if (obstacleDetector.isPlayerDeteched
            || obstacleDetector.isAnimalDeteched) {
            return MonsterState.Chase;
        }
        else if (obstacleDetector.isObstacleDeteched) {
            int randomAction = Random.Range(0, 3); // 점프, 좌회전, 우회전 중 하나 선택
            switch (randomAction) {
                case 0:
                    return MonsterState.Jump;
                case 1:
                    return MonsterState.TurnLeft;
                case 2:
                    return MonsterState.TurnRight;
                default:
                    return MonsterState.Jump;
            }
        }
        else {
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
    }

    private void CheckPositionChange() {
        if (Time.time - lastPositionCheckTime >= positionCheckInterval) {
            float distance = Vector3.Distance(transform.position, lastPosition);
            if (distance < minPositionChange) {
                // 15초간 3f 이상의 위치 변화가 없을 경우 180도 회전
                transform.Rotate(0, 180, 0);
                targetPosition = transform.position + transform.forward * walkRadius;
                ChangeState(MonsterState.Walk);
            }
            lastPosition = transform.position;
            lastPositionCheckTime = Time.time;
        }
    }
}
    
