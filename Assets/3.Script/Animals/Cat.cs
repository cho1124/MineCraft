using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Animal
{
    protected override void Update() {
        base.Update();
    }

    protected override void OnPlayerDetected() {
        Debug.Log("Cat: OnPlayerDetected 호출됨");
        StartCoroutine(FleeSequence());
    }

    IEnumerator FleeSequence() {
        Debug.Log("FleeSequence 시작");
        // Jump
        ChangeState(State.Jump);
        yield return new WaitForSeconds(1.1f); // Jump duration

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
    }
}