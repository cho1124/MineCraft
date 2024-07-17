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

    */

    //상태 및 동작 관련 변수
    private enum MonsterState { Idle, Walk, Run, Jump,Chase }
    private MonsterState currentState; // 몬스터의 현재상태 

    //참조를 위한 변수
    private Vector3 targetPosition; //몬스터가 공격할 물체 목표 위치
    private Rigidbody rb;
    private ObstacleDetector obstacleDetector; //장애물감지 큐브
    public Transform head; //몬스터의 머리(head) 참조
    public Transform player; //플레이어 오브젝트
    public Transform animal; // 동물 오브젝트 참조 추가

    //이동 및 행동 속성 변수
    public float walkRadius = 10; //몬스터가 이동할 수 있는 방황 반경
    public float walkSpeed = 3; //몬스터 기본 걸음 속도
    public float runSpeed = 4; //몬스터 달릴때 속도
    public float idleTime = 4f; //몬스터 idle 시간
    public float minWalkTime = 4; //몬스터 걷기 최소 시간
    public float maxWalkTime = 6; //몬스터 걷기 최대 시간
    public float jumpForce = 4f; //몬스터 점프할때 힘
    public float detectionDistance = 2f; //몬스터 탐지 거리
    public float ChaseDuration = 7f; // 쫒아오기 지속 시간

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        obstacleDetector = GetComponentInChildren<ObstacleDetector>();
        ChangeState(MonsterState.Walk);

    }
    private void Update()
    {
        switch(currentState)
        {
            case MonsterState.Walk:
                Walk();
                break;
            case MonsterState.Idle:
                //멈춰있는 코드를 넣자
                break;
            case MonsterState.Run:
                Run();
                break;
            case MonsterState.Jump:
                //점프
                break;
            case MonsterState.Chase:
                Chase();
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

    private void Chase()
    {
        //플레이어나 동물을 감지하면 그쪽으로 이동
        if(obstacleDetector.isPlayerDeteched)
        {
            targetPosition = player.position;
        }
        else if(obstacleDetector.isAnimalDeteched)
        {
            targetPosition = animal.position;
        }
        MoveTowardsTarget(runSpeed);
    }

    private void MoveTowardsTarget(float speed)
    {
        // head 오브젝트의 forward 방향으로 이동
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private Vector3 GetRandomPosition()
    {
        //무작위 방향에서 z축만 변경하고 x와 y는 현재 위치를 유지
        float randomZ = Random.Range(0, 100) < 30 ? Random.Range(-walkRadius, 0) : Random.Range(0, walkRadius);
        return new Vector3(transform.position.x, transform.position.y, transform.position.z + randomZ);
    }

    private IEnumerator StateDuration(MonsterState state, float duration)
    {
        yield return new WaitForSeconds(duration);

        MonsterState nextState = GetRandomState();
        ChangeState(nextState);
    }

    private MonsterState GetRandomState()
    {
        if (obstacleDetector.isPlayerDeteched || obstacleDetector.isAnimalDeteched)
        {
            return MonsterState.Chase;
        }
        else if (obstacleDetector.isObstacleDeteched)
        {
            return MonsterState.Jump;
        }
        else
        {
            int randomIndex = Random.Range(0, 3); //jump 상태를 제외한 상태 선택
            switch (randomIndex)
            {
                case 0:
                    return MonsterState.Walk;
                case 1:
                    return MonsterState.Run;
                case 2:
                    return MonsterState.Idle;
                default:
                    return MonsterState.Walk;
            }
        }
    }
}
    
