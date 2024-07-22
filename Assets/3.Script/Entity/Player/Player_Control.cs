using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform head_transform;
    // [SerializeField] private Inventory inventory_class;

    private float cursor_h, cursor_v, key_h, key_v;
    private float cursor_x = 0f;
    private float cursor_y = 0f;
    private float temp_y = 0f;

    private float speed_current = 0f;
    private float speed_walk = 2f; // player_data���� ��������
    private float speed_sprint = 4f; // player_data���� ��������
    private float jump_height = 1f; // player_data���� ��������
    private float gravity_velocity = 0f;

    private void Awake()
    {
        TryGetComponent(out controller);
        TryGetComponent(out animator);
        head_transform = transform.GetChild(1).transform;
    }

    private void Update() // �� �κ� ���� ������.. ���߿� lateupdate�� ����� ���� ���涧 ��ߵǼ� ������ �̷��� ���°� ������ �մϴ�....
    {
        Move_Control();
    }

    private void LateUpdate() // �밡��
    {
        Head_Body_Rotate();
    }

    private void Move_ani(float key_h, float key_v)
    {
        float speed = new Vector2(key_h, key_v).magnitude;

        


        // Set the speed value to 1 when moving diagonally
        if (speed > 1)
        {
            speed = 1;
        }

        if(key_h < 0 || key_v < 0)
        {
            speed = -speed;
        }

        speed = Input.GetKey(KeyCode.LeftControl) ? speed * 2 : speed;

        animator.SetFloat("Speed", speed);
    }


    private void Move_Control()
    {
        //���ΰ�? �߷��ΰ�?
        if (controller.isGrounded)
        {
            key_h = Input.GetAxis("Horizontal");
            key_v = Input.GetAxis("Vertical");

            animator.SetBool("IsGround", true);
            animator.SetBool("IsJump", false);


            if(Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Attack");
                Debug.Log("Attack");
            }

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
            gravity_velocity += Physics.gravity.y * Time.deltaTime;
        }

        // ����
        Vector3 direction = head_transform.forward * key_v + head_transform.right * key_h;

        //�̰� ���ø� �˴ϴ�.
        //float speed = direction.normalized.magnitude; //�ٽ�!!! ���� ����ȭ�� �� ���ϱ�
        //speed = Input.GetKey(KeyCode.LeftControl) ? speed * 2f : speed;
        //if (key_v < 0f || key_h < 0f) speed = -speed;

        // �ִϸ��̼�
        //float speed_animation = Mathf.Sqrt(key_h * key_h + key_v * key_v) * speed_current;
        ////float speed_animation = speed_current;
        //if (key_v < 0f || key_h < 0f) speed_animation = -speed_animation;
        //animator.SetFloat("Speed", speed);

        //�ӵ�
        if (key_h == 0 && key_v == 0) speed_current = 0f;
        else if (key_h != 0 || key_v != 0) speed_current = Mathf.Min(direction.magnitude, 1.0f) * (Input.GetKey(KeyCode.LeftControl) ? speed_sprint : speed_walk);

        Move_ani(key_h, key_v);

        // �⺻ ���⿡ ĳ������ �̵��ӵ��� ���ؼ� ������ �ӵ� ����
        direction.y = 0f;
        controller.Move(direction.normalized * speed_current * Time.deltaTime);
        controller.Move(new Vector3(0, gravity_velocity, 0) * Time.deltaTime);
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
            if (((key_h < 0) && !(key_v < 0)) || ((key_h > 0) && (key_v < 0))) temp_y = cursor_x - 45f;
            else if (((key_h > 0) && !(key_v < 0)) || ((key_h < 0) && (key_v < 0))) temp_y = cursor_x + 45f;
            else if (key_v != 0 && key_h == 0) temp_y = cursor_x;
        }
        else
        {
            if (Difference(cursor_x, temp_y) > 45f)
            {
                if(cursor_h > 0) temp_y = cursor_x - 45f;
                else if(cursor_h < 0) temp_y = cursor_x + 45f;
            }
        }
        Quaternion target_rotation = Quaternion.Euler(transform.rotation.x, temp_y, transform.rotation.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, 5f * Time.deltaTime);
        head_transform.rotation = Quaternion.Euler(-cursor_y, cursor_x, 0);
    }

    private float Difference(float a, float b)
    {
        if (a < b) return b - a;
        else if (a > b) return a - b;
        else return 0;
    }
}