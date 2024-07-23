using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Animal
{

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
        Debug.Log("Cat:FleeSequence 시작");
        // Jump
      //  ChangeState(State.Jump);
      //  yield return new WaitForSeconds(1.1f); // Jump duration

        // NavMeshAgent의 회전을 수동으로 업데이트
        Vector3 newDirection = -transform.forward; // 180도 회전
        transform.rotation = Quaternion.LookRotation(newDirection);

        // Run twice
        ChangeState(State.Run);
        yield return new WaitForSeconds(2f); // Duration for the first run
        ChangeState(State.Run);
        yield return new WaitForSeconds(2f); // Duration for the second run

        // Return to a random state
        ChangeState(GetRandomState());
        SetRandomDestination();
    }
}