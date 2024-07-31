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

    private float jump_height = 1f; // player_data에서 가져오기
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

    //public bool Grounded;
    //public float GroundedOffset;
    //public Vector3 GroundedBoxSize;
    //public LayerMask GroundLayers;
    //
    //[Header("Debug")]
    //[SerializeField] private bool drawGizmo;

    public float EntityWidth = 0.25f;
    public float EntityHeight = 2f;
    private float verticalMomentum = 0;
    public float gravity = -9.8f;
    private Vector3 velocity;
    public bool isGrounded;
    private World world;

    private Vector3 last_position;

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

        world = FindAnyObjectByType<World>();
    }

    private void Update()
    {
        last_position = transform.position;

        Move_Control();
        Attack_Control();


        if(Input.GetKeyDown(KeyCode.F))
        {
            for(int i = 0; i < Inventory.instance.inv_Slot.Length; i++)
            {
                Debug.Log($"{i}인덱스의 아이디" + Inventory.instance.inv_Slot[i].ItemID + $"{i}인덱스의 현재 스택 개수 :" + Inventory.instance.inv_Slot[i].StackCurrent);
            }
        }

        Debug.Log($"{transform.forward}");
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

        //if (right || left)
        //{
        //    transform.position = new Vector3(last_position.x, transform.position.y, transform.position.z);
        //}
        //
        //if (front || back)
        //{
        //    transform.position = new Vector3(transform.position.x, transform.position.y, last_position.z);
        //}

        if(animator.GetFloat("Speed_V") > 0f && front)
        {
            Debug.Log("front");
            transform.position += transform.forward * -0.1f;
        }

        if (animator.GetFloat("Speed_V") < 0f && back)
        {
            Debug.Log("back");
            transform.position += transform.forward * 0.1f;
        }

        if (animator.GetFloat("Speed_H") > 0f && right)
        {
            Debug.Log("right");
            transform.position += transform.right * -0.1f;
        }

        if (animator.GetFloat("Speed_H") < 0f && left)
        {
            Debug.Log("left");
            transform.position += transform.right * 0.1f;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Check_Ground();
    }


    //private void GroundedCheck()
    //{
    //    // set sphere position, with offset
    //    Vector3 box_position = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
    //    Grounded = Physics.CheckBox(box_position, GroundedBoxSize, transform.rotation, GroundLayers);
    //    // update animator if using character
    //}
    //private void OnDrawGizmos()
    //{
    //    if (!drawGizmo) return;
    //
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawCube(transform.position + (transform.up * GroundedOffset), GroundedBoxSize);
    //}

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
        if (isGrounded)
        {
            if (!animator.GetBool("Is_Attacking"))
            {
                key_h = input_key_h;
                key_v = input_key_v;

                if (key_h != 0 && (animator.GetInteger("Moveset_Number") == 2 || animator.GetInteger("Moveset_Number") == -2))
                {
                    if (key_h < 0) animator.SetInteger("Moveset_Number", -2);
                    if (key_h > 0) animator.SetInteger("Moveset_Number", 2);
                }

                animator.SetBool("IsGround", true);
                animator.SetBool("IsJump", false);

                gravity_velocity = 0f;

                // 짬푸
                if (input_key_jump)
                {
                    animator.SetBool("IsJump", true);
                    animator.SetBool("IsGround", false);
                    gravity_velocity = jump_height * 9.8f;
                }
            }
            else
            {
                key_h = 0f;
                key_v = 0f;
            }
        }
        else
        {
            animator.SetBool("IsGround", false);
            gravity_velocity -= 9.8f * Time.deltaTime;
        }

        Move_Animation(key_h, key_v);
        //controller.Move(new Vector3(0, gravity_velocity, 0) * Time.deltaTime);
        //if(gravity_velocity != 0f) transform.position += transform.up * gravity_velocity * Time.deltaTime;
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
        //AudioManager.instance.PlayRandomSFX("Player", "Attack");
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

    private void CalculateVelocity()
    {

        // 중력 계산
        //if (verticalMomentum > gravity)
        //    verticalMomentum += Time.fixedDeltaTime * gravity;


        // 걷기 & 달리기
        //if (isSprinting)
        //    velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        //else
        //    velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        //velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        //앞뒤좌우 검사 
        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        // 떨어지거나 올라갈때
        if (velocity.y < 0)
            velocity.y = CheckDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = CheckUpSpeed(velocity.y);
    }

    private bool Check_Ground()
    {
        if (world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 0.1f, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 0.1f, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 0.1f, transform.position.z + EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 0.1f, transform.position.z + EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y - 0.1f, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y - 0.1f, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y - 0.1f, transform.position.z + EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y - 0.1f, transform.position.z + EntityWidth))
            )
        {
            Debug.Log("땅인데?");
            if (world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 0.1f, transform.position.z - EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 0.1f, transform.position.z - EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 0.1f, transform.position.z + EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 0.1f, transform.position.z + EntityWidth)))
                return false;
            else return true;
        }
        else return false;
    }


    // 떨어지는거 계산
    private float CheckDownSpeed(float downSpeed)
    {
        if (world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + downSpeed, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + downSpeed, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + downSpeed, transform.position.z + EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + downSpeed, transform.position.z + EntityWidth)))
        {
            if (world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + downSpeed, transform.position.z - EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + downSpeed, transform.position.z - EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + downSpeed, transform.position.z + EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + downSpeed, transform.position.z + EntityWidth)))
            {
                isGrounded = false;
                return downSpeed;

            }
            else
            {
                isGrounded = true;
                return 0;
            }
        }
        else
        {
            isGrounded = false;
            return downSpeed;
        }
    }

    // 올라가는거 계산
    private float CheckUpSpeed(float upSpeed)
    {
        if (world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 2f + upSpeed, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 2f + upSpeed, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 2f + upSpeed, transform.position.z + EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 2f + upSpeed, transform.position.z + EntityWidth)))
        {
            if (world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 2f + upSpeed, transform.position.z - EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 2f + upSpeed, transform.position.z - EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 2f + upSpeed, transform.position.z + EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 2f + upSpeed, transform.position.z + EntityWidth)))
                return upSpeed;
            else
                return 0;
        }
        else
        {
            return upSpeed;
        }
    }



    public bool front
    {
        //get
        //{
        //    if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + EntityWidth)) ||
        //       world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + EntityWidth)))
        //    {
        //        if (world.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + EntityWidth)) ||
        //            world.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + EntityWidth)))
        //            return false;
        //        else
        //            return true;
        //
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //
        //    //return World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
        //    //       World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth));
        //}

        get
        {
            if (world.CheckForVoxel(transform.position + transform.forward * EntityWidth + transform.up * EntityHeight * 0.1f) || world.CheckForVoxel(transform.position + transform.forward * EntityWidth + transform.up * EntityHeight))
            {
                if (world.CheckWaterForVoxel(transform.position + transform.forward * EntityWidth + transform.up * EntityHeight * 0.1f) || world.CheckWaterForVoxel(transform.position + transform.forward * EntityWidth + transform.up * EntityHeight)) return false;
                else return true;
            }
            else return false;
        }
    }
    public bool back
    {
        //get
        //{
        //    if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - EntityWidth)) ||
        //        world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - EntityWidth)))
        //    {
        //        if (world.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - EntityWidth)) ||
        //            world.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - EntityWidth)))
        //            return false;
        //        else
        //            return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //    //return World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
        //    //       World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth));
        //}

        get
        {
            if (world.CheckForVoxel(transform.position - transform.forward * EntityWidth + transform.up * EntityHeight * 0.1f) || world.CheckForVoxel(transform.position - transform.forward * EntityWidth + transform.up * EntityHeight))
            {
                if (world.CheckWaterForVoxel(transform.position - transform.forward * EntityWidth + transform.up * EntityHeight * 0.1f) || world.CheckWaterForVoxel(transform.position - transform.forward * EntityWidth + transform.up * EntityHeight)) return false;
                else return true;
            }
            else return false;
        }
    }
    public bool left
    {
        //get
        //{
        //    if (world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y, transform.position.z)) ||
        //        world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 1f, transform.position.z)))
        //    {
        //        if (world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y, transform.position.z)) ||
        //            world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 1f, transform.position.z)))
        //            return false;
        //        else
        //            return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //    //return world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
        //    //       world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z));
        //}

        get
        {
            if (world.CheckForVoxel(transform.position - transform.right * EntityWidth + transform.up * EntityHeight * 0.1f) || world.CheckForVoxel(transform.position - transform.right * EntityWidth + transform.up * EntityHeight))
            {
                if (world.CheckWaterForVoxel(transform.position - transform.right * EntityWidth + transform.up * EntityHeight * 0.1f) || world.CheckWaterForVoxel(transform.position - transform.right * EntityWidth + transform.up * EntityHeight)) return false;
                else return true;
            }
            else return false;
        }
    }
    public bool right
    {
        //get
        //{
        //    if (world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y, transform.position.z)) ||
        //        world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 1f, transform.position.z)))
        //    {
        //        if (world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y, transform.position.z)) ||
        //            world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 1f, transform.position.z)))
        //            return false;
        //        else
        //            return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //    //return world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
        //    //       world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z));
        //}

        get
        {
            if (world.CheckForVoxel(transform.position + transform.right * EntityWidth + transform.up * EntityHeight * 0.1f) || world.CheckForVoxel(transform.position + transform.right * EntityWidth + transform.up * EntityHeight))
            {
                if (world.CheckWaterForVoxel(transform.position + transform.right * EntityWidth + transform.up * EntityHeight * 0.1f) || world.CheckWaterForVoxel(transform.position + transform.right * EntityWidth + transform.up * EntityHeight)) return false;
                else return true;
            }
            else return false;
        }
    }
}