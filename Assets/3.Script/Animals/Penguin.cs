using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  ★★프리팹과 이름을 맞추기위해 펭귄이라 써놨지만 참새입니다!!!★★
// 참새는 애니멀 상속 안받고 움직임 만들어보는중

public class Penguin : Animal 
    {

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update() {
        base.Update();
    }

    protected override void OnPlayerDetected() {

        // Animal 클래스의 기본 동작 수행
        canDetectPlayer = false; // 탐지 비활성화
        StartCoroutine(PlayerDetectionCooldown()); // 쿨타임 시작
        StartCoroutine(FleeSequence());
    }

    IEnumerator FleeSequence() {
        // Jump
        //  ChangeState(State.Jump);
        //  yield return new WaitForSeconds(1.1f); // Jump duration


        // NavMeshAgent의 회전을 수동으로 업데이트
        Vector3 newDirection = -transform.forward; // 180도 회전
        transform.rotation = Quaternion.LookRotation(newDirection);

        //  ChangeState(State.Jump);
        //  yield return new WaitForSeconds(1.1f); // Jump duration

        // Run twice
        ChangeState(State.Run);
        yield return new WaitForSeconds(2f); // Duration for the first run

        // Return to a random state
        ChangeState(GetRandomState());
        SetRandomDestination();
    }
}