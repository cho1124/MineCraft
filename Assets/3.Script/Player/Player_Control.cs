using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform head_transform;
    [SerializeField] private float moveSpeed; //�̵��ӵ� ������ �װ�
    
    private float cursor_h, cursor_v, key_h, key_v;
    private float cursor_x = 0f;
    private float cursor_y = 0f;
    private float temp_y = 0f;

    private Vector3 direction = Vector3.zero;
    private float speed_rotate = 10f;
    private float speed_walk = 1f;
    private float speed_sprint = 2f;
    private float jump_height = 1f;
    private float gravity_velocity = 0f;

    private void Awake()
    {
        TryGetComponent(out controller);
        TryGetComponent(out animator);
        head_transform = transform.GetChild(1).transform;
    }

    private void Update()
    {
        //�� �κ� ���� ������.. ���߿� lateupdate�� ����� ���� ���涧 ��ߵǼ� ������ �̷��� ���°� ������ �մϴ�..
        Move_Control();
    }


    private void LateUpdate()
    {
        
        // �밡��
        Head_Body_Rotate();
    }

    private void Move_Control()
    {
        // �Է�
        float key_h = Input.GetAxis("Horizontal");
        float key_v = Input.GetAxis("Vertical");

        // �ӵ�
        float speed_current = Input.GetKey(KeyCode.LeftControl) ? speed_sprint : speed_walk;

        // �ִϸ��̼�
        float speed = Mathf.Sqrt(key_h * key_h + key_v * key_v) * speed_current;

        if (key_v < 0f || key_h < 0f)
        {
            speed = -speed;
        }

        animator.SetFloat("Speed", speed);

        


        Vector3 direction = head_transform.forward * key_v + head_transform.right * key_h;

        // ������
        if (controller.isGrounded)
        {
            animator.SetBool("IsGround", true);
            animator.SetBool("IsJump", false);

            // «Ǫ
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetBool("IsJump", true);
                gravity_velocity = Mathf.Sqrt(jump_height * -2f * Physics.gravity.y);
            }
        }
        else
        {
            animator.SetBool("IsGround", false);
        }

        // �߷����� -> ĳ���� ��Ʈ�ѷ��̱� ����
        gravity_velocity += Physics.gravity.y * Time.deltaTime;
        direction.y = gravity_velocity;

        // �⺻ ���⿡ ĳ������ �̵��ӵ��� ���ؼ� ������ �ӵ� ����
        controller.Move(direction * Time.deltaTime * moveSpeed);
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