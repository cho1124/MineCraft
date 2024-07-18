using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform head_transform;
    [SerializeField] private Inventory inventory_class;
    [SerializeField] private GameObject position_anchor;
    [SerializeField] private GameObject rotation_anchor;

    private Quaternion target_rotation;
    private float cursor_h, cursor_v, key_h, key_v;
    private float cursor_x = 0f;
    private float cursor_y = 0f;
    private float temp_y = 0f;

    private float speed_current = 0f;
    private float speed_walk = 2f; // player_data���� ��������
    private float speed_sprint = 4f; // player_data���� ��������
    private float jump_height = 1f; // player_data���� ��������
    private float gravity_velocity = 0f;
    private float currentSpeed = 0f;
    public float smoothTime = 0.1f;
    private float velocity = 0f;



    private void Awake()
    {
        TryGetComponent(out controller);
        TryGetComponent(out animator);
    }

    private void Update()
    {
        //�� �κ� ���� ������.. ���߿� lateupdate�� ����� ���� ���涧 ��ߵǼ� ������ �̷��� ���°� ������ �մϴ�..

        Attack_Control();
        Move_Control();
    }

    private void LateUpdate()
    {
        // ������ 1. �Ӹ��� ȸ�� �κ��� ���� ȸ���� ���ؼ� ���Ӱ� ���� �������� ġ������ ����
        Rotation_Control();
    }


    private void Move_Animation(float key_h, float key_v)
    {
        float targetSpeed = new Vector2(key_h, key_v).magnitude;

        // �밢�� ���� �ذ�
        if (targetSpeed > 1) targetSpeed = 1;

        if (key_v < 0) targetSpeed = -targetSpeed;

        targetSpeed = Input.GetKey(KeyCode.LeftControl) ? targetSpeed * 2 : targetSpeed;

        // �ε巯�� �ִϸ��̼� ó��
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocity, smoothTime);

        animator.SetFloat("Speed", currentSpeed);
    }

    private void Attack_Control()
    {
        if((Input.GetMouseButton(0) && Input.GetMouseButtonDown(1)) || (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1)))
        {
            animator.SetBool("LR_Attack", true);
        }
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            animator.SetBool("L_Attack", true);
        }
        if (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
        {
            animator.SetBool("R_Attack", true);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("Is_Guarding", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("Is_Guarding", false);
        }
    }
    private void Move_Control()
    {
        if (true)
        {
            if (!animator.GetBool("Is_Attacking") || animator.GetBool("Is_Guarding"))
            {
                key_h = Input.GetAxis("Horizontal");
                key_v = Input.GetAxis("Vertical");

                if(key_h != 0 && (animator.GetInteger("Moveset_Number") == 2 || animator.GetInteger("Moveset_Number") == -2))
                {
                    if (key_h < 0) animator.SetInteger("Moveset_Number", -2);
                    if (key_h > 0) animator.SetInteger("Moveset_Number", 2);
                }

                animator.SetBool("IsGround", true);
                animator.SetBool("IsJump", false);

                // «Ǫ
                if (Input.GetButtonDown("Jump"))
                {
                    animator.SetBool("IsJump", true);
                    gravity_velocity = Mathf.Sqrt(jump_height * -2f * Physics.gravity.y);
                }
            }
        }
        else
        {
            animator.SetBool("IsGround", false);
            gravity_velocity += Physics.gravity.y * Time.deltaTime;
        }

        // ����
        Vector3 direction = head_transform.forward * key_v + head_transform.right * key_h;

        //�ӵ�
        if (key_h == 0 && key_v == 0) speed_current = 0f;
        else if (key_h != 0 || key_v != 0) speed_current = Mathf.Min(direction.magnitude, 1.0f) * (Input.GetKey(KeyCode.LeftControl) ? speed_sprint : speed_walk);

        Move_Animation(key_h, key_v);

        // �⺻ ���⿡ ĳ������ �̵��ӵ��� ���ؼ� ������ �ӵ� ����
        direction.y = 0f;
        if(!animator.GetBool("Is_Stop"))controller.Move(direction.normalized * speed_current * Time.deltaTime);
        controller.Move(new Vector3(0, gravity_velocity, 0) * Time.deltaTime);
    }

    private void Rotation_Control()
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
        target_rotation = Quaternion.Euler(transform.rotation.x, temp_y, transform.rotation.z);

        rotation_anchor.transform.position = position_anchor.transform.position;
        rotation_anchor.transform.rotation = Quaternion.Euler(-cursor_y, cursor_x, 0);


        if (!animator.GetBool("Is_Attacking") || animator.GetBool("Is_Guarding"))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, 5f * Time.deltaTime);
            //head_transform.rotation = Quaternion.Euler(-cursor_y, cursor_x, 0);
            head_transform.LookAt(rotation_anchor.transform.position + rotation_anchor.transform.forward * 5f);
        }
    }

    private float Difference(float a, float b)
    {
        if (a < b) return b - a;
        else if (a > b) return a - b;
        else return 0;
    }

    //[Header("Boxcast Property")]
    //[SerializeField] private Vector3 boxSize;
    //[SerializeField] private float maxDistance;
    //[SerializeField] private LayerMask groundLayer;
    //
    //[Header("Debug")]
    //[SerializeField] private bool drawGizmo;
    //
    //private void OnDrawGizmos()
    //{
    //    if (!drawGizmo) return;
    //
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    //}
    //
    //public bool IsGrounded()
    //{
    //    return Physics.BoxCast(transform.position, boxSize, -transform.up, transform.rotation, maxDistance);
    //}
}