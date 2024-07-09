using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform head_transform;
    
    private float cursor_h, cursor_v, key_h, key_v;
    private float cursor_x = 0f;
    private float cursor_y = 0f;
    private float temp_y = 0f;

    private Vector3 direction = Vector3.zero;
    private float speed_rotate = 10f;
    private float speed_walk = 1f;
    private float speed_sprint = 10f;
    private float jump_height = 1f;
    private float gravity_velocity = 0f;

    private void Awake()
    {
        TryGetComponent(out controller);
        TryGetComponent(out animator);
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

        float speed_current;
        // 입력이 없을 때 속도 0으로 설정
        if (key_h == 0 && key_v == 0) speed_current = 0.0f;
        else speed_current = Input.GetKey(KeyCode.LeftControl) ? speed_sprint : speed_walk;

        // 방향 설정
        direction = head_transform.forward * key_v + head_transform.right * key_h;

        // LeftControl 키가 눌렸을 때 달리기 속도로 설정
        if (Input.GetKey(KeyCode.LeftControl)) direction *= speed_sprint;
        else direction *= speed_walk;

        // 지면 체크
        if (controller.isGrounded)
        {
            animator.SetBool("IsGround", true);
            animator.SetBool("IsJump", false);
        }
        else
        {
            animator.SetBool("IsGround", false);
        }

        // 점프 체크
        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetBool("IsJump", true);
                gravity_velocity = Mathf.Sqrt(jump_height * -2f * -9.81f);
            }
        }

        // 중력 적용
        gravity_velocity += -9.81f * Time.deltaTime;
        direction.y = gravity_velocity;

        // 애니메이션 속도 설정
        animator.SetFloat("Speed", speed_current);

        // 이동 적용
        controller.Move(direction * Time.deltaTime);
    }


    


    private void Head_Body_Rotate()
    {
        cursor_h = Input.GetAxis("Mouse X");
        cursor_v = Input.GetAxis("Mouse Y");
        
        cursor_x += cursor_h * 3f;
        cursor_y += cursor_v * 1.5f;
        cursor_y = Mathf.Clamp(cursor_y, -90f, 90f);

        if(key_h != 0 || key_v != 0)
        {
            if ((Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))) temp_y = cursor_x - 45f;
            else if ((Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))) temp_y = cursor_x + 45f;
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))) temp_y = cursor_x;

            Quaternion target_rotation = Quaternion.Euler(transform.rotation.x, temp_y, transform.rotation.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, speed_rotate * Time.deltaTime);
            
            head_transform.rotation = Quaternion.Euler(-cursor_y, cursor_x, 0);
        }
        else
        {
            if (Difference(cursor_x, temp_y) > 45f)
            {
                if(cursor_h > 0) temp_y = cursor_x - 45f;
                else if(cursor_h < 0) temp_y = cursor_x + 45f;
            }

            transform.rotation = Quaternion.Euler(transform.rotation.x, temp_y, transform.rotation.z);
            head_transform.rotation = Quaternion.Euler(-cursor_y, cursor_x, 0);
        }
    }

    private float Difference(float a, float b)
    {
        if (a < b) return b - a;
        else if (a > b) return a - b;
        else return 0;
    }
}