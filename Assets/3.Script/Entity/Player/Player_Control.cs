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
    private float speed_walk = 2f; // player_data에서 가져오기
    private float speed_sprint = 4f; // player_data에서 가져오기
    private float jump_height = 1f; // player_data에서 가져오기
    private float gravity_velocity = 0f;
    private float current_speed = 0f;
    private float velocity = 0f;




    private void Awake()
    {
        TryGetComponent(out controller);
        TryGetComponent(out animator);
    }

    private void Update()
    {
        //이 부분 나눈 이유는.. 나중에 lateupdate를 써야할 일이 생길때 써야되서 지금은 이렇게 쓰는게 좋을듯 합니다..

        GroundedCheck();
        Move_Control();
        Attack_Control();
    }

    private void LateUpdate()
    {
        Rotation_Control();
    }


    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded;
    
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset;
    
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public Vector3 GroundedBoxSize;
    
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;
    
    [Header("Debug")]
    [SerializeField] private bool drawGizmo;
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 box_position = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckBox(box_position, GroundedBoxSize, transform.rotation, GroundLayers);

        // update animator if using character
    }
    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;
    
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position + (transform.up * GroundedOffset), GroundedBoxSize);
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
        //!!!!!! 그라운드 체크 해야함
        if (controller.isGrounded)
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

                // 짬푸
                if (Input.GetButtonDown("Jump"))
                {
                    animator.SetBool("IsJump", true);
                    animator.SetBool("IsGround", false);
                    gravity_velocity = Mathf.Sqrt(-5f * jump_height * Physics.gravity.y);
                }
            }
        }
        else
        {
            animator.SetBool("IsGround", false);
            gravity_velocity += Physics.gravity.y * Time.deltaTime;
        }

        Move_Animation(key_h, key_v);
        controller.Move(new Vector3(0, gravity_velocity, 0) * Time.deltaTime);
    }

    private void Move_Animation(float key_h, float key_v)
    {
        float target_speed = 0f;
        if (key_h != 0f && key_v != 0f)
        {
            target_speed = Mathf.Sqrt(key_h * key_h + key_v * key_v);
            if (key_v < 0) target_speed = -target_speed;
        }
        else if (key_h == 0f && key_v != 0f) target_speed = key_v;
        else if (key_h != 0f && key_v == 0f) target_speed = key_h;
        else target_speed = 0f;

        target_speed = Input.GetKey(KeyCode.LeftControl) ? target_speed * 2f : target_speed;

        current_speed = Mathf.SmoothDamp(current_speed, target_speed, ref velocity, 0.2f);

        if (key_v != 0)
        {
            animator.SetFloat("Speed_V", current_speed);
            animator.SetFloat("Speed_H", Mathf.Lerp(animator.GetFloat("Speed_H"), 0f, Time.deltaTime));
        }
        else
        {
            animator.SetFloat("Speed_V", Mathf.Lerp(animator.GetFloat("Speed_V"), 0f, Time.deltaTime));
            animator.SetFloat("Speed_H", current_speed);
        }
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
            if (key_v == 0) temp_y = cursor_x;
            else if (((key_h < 0) && !(key_v < 0)) || ((key_h > 0) && (key_v < 0))) temp_y = cursor_x - 45f;
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
            head_transform.LookAt(rotation_anchor.transform.position + rotation_anchor.transform.forward * 5f);
        }
    }

    private float Difference(float a, float b)
    {
        if (a < b) return b - a;
        else if (a > b) return a - b;
        else return 0;
    }
}