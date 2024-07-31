using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental;
using Entity_Data;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform head_transform;
    [SerializeField] private GameObject position_anchor;
    [SerializeField] private GameObject rotation_anchor;

    [SerializeField] private GameObject L_Hand;
    [SerializeField] private GameObject R_Hand;

    private Quaternion target_rotation;
    private float cursor_h, cursor_v, key_h, key_v;
    private float cursor_x = 0f;
    private float cursor_y = 0f;
    private float temp_y = 0f;

    private float speed_h;
    private float speed_v;

    private float jump_height = 1f; // player_data���� ��������
    private float gravity_velocity = 0f;

    private float input_cursor_h;
    private float input_cursor_v;
    
    private float input_key_h;
    private float input_key_v;

    private bool input_key_sprint = false;
    private bool input_key_jump = false;

    private float draw_rate = 0f;

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
        TryGetComponent(out entity);
        TryGetComponent(out controller);
        TryGetComponent(out animator);
        head_transform = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Head");
        position_anchor = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Head/Position_Anchor").gameObject;
        rotation_anchor = gameObject.transform.Find("Rotation_Anchor").gameObject;
        L_Hand = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Arm:Left:Upper/Arm:Left:Lower/Arm:Left:Hand").gameObject;
        R_Hand = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Arm:Right:Upper/Arm:Right:Lower/Arm:Right:Hand").gameObject;
    }

    private void Update()
    {
        GroundedCheck();
        Move_Control();
        Attack_Control();


        if(Input.GetKeyDown(KeyCode.F))
        {
            for(int i = 0; i < Inventory.instance.inv_Slot.Length; i++)
            {
                Debug.Log($"{i}�ε����� ���̵�" + Inventory.instance.inv_Slot[i].ItemID + $"{i}�ε����� ���� ���� ���� :" + Inventory.instance.inv_Slot[i].StackCurrent);
            }
        }

    }

    private void LateUpdate()
    {
        Rotation_Control();

        if (animator.GetInteger("Moveset_Number") == 3)
        {
            R_Hand.transform.LookAt(L_Hand.transform);
            //R_Hand.transform.Rotate(new Vector3(-60f, -45f, 15f));
        }
        else if (animator.GetInteger("Moveset_Number") == -3)
        {
            L_Hand.transform.LookAt(R_Hand.transform);
            //L_Hand.transform.Rotate(new Vector3(-60f, -45f, 15f));
        }
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

        if(!animator.GetBool("Is_Attacking") && (animator.GetBool("LR_Attack") || animator.GetBool("L_Attack") || animator.GetBool("R_Attack")) || animator.GetBool("Is_Guarding") || animator.GetBool("Is_Drawing"))
        {
            temp_y = cursor_x;
        }
        else if(key_h != 0 || key_v != 0)
        {
            temp_y = cursor_x;
            //if (key_v == 0) temp_y = cursor_x;
            //else if (((key_h < 0) && !(key_v < 0)) || ((key_h > 0) && (key_v < 0))) temp_y = cursor_x - 45f;
            //else if (((key_h > 0) && !(key_v < 0)) || ((key_h < 0) && (key_v < 0))) temp_y = cursor_x + 45f;
            //else if (key_v != 0 && key_h == 0) temp_y = cursor_x;
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


        if (!animator.GetBool("Is_Attacking"))
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

                // «Ǫ
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
        //controller.Move(new Vector3(0, gravity_velocity, 0) * Time.deltaTime);
    }

    private void Move_Animation(float key_h, float key_v)
    {
        speed_h = input_key_sprint ? key_h * 2f : key_h;
        speed_v = input_key_sprint ? key_v * 2f : key_v;

        animator.SetFloat("Speed_H", Mathf.Lerp(animator.GetFloat("Speed_H"), speed_h, Time.deltaTime * 3f));
        animator.SetFloat("Speed_V", Mathf.Lerp(animator.GetFloat("Speed_V"), speed_v, Time.deltaTime * 3f));
    }
    private void Attack_Control()
    {
        if (!entity.is_stunned)
        {
            if (!animator.GetBool("Is_Guarding"))
            {
                animator.SetBool("LR_Attack", is_L_down && is_R_down);
                animator.SetBool("L_Attack", is_L_down);
                animator.SetBool("R_Attack", is_R_down);
            }

            if (!animator.GetBool("Is_Attacking"))
            {
                animator.SetBool("Guard", is_guard_down);
            }
        }
        else
        {
            animator.SetBool("LR_Attack", false);
            animator.SetBool("L_Attack", false);
            animator.SetBool("R_Attack", false);
            animator.SetBool("Guard", false);
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
    }
    public void On_Attack_Trigger_Exit()
    {
        animator.SetBool("Is_Attacking", false);
    }
    public void Draw_Rate_Increase()
    {
        animator.SetBool("Is_Drawing", true);
        draw_rate += 0.1f;
    }
    public void Draw_Rate_Reset()
    {
        animator.SetBool("Is_Drawing", false);
        draw_rate = 0f;
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