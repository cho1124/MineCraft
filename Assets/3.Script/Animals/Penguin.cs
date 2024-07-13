using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  �ڡ������հ� �̸��� ���߱����� ����̶� ������� �����Դϴ�!!!�ڡ�
public class Penguin : Animal
{
    private bool isSecondJump = false; // �� ��° ���� ����
    protected override void Update() {
        base.Update();
    }

    protected override State GetRandomState() {
        int random = Random.Range(0, 100);
        if (random < 25) {
            return State.Jump; // 25% Ȯ���� �Ϲ� ���� ���·� ��ȯ
        }
        else if (random < 50) {
            return State.DoubleJump; // 25% Ȯ���� 2�� ���� ���·� ��ȯ
        }
        else if (random < 75) {
            return State.Run; // 25% Ȯ���� �޸��� ���·� ��ȯ
        }
        else if (random < 90) {
            return State.Idle; // 15% Ȯ���� ��� ���·� ��ȯ
        }
        else {
            return State.Wander; // 10% Ȯ���� ��ȸ ���·� ��ȯ
        }
    }

    protected override void OnPlayerDetected() {
        StartCoroutine(FleeSequence());
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
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
                rb.isKinematic = false;
                animator.Play("Jump");
                StartCoroutine(JumpThenIdle());
                break;
            case State.DoubleJump:
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
                rb.isKinematic = false;
                animator.Play("Jump");
                StartCoroutine(DoubleJumpThenIdle());
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
        }
    }

    protected IEnumerator DoubleJumpThenIdle() {
        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse); // ù ��° ����
        yield return new WaitForSeconds(1f); // ù ��° ���� �� ��� ���

        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse); // �� ��° ����
        yield return new WaitForSeconds(1f); // �� ��° ���� �� ��� ���

        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;
        rb.isKinematic = true;
        ChangeState(State.Idle);
    }

    IEnumerator FleeSequence() {
        // Jump
        ChangeState(State.Jump);
        yield return new WaitForSeconds(1.1f); // Jump duration

        // Rotate 180 degrees
        transform.Rotate(0, 180, 0);

        ChangeState(State.Jump);
        yield return new WaitForSeconds(1.1f); // Jump duration

        // Run twice
        ChangeState(State.Run);
        yield return new WaitForSeconds(2f); // Duration for the first run

        // Return to a random state
        ChangeState(GetRandomState());
    }
}
