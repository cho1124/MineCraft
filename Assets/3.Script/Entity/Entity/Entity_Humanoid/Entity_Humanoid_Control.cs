using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Humanoid_Control : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform head_transform;
    [SerializeField] private GameObject position_anchor;
    [SerializeField] private GameObject rotation_anchor;

    [SerializeField] private GameObject L_Hand;
    [SerializeField] private GameObject R_Hand;

    [SerializeField] private Target_Handler target_handler;

    private Quaternion target_rotation;
    private float key_h, key_v;

    private float speed_h;
    private float speed_v;

    private float jump_height = 1f; // player_data¿¡¼­ °¡Á®¿À±â
    private float gravity_velocity = 0f;

    private float input_key_h = 0f;
    private float input_key_v;

    private bool input_key_sprint = false;
    private bool input_key_jump = false;

    private float draw_rate = 0f;

    private bool is_L_down = false;
    private bool is_R_down = false;

    private bool is_guard_down = false;

    private bool is_tracking = false;

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
        head_transform = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Head");
        position_anchor = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Head/Position_Anchor").gameObject;
        rotation_anchor = gameObject.transform.Find("Rotation_Anchor").gameObject;
        L_Hand = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Arm:Left:Upper/Arm:Left:Lower/Arm:Left:Hand").gameObject;
        R_Hand = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Arm:Right:Upper/Arm:Right:Lower/Arm:Right:Hand").gameObject;
        TryGetComponent(out target_handler);
    }

    private void Update()
    {
        if((target_handler.target != null && target_handler.target.activeSelf != false) && !is_tracking)
        {
            is_tracking = true;
            StartCoroutine(Tracking_Target_Co());
            StartCoroutine(Attack_Target_Co());
        }
        else if (target_handler.target == null || target_handler.target.activeSelf == false)
        {
            target_handler.target = null;
            is_tracking = false;
            StopCoroutine(Tracking_Target_Co());
            StopCoroutine(Attack_Target_Co());
        }
        GroundedCheck();
        Move_Control();
        Attack_Control();
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

    private IEnumerator Tracking_Target_Co()
    {
        while(target_handler.target != null)
        {
            if (Vector3.Distance(target_handler.target.transform.position, transform.position) >= 2f)
            {
                input_key_v = Mathf.Lerp(input_key_v, 1f, 0.5f);
                input_key_sprint = true;
            }
            else if (1f < Vector3.Distance(target_handler.target.transform.position, transform.position) && Vector3.Distance(target_handler.target.transform.position, transform.position) < 2f)
            {
                input_key_v = Mathf.Lerp(input_key_v, -1f, 0.5f);
                input_key_sprint = false;
            } 
            else
            {
                input_key_v = Mathf.Lerp(input_key_v, 0f, 0.5f);
                input_key_sprint = false;
                yield return new WaitForSeconds(1.0f);
            }
            yield return new WaitForSeconds(1.0f);
        }
        input_key_sprint = false;
        yield return null;
    }

    private IEnumerator Attack_Target_Co()
    {
        while(target_handler.target != null)
        {
            if (Vector3.Distance(target_handler.target.transform.position, transform.position) < 2f)
            {
                switch(Random.Range(0, 6))
                {
                    case 0: case 1:
                        is_L_down = true;
                        is_R_down = false;
                        is_guard_down = false;
                        break;
                    case 2: case 3:
                        is_L_down = false;
                        is_R_down = true;
                        is_guard_down = false;
                        break;
                    case 4:
                        is_L_down = true;
                        is_R_down = true;
                        is_guard_down = false;
                        break;
                    case 5:
                        is_L_down = false;
                        is_R_down = false;
                        is_guard_down = true;
                        yield return new WaitForSeconds(2.0f);
                        break;
                }
            }
            yield return new WaitForSeconds(1.0f);
            is_L_down = false;
            is_R_down = false;
            is_guard_down = false;
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
        if(target_handler.target != null && target_handler.target.activeSelf != false) rotation_anchor.transform.LookAt(target_handler.target.transform);

        target_rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotation_anchor.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        if (!animator.GetBool("Is_Attacking"))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, 5f * Time.deltaTime);
            head_transform.LookAt(rotation_anchor.transform.position + rotation_anchor.transform.forward * 5f);
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

                // Â«Çª
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
        speed_h = input_key_sprint ? key_h * 2f : key_h;
        speed_v = input_key_sprint ? key_v * 2f : key_v;

        animator.SetFloat("Speed_H", Mathf.Lerp(animator.GetFloat("Speed_H"), speed_h, Time.deltaTime * 3f));
        animator.SetFloat("Speed_V", Mathf.Lerp(animator.GetFloat("Speed_V"), speed_v, Time.deltaTime * 3f));
    }
    private void Attack_Control()
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
