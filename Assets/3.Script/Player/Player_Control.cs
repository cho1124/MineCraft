using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody player_rigidbody;
    [SerializeField] private Transform head_transform;
    
    private float cursor_x = 0f;
    private float cursor_y = 0f;
    private float temp_y = 0f;
    private float cursor_h;
    private float cursor_v;
    private float key_h;
    private float key_v;
    private Vector3 direction = Vector3.zero;
    private float speed_walk = 5f;
    private float speed_sprint = 10f;
    private float jump_height = 1f;
    private float gravity_velocity = 0f;

    private void Awake()
    {
        TryGetComponent(out player_rigidbody);
        head_transform = transform.GetChild(1).transform;



    }

    private void LateUpdate()
    {
        Move_Control();
    }

    private void Move_Control()
    {
        key_h = Input.GetAxis("Horizontal");
        key_v = Input.GetAxis("Vertical");

        Head_Body_Rotate();

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //is running = true;
            //속도 = 달리기 속도
        }

        direction = head_transform.forward * key_v + head_transform.right * key_h;

        if (Input.GetKeyDown(KeyCode.LeftControl)) direction *= speed_sprint;
        else direction *= speed_walk;

        //check is ground
        if (controller.isGrounded)
        {
            animator.SetBool("IsGround", true);
            animator.SetBool("IsJump", false);
        }
        else animator.SetBool("IsGround", false);

        //check is jump
        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetBool("IsJump", true);
                gravity_velocity = Mathf.Sqrt(jump_height * -2f * -9.81f);
            }
        }
        gravity_velocity += -9.81f * Time.deltaTime;
        direction.y = gravity_velocity;

        Vector3 horizontalMoveDirection = new Vector3(direction.x, 0, direction.z);
        animator.SetFloat("Speed", horizontalMoveDirection.magnitude);
        controller.Move(direction * Time.deltaTime);
    }

    private void Head_Body_Rotate()
    {
        cursor_h = Input.GetAxis("Mouse X");
        cursor_v = Input.GetAxis("Mouse Y");
        
        cursor_x += cursor_h * 3f;
        cursor_y += cursor_v * 1.5f;
        cursor_y = Mathf.Clamp(cursor_y, -90f, 90f);

        

        if (Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))) temp_y = cursor_x - 45f;
        else if (Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))) temp_y = cursor_x + 45f;
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))) temp_y = cursor_x;
        
        if (Difference(cursor_x, temp_y) > 45f) temp_y += cursor_h * 3f;



        transform.rotation = Quaternion.Euler(transform.rotation.x, temp_y, transform.rotation.z);
        head_transform.rotation = Quaternion.Euler(-cursor_y, cursor_x, 0);
    }

    


    private float Difference(float a, float b)
    {
        if (a < b) return b - a;
        else if (a > b) return a - b;
        else return 0;
    }
}