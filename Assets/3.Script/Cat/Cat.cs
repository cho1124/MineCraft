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
        // ����̰� ���� ��� �ִ��� Ȯ���ϴ� ����
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
        // �� �ִ� ���¿� ������ �� ȣ��Ǵ� �ڵ�
        cat.animator.Play("CatIdle");
    }

    public void UpdateState(Cat cat)
    {
        // �� �ִ� ���� ������ ����
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
        // �� �ִ� ���¸� ���� �� ȣ��Ǵ� �ڵ�
        Debug.Log("����̰� ���ִ°��� ������ϴ�. ");
    }
}

public class WalkingState : ICatState
{
    public void EnterState(Cat cat)
    {
        // �ȴ� ���¿� ������ �� ȣ��Ǵ� �ڵ�
        cat.animator.Play("CatWalk");
    }

    public void UpdateState(Cat cat)
    {
        // �ȴ� ���� ������ ����
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
        // �ȴ� ���¸� ���� �� ȣ��Ǵ� �ڵ�
        Debug.Log("����̰� �ȴ°��� ������ϴ�. ");
    }
}

public class JumpingState : ICatState
{
    public void EnterState(Cat cat)
    {
        // �����ϴ� ���¿� ������ �� ȣ��Ǵ� �ڵ�
        cat.animator.Play("CatJump");
        cat.rigidbody.AddForce(Vector3.up * cat.jumpForce, ForceMode.Impulse);
    }

    public void UpdateState(Cat cat)
    {
        // �����ϴ� ���� ������ ����
        if (cat.IsGrounded())
        {
            // �� �ִ� ���³� �ȴ� ���·� ���ư���
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
        // �����ϴ� ���¸� ���� �� ȣ��Ǵ� �ڵ�
        Debug.Log("����̰� ������ ������ϴ�. ");
    }
}