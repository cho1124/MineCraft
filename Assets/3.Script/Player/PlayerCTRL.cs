using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCTRL : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform headTransform;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float mouseSensitivity = 100f;

    private float cursorX = 0f;
    private float cursorY = 0f;
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
        HandleMouseLook();
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

        moveDirection = transform.right * moveX + transform.forward * moveZ;
        moveDirection *= currentSpeed;

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
        Vector3 horizontalMoveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        animator.SetFloat("Speed", horizontalMoveDirection.magnitude);

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        cursorX += mouseX;
        cursorY -= mouseY;
        cursorY = Mathf.Clamp(cursorY, -90f, 90f);

        transform.rotation = Quaternion.Euler(0f, cursorX, 0f);
        headTransform.localRotation = Quaternion.Euler(cursorY, 0f, 0f);
    }

    
}
