using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : Animal
{
    protected override void Update() {
        base.Update();
    }

    protected override void OnPlayerDetected() {
        StartCoroutine(FleeSequence());
    }

    IEnumerator FleeSequence() {
        // Jump
        ChangeState(State.Jump);
        yield return new WaitForSeconds(1.1f); // Jump duration


        // NavMeshAgent의 회전을 수동으로 업데이트
        Vector3 newDirection = -transform.forward; // 180도 회전
        transform.rotation = Quaternion.LookRotation(newDirection);

        ChangeState(State.Jump);
        yield return new WaitForSeconds(1.1f); // Jump duration

        // Run twice
        ChangeState(State.Run);
        yield return new WaitForSeconds(2f); // Duration for the first run

        // Return to a random state
        ChangeState(GetRandomState());
    }
}