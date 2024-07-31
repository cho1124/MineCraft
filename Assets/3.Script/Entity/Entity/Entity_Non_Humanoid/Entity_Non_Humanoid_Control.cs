using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Entity_Non_Humanoid_Control : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform head_transform;
    [SerializeField] private GameObject position_anchor;
    [SerializeField] private GameObject rotation_anchor;

    [SerializeField] private Target_Handler target_handler;

    [SerializeField] private bool is_passive = false;

    private Quaternion target_rotation;
    private float key_h, key_v;

    private float speed_h;
    private float speed_v;

    private float jump_height = 1f;
    [SerializeField] private float gravity_velocity = 0f;

    private float input_key_h = 0f;
    private float input_key_v;

    private bool input_key_sprint = false;
    private bool input_key_jump = false;

    private bool is_L_down = false;
    private bool is_R_down = false;

    private bool is_tracking = false;

    public bool Grounded;
    public float GroundedOffset;
    public Vector3 GroundedBoxSize;
    public LayerMask GroundLayers;

    //임시로 추가된 것
    public float EntityWidth = 0.3f;
    private float verticalMomentum = 0;
    public float gravity = -9.8f;
    private Vector3 velocity;
    public bool isGrounded;
    private World world;


    [Header("Debug")]
    [SerializeField] private bool drawGizmo;

    private void Awake()
    {
        TryGetComponent(out entity);
        TryGetComponent(out controller);
        TryGetComponent(out animator);
        //head_transform = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Head");
        //position_anchor = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Head/Position_Anchor").gameObject;
        //rotation_anchor = gameObject.transform.Find("Rotation_Anchor").gameObject;
        TryGetComponent(out target_handler);
        world = FindObjectOfType<World>();
    }

    private void Update()
    {
        if ((target_handler.target != null && target_handler.target.activeSelf != false) && !is_tracking)
        {
            is_tracking = true;
            StartCoroutine(Tracking_Target_Co());
            StartCoroutine(Attack_Target_Co());
        }
        else if (target_handler.target == null || target_handler.target.activeSelf == false)
        {
            target_handler.target = null;
            is_tracking = false;
        }
        //GroundedCheck();
        Move_Control();
        Attack_Control();
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
    }

    private void LateUpdate()
    {
        Rotation_Control();
    }

    private IEnumerator Tracking_Target_Co()
    {
        while (target_handler.target != null)
        {
            input_key_v = Mathf.Lerp(input_key_v, 1f, 0.5f);
            input_key_sprint = true;
            yield return new WaitForSeconds(1.0f);
        }
        input_key_v = 0f;
        input_key_sprint = false;
        yield return null;
    }

    private IEnumerator Attack_Target_Co()
    {
        while (target_handler.target != null)
        {
            if (Vector3.Distance(target_handler.target.transform.position, transform.position) < 2f)
            {
                switch (Random.Range(0, 6))
                {
                    case 0:
                    case 1:
                        is_L_down = true;
                        is_R_down = false;
                        break;
                    case 2:
                    case 3:
                        is_L_down = false;
                        is_R_down = true;
                        break;
                    case 4:
                        is_L_down = true;
                        is_R_down = true;
                        break;
                    case 5:
                        is_L_down = false;
                        is_R_down = false;
                        yield return new WaitForSeconds(2.0f);
                        break;
                }
            }
            yield return new WaitForSeconds(0.5f);
            is_L_down = false;
            is_R_down = false;
            yield return new WaitForSeconds(0.5f);
        }
        is_L_down = false;
        is_R_down = false;
        yield return null;
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
        rotation_anchor.transform.position = position_anchor.transform.position;
        if (target_handler.target != null && target_handler.target.activeSelf != false) rotation_anchor.transform.LookAt(target_handler.target.transform);

        target_rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotation_anchor.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        if (!animator.GetBool("Is_Attacking"))
        {
            if (is_passive)
            {
                target_rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotation_anchor.transform.rotation.eulerAngles.y - 180f, transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, 5f * Time.deltaTime);
                head_transform.LookAt(rotation_anchor.transform.position + rotation_anchor.transform.forward * -5f);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, 5f * Time.deltaTime);
                head_transform.LookAt(rotation_anchor.transform.position + rotation_anchor.transform.forward * 5f);
            }
        }
    }
    private void Move_Control()
    {
        if (Grounded)
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
        //gravity_velocity += Physics.gravity.y * Time.deltaTime;
        //controller.Move(new Vector3(0, gravity_velocity, 0) * Time.deltaTime);
    }

    private void Move_Animation(float key_h, float key_v)
    {
        speed_h = input_key_sprint ? key_h * 2f : key_h;
        speed_v = input_key_sprint ? key_v * 2f : key_v;



        animator.SetFloat("Speed_H", Mathf.Lerp(animator.GetFloat("Speed_H"), speed_h, Time.deltaTime * 3f));
        animator.SetFloat("Speed_V", Mathf.Lerp(animator.GetFloat("Speed_V"), speed_v, Time.deltaTime * 3f));


        controller.Move(new Vector3(transform.forward.x, transform.position.y, transform.forward.z) * Time.deltaTime * speed_v * animator.GetFloat("Movement_Speed"));
    }
    private void Attack_Control()
    {
        if (!entity.is_stunned)
        {
            animator.SetBool("LR_Attack", is_L_down && is_R_down);
            animator.SetBool("L_Attack", is_L_down);
            animator.SetBool("R_Attack", is_R_down);
        }
        else
        {
            animator.SetBool("LR_Attack", false);
            animator.SetBool("L_Attack", false);
            animator.SetBool("R_Attack", false);
        }
    }

    public void On_Attack_Trigger_Enter()
    {
        animator.SetBool("Is_Attacking", true);
    }
    public void On_Attack_Trigger_Exit()
    {
        animator.SetBool("Is_Attacking", false);
    }

    private void CalculateVelocity()
    {

        // 중력 계산
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;


        // 걷기 & 달리기
        //if (isSprinting)
        //    velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        //else
        //    velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

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
        get
        {
            if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + EntityWidth)) ||
               world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + EntityWidth)))
            {
                if (world.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + EntityWidth)) ||
                    world.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + EntityWidth)))
                    return false;
                else
                    return true;

            }
            else
            {
                return false;
            }

            //return World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
            //       World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth));
        }
    }
    public bool back
    {
        get
        {

            if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - EntityWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - EntityWidth)))
            {
                if (world.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - EntityWidth)) ||
                    world.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - EntityWidth)))
                    return false;
                else
                    return true;

            }
            else
            {
                return false;
            }
            //return World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
            //       World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth));
        }
    }
    public bool left
    {
        get
        {

            if (world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 1f, transform.position.z)))
            {
                if (world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y, transform.position.z)) ||
                    world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 1f, transform.position.z)))
                    return false;
                else
                    return true;

            }
            else
            {
                return false;
            }



            //return world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
            //       world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z));
        }
    }
    public bool right
    {
        get
        {

            if (world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 1f, transform.position.z)))
            {
                if (world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y, transform.position.z)) ||
                    world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 1f, transform.position.z)))
                    return false;
                else
                    return true;

            }
            else
            {
                return false;
            }



            //return world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
            //       world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z));
        }
    }


}
