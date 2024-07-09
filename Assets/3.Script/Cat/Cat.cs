using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICatState
{
    void EnterState(Cat cat);
    void UpdateState(Cat cat);
    void ExitState(Cat cat);
}

public class Cat : MonoBehaviour
{
    public ICatState currentState;
    public StandingState standingState = new StandingState();
    public WalkingState walkingState = new WalkingState();
    public JumpingState jumpingState = new JumpingState();

    public Animator animator;
    public Rigidbody rigidbody;
    public float jumpForce = 5f;
    public Transform player;
    public float detectionRadius = 1f;

    void Start()
    {
        TransitionToState(standingState);
    }

    void Update()
    {
        DetectPlayer();
        currentState.UpdateState(this);
    }

    public void TransitionToState(ICatState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
    }

    public bool IsGrounded()
    {
        // 고양이가 땅에 닿아 있는지 확인하는 로직
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    private void DetectPlayer()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            if (!(currentState is JumpingState))
            {
                TransitionToState(jumpingState);
            }
        }
    }
}

public class StandingState : ICatState
{
    public void EnterState(Cat cat)
    {
        // 서 있는 상태에 진입할 때 호출되는 코드
        cat.animator.Play("CatIdle");
    }

    public void UpdateState(Cat cat)
    {
        // 서 있는 동안 수행할 로직
        if (Input.GetKeyDown(KeyCode.W))
        {
            cat.TransitionToState(cat.walkingState);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            cat.TransitionToState(cat.jumpingState);
        }
    }

    public void ExitState(Cat cat)
    {
        // 서 있는 상태를 떠날 때 호출되는 코드
        Debug.Log("고양이가 서있는것을 멈췄습니다. ");
    }
}

public class WalkingState : ICatState
{
    public void EnterState(Cat cat)
    {
        // 걷는 상태에 진입할 때 호출되는 코드
        cat.animator.Play("CatWalk");
    }

    public void UpdateState(Cat cat)
    {
        // 걷는 동안 수행할 로직
        if (Input.GetKeyUp(KeyCode.W))
        {
            cat.TransitionToState(cat.standingState);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            cat.TransitionToState(cat.jumpingState);
        }
    }

    public void ExitState(Cat cat)
    {
        // 걷는 상태를 떠날 때 호출되는 코드
        Debug.Log("고양이가 걷는것을 멈췄습니다. ");
    }
}

public class JumpingState : ICatState
{
    public void EnterState(Cat cat)
    {
        // 점프하는 상태에 진입할 때 호출되는 코드
        cat.animator.Play("CatJump");
        cat.rigidbody.AddForce(Vector3.up * cat.jumpForce, ForceMode.Impulse);
    }

    public void UpdateState(Cat cat)
    {
        // 점프하는 동안 수행할 로직
        if (cat.IsGrounded())
        {
            // 서 있는 상태나 걷는 상태로 돌아가기
            if (Input.GetKey(KeyCode.W))
            {
                cat.TransitionToState(cat.walkingState);
            }
            else
            {
                cat.TransitionToState(cat.standingState);
            }
        }
    }

    public void ExitState(Cat cat)
    {
        // 점프하는 상태를 떠날 때 호출되는 코드
        Debug.Log("고양이가 점프를 멈췄습니다. ");
    }
}