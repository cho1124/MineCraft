using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  �ڡ������հ� �̸��� ���߱����� ���ڶ� ������� ����(�ʽĵ���)�Դϴ�!!!�ڡ�
public class Lion : Animal
{
    private Transform playerTransform;

    protected override void Start() {
        base.Start();
        detectionDistance *= 3; // Raycast �Ÿ��� �� ��� ����
    }

    protected override void Update() {
        base.Update();
    }

    IEnumerator FleeSequence() {
        // �÷��̾� ������ �� ���� ���� ������ �ð�
        float circleDuration = 7f;
        float circleSpeed = 2f;
        float circleRadius = 3f;
        float startTime = Time.time;

        while (Time.time < startTime + circleDuration) {
            Vector3 offset = new Vector3(Mathf.Sin(Time.time * circleSpeed) * circleRadius, 0, Mathf.Cos(Time.time * circleSpeed) * circleRadius);
            agent.SetDestination(playerTransform.position + offset);
            yield return null;
        }

        // ���� �ð� ���� �÷��̾ ����ٴ�
        ChangeState(State.Follow);
        yield return FollowPlayer(3f);// ���⼭ FollowPlayer �ڷ�ƾ�� ȣ��˴ϴ�.

        // ���� ���·� ����
        ChangeState(GetRandomState());
        SetRandomDestination();
    }

    protected override void ChangeState(State newState) {
        currentState = newState;

        switch (currentState) {
            case State.Wander:
                agent.speed = baseSpeed;
                animator.SetInteger("Walk", 1);
                SetRandomDestination();
                break;
            case State.Jump:
             // agent.isStopped = true;
             // agent.updatePosition = false;
             // agent.updateRotation = false;
             // rb.isKinematic = false;
                animator.Play("Jump");
             //   StartCoroutine(JumpThenIdle());
                break;
            case State.Idle:
                agent.ResetPath();
                animator.Play("Idle");
                StartCoroutine(IdleThenWander());
                break;
            case State.Run:
                agent.speed = baseSpeed * 2;
                animator.SetInteger("Walk", 1);
                SetRandomDestination();
                break;
            case State.Follow:
                agent.speed = baseSpeed;
                break;
        }
    }

    private IEnumerator FollowPlayer(float duration) {
        float startTime = Time.time;
        while (Time.time < startTime + duration) {
            if (playerTransform != null) {
                agent.SetDestination(playerTransform.position);
            }
            yield return null;
        }
    }
}
