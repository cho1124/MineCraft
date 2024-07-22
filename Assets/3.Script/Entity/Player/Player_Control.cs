using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform head_transform;
    [SerializeField] private GameObject position_anchor;
    [SerializeField] private GameObject rotation_anchor;

    private Quaternion target_rotation;
    private float cursor_h, cursor_v, key_h, key_v;
    private float cursor_x = 0f;
    private float cursor_y = 0f;
    private float temp_y = 0f;

    //private float speed_current = 0f;
    //private float speed_walk = 2f; // player_data에서 가져오기
    //private float speed_sprint = 4f; // player_data에서 가져오기
    private float jump_height = 1f; // player_data에서 가져오기
    private float gravity_velocity = 0f;
    private float current_speed = 0f;
    private float velocity = 0f;

    private float input_cursor_h;
    private float input_cursor_v;
    
    private float input_key_h;
    private float input_key_v;

    private bool input_key_sprint = false;
    private bool input_key_jump = false;

    private bool is_L_down = false;
    private bool is_R_down = false;

    private bool is_guard_down = false;

    public bool Grounded;
    public float GroundedOffset;
    public Vector3 GroundedBoxSize;
    public LayerMask GroundLayers;
    
    [Header("Debug")]
    [SerializeField] private bool drawGizmo;

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
    private void Rotation_Control()
    {
        cursor_h = input_cursor_h; //new
        cursor_v = input_cursor_v; //new
        
        cursor_x += cursor_h;
        cursor_y += cursor_v;
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
    private void Move_Control()
    {
        if (Grounded)
        {
            if (!animator.GetBool("Is_Attacking"))
            {
                key_h = input_key_h;
                key_v = input_key_v;

                if(key_h != 0 && (animator.GetInteger("Moveset_Number") == 2 || animator.GetInteger("Moveset_Number") == -2))
                {
                    if (key_h < 0) animator.SetInteger("Moveset_Number", -2);
                    if (key_h > 0) animator.SetInteger("Moveset_Number", 2);
                }

                animator.SetBool("IsGround", true);
                animator.SetBool("IsJump", false);

                // 짬푸
                if (input_key_jump)
                {
                    animator.SetBool("IsJump", true);
                    animator.SetBool("IsGround", false);
                    gravity_velocity = Mathf.Sqrt(-5f * jump_height * Physics.gravity.y);
                }
            }
            else
            {
                key_h = 0f;
                key_v = 0f;
            }
        }
        else animator.SetBool("IsGround", false);

        Move_Animation(key_h, key_v);
        gravity_velocity += Physics.gravity.y * Time.deltaTime;
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

        target_speed = input_key_sprint ? target_speed * 2f : target_speed;

        current_speed = Mathf.SmoothDamp(current_speed, target_speed, ref velocity, 0.35f);

        if (key_v != 0f)
        {
            animator.SetFloat("Speed_V", current_speed);
            animator.SetFloat("Speed_H", Mathf.Lerp(animator.GetFloat("Speed_H"), 0f, Time.deltaTime)); if (-0.1f < animator.GetFloat("Speed_H") && animator.GetFloat("Speed_H") < 0.1f) animator.SetFloat("Speed_H", 0f);
        }
        else if (key_h != 0f)
        {
            animator.SetFloat("Speed_V", Mathf.Lerp(animator.GetFloat("Speed_V"), 0f, Time.deltaTime)); if (-0.1f < animator.GetFloat("Speed_V") && animator.GetFloat("Speed_V") < 0.1f) animator.SetFloat("Speed_V", 0f);
            animator.SetFloat("Speed_H", current_speed);
        }
        else
        {
            animator.SetFloat("Speed_V", Mathf.Lerp(animator.GetFloat("Speed_V"), 0f, Time.deltaTime)); if (-0.1f < animator.GetFloat("Speed_V") && animator.GetFloat("Speed_V") < 0.1f) animator.SetFloat("Speed_V", 0f);
            animator.SetFloat("Speed_H", Mathf.Lerp(animator.GetFloat("Speed_H"), 0f, Time.deltaTime)); if (-0.1f < animator.GetFloat("Speed_H") && animator.GetFloat("Speed_H") < 0.1f) animator.SetFloat("Speed_H", 0f);
        }
    }
    private void Attack_Control()
    {
        if(is_L_down && is_R_down && !animator.GetBool("Is_Attacking") && !animator.GetBool("Is_Guarding"))
        {
            animator.SetBool("LR_Attack", true);
        }
        else if (is_L_down && !animator.GetBool("Is_Attacking") && !animator.GetBool("Is_Guarding"))
        {
            animator.SetBool("L_Attack", true);
        }
        else if (is_R_down && !animator.GetBool("Is_Attacking") && !animator.GetBool("Is_Guarding"))
        {
            animator.SetBool("R_Attack", true);
        }

        if(!animator.GetBool("Is_Attacking"))
        {
            animator.SetBool("Guard", is_guard_down);
        }
    }



    public void OnRotate(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input != null)
        {
            input_cursor_h = input.x;
            input_cursor_v = input.y;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input != null)
        {
            input_key_h = input.x;
            input_key_v = input.y;
        }
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) input_key_sprint = true;
        else if (context.canceled) input_key_sprint = false;
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) input_key_jump = true;
        else input_key_jump = false;
    }

    public void OnGuard(InputAction.CallbackContext context)
    {
        if (context.performed) is_guard_down = true;
        else if (context.canceled) is_guard_down = false;
    }
    public void OnLAttack(InputAction.CallbackContext context)
    {
        if (context.performed) is_L_down = true;
        else if (context.canceled) is_L_down = false;
    }
    public void OnRAttack(InputAction.CallbackContext context)
    {
        if (context.performed) is_R_down = true;
        else if (context.canceled) is_R_down = false;
    }

    public void On_Attack_Trigger_Enter()
    {
        animator.SetBool("Is_Attacking", true);
        Debug.Log("True");
    }
    public void On_Attack_Trigger_Exit()
    {
        animator.SetBool("Is_Attacking", false);
        Debug.Log("False");
    }
    public void On_Guard_Trigger_Enter()
    {
        animator.SetBool("Is_Guarding", true);
    }
    public void On_Guard_Trigger_Exit()
    {
        animator.SetBool("Is_Guarding", false);
    }
}