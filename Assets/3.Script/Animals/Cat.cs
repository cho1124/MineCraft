using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Animal
{

    protected override void Update() {
        base.Update();
    }

    protected override void OnPlayerDetected() {

        // Animal Ŭ������ �⺻ ���� ����
        canDetectPlayer = false; // Ž�� ��Ȱ��ȭ
        StartCoroutine(PlayerDetectionCooldown()); // ��Ÿ�� ����

        StartCoroutine(FleeSequence());
    }

    IEnumerator FleeSequence() {
        Debug.Log("Cat:FleeSequence ����");
        // Jump
      //  ChangeState(State.Jump);
      //  yield return new WaitForSeconds(1.1f); // Jump duration

        // NavMeshAgent�� ȸ���� �������� ������Ʈ
        Vector3 newDirection = -transform.forward; // 180�� ȸ��
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