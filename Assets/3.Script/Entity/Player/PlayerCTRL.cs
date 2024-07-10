using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCTRL : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform headTransform;
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;

    private float cursorX = 0f;
    private float cursorY = 0f;
    private float temp_y = 0f;
    private float yVelocity = 0f;
    private Vector3 moveDirection = Vector3.zero;
    private Animator animator;

    private void Awake()
    {
        if (characterController == null)
        {
            TryGetComponent(out characterController);
        }
        if (headTransform == null && transform.childCount > 1)
        {
            headTransform = transform.GetChild(0).transform;
        }
        
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        CheckGround();
        HandleMovement();



    }

    private void LateUpdate()
    {
        MouseMove();
    }

    private void CheckGround()
    {
        if(characterController.isGrounded)
        {
            animator.SetBool("IsGround", true);
            animator.SetBool("IsJump", false);
        }
        else
        {
            animator.SetBool("IsGround", false);
        }
    }


    private void HandleMovement()
    {
        // WASD 입력 처리
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 달리기 속도 조절
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2 : speed;

        moveDirection = headTransform.right * moveX + headTransform.forward * moveZ;
        moveDirection *= currentSpeed * playerSpeed;

        


        // 점프 처리
        if (characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetBool("IsJump", true);
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        yVelocity += gravity * Time.deltaTime;
        moveDirection.y = yVelocity;

        // 실제 이동 벡터의 크기를 Animator에 전달
        Vector3 horizontalMoveDirection = new Vector3(moveDirection.x / playerSpeed, 0, moveDirection.z / playerSpeed);
        animator.SetFloat("Speed", horizontalMoveDirection.magnitude);

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void MouseMove()
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        cursorX += h * 3f;
        cursorY += v * 1.5f;
        cursorY = Mathf.Clamp(cursorY, -90f, 90f);

        if (Difference(cursorX, temp_y) > 45f)
        {
            temp_y += h * 3f;
            transform.rotation = Quaternion.Euler(transform.rotation.x, temp_y, transform.rotation.z);
            headTransform.rotation = Quaternion.Euler(cursorY, cursorX, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, temp_y, transform.rotation.z);
            headTransform.rotation = Quaternion.Euler(cursorY, cursorX, 0);
        }
    }


    private float Difference(float a, float b)
    {
        if (a < b) return b - a;
        else if (a > b) return a - b;
        else return 0;
    }


}
