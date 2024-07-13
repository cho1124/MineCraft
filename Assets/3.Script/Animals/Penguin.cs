using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  ★★프리팹과 이름을 맞추기위해 펭귄이라 써놨지만 참새입니다!!!★★
public class Penguin : Animal
{
    private bool isSecondJump = false; // 두 번째 점프 여부
    protected override void Update() {
        base.Update();
    }

    protected override State GetRandomState() {
        int random = Random.Range(0, 100);
        if (random < 25) {
            return State.Jump; // 25% 확률로 일반 점프 상태로 전환
        }
        else if (random < 50) {
            return State.DoubleJump; // 25% 확률로 2단 점프 상태로 전환
        }
        else if (random < 75) {
            return State.Run; // 25% 확률로 달리기 상태로 전환
        }
        else if (random < 90) {
            return State.Idle; // 15% 확률로 대기 상태로 전환
        }
        else {
            return State.Wander; // 10% 확률로 배회 상태로 전환
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
        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse); // 첫 번째 점프
        yield return new WaitForSeconds(1f); // 첫 번째 점프 후 잠시 대기

        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse); // 두 번째 점프
        yield return new WaitForSeconds(1f); // 두 번째 점프 후 잠시 대기

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
