using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental;
using Entity_Data;
using UnityEngine.UI;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private Rigidbody rigidbody_self;
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
    private Transform cam;


    [SerializeField] private float EntityWidth = 0.25f;
    [SerializeField] private float EntityHeight = 2f;
    [SerializeField] private bool isGrounded;
    private Vector3 velocity;
    private World world;

    //블록 체크 관련된 것
    public Transform highlightBlock;
    public Transform placeBlock;
    public float checkIncrement = 0.1f;
    public float reach = 2f;

    public Text selectedBlockText;
    public byte selectedBlockIndex = 1; // 0 은 에어블록 -> isSolid 가 아님

    public int orientation;

    
    private ItemComponent[] inventorySlots; //사용할 인덱스는 0부터 8까지

    private InventoryOnOff inventoryOnOff;

    private void Awake()
    {
        world = FindObjectOfType<World>();
        TryGetComponent(out rigidbody_self);
        TryGetComponent(out entity);
        TryGetComponent(out animator);
        head_transform = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Head");
        position_anchor = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Head/Position_Anchor").gameObject;
        rotation_anchor = gameObject.transform.Find("Rotation_Anchor").gameObject;
        L_Hand = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Arm:Left:Upper/Arm:Left:Lower/Arm:Left:Hand").gameObject;
        R_Hand = gameObject.transform.Find("SimplePlayer.arma/center/Body/Chest/Arm:Right:Upper/Arm:Right:Lower/Arm:Right:Hand").gameObject;
        
        

    }

    private void Start()
    {
        
        inventorySlots = GetComponent<Inventory>().inv_Slot;
        cam = GameObject.Find("Main Camera").transform;
        inventoryOnOff = GetComponent<InventoryOnOff>();

        SelectedIndex();
    }

    private void Update()
    {
        isGrounded = Check_Ground();

        if(!inventoryOnOff.isInventoryOpen)
        {
            Move_Control();
            Attack_Control();
            PlaceCursorBlocks();
            SetDirection();

        }
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5f);

            for(int i = 200; i < 400; i++)
            {
                Item_Manager.instance.SpawnItem(i, newPos);
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

        if (animator.GetFloat("Speed_V") > 0f && front) transform.position += transform.forward * -0.1f;
        if (animator.GetFloat("Speed_V") < 0f && back) transform.position += transform.forward * 0.1f;
        if (animator.GetFloat("Speed_H") > 0f && right) transform.position += transform.right * -0.1f;
        if (animator.GetFloat("Speed_H") < 0f && left) transform.position += transform.right * 0.1f;
        if (down) transform.position += transform.up * 0.1f;
    }

    private void SelectedIndex()
    {
        //SetHighlightBlock();

        if (inventorySlots[selectedBlockIndex] == null)
        {
            selectedBlockText.text = $"{selectedBlockIndex + 1}";
        }
        else
        {
            selectedBlockText.text = inventorySlots[selectedBlockIndex].itemName;
        }
    }

    private void Rotation_Control()
    {
        cursor_h = input_cursor_h; //new
        cursor_v = input_cursor_v; //new
        
        cursor_x += cursor_h;
        cursor_y += cursor_v;
        cursor_y = Mathf.Clamp(cursor_y, -90f, 90f);

        if(!animator.GetBool("Is_Attacking") && (animator.GetBool("LR_Attack") || animator.GetBool("L_Attack") || animator.GetBool("R_Attack")) || animator.GetBool("Is_Guarding") || animator.GetBool("Is_Drawing")) temp_y = cursor_x;
        else if(key_h != 0 || key_v != 0) temp_y = cursor_x;
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


    //개선점 : mathf.abs 사용
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

                velocity = Vector3.zero;

                // 짬푸
                if (input_key_jump)
                {
                    animator.SetBool("IsJump", true);
                    animator.SetBool("IsGround", false);
                    isGrounded = false;
                    velocity = rigidbody_self.velocity + transform.up * jump_height * 9.8f * 0.5f;
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
            velocity += transform.up * -9.8f * 0.01f;
        }

        Move_Animation(key_h, key_v);
        if (velocity != Vector3.zero)
        {
            velocity = new Vector3(Mathf.Lerp(velocity.x, 0f, Time.deltaTime), velocity.y, Mathf.Lerp(velocity.z, 0f, Time.deltaTime));
            if (Mathf.Abs(velocity.x) < 0.01f) velocity.x = 0f;
            if (Mathf.Abs(velocity.z) < 0.01f) velocity.z = 0f; //?이건 머시여
            transform.position = transform.position + velocity * Time.deltaTime;
        }

        InvenScroll();
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

            if (!animator.GetBool("Is_Attacking")) animator.SetBool("Guard", is_guard_down);
        }
        else
        {
            animator.SetBool("LR_Attack", false);
            animator.SetBool("L_Attack", false);
            animator.SetBool("R_Attack", false);
            animator.SetBool("Guard", false);
        }
    }

    void SetDirection()
    {
        Vector3 XZDirection = transform.forward;
        XZDirection.y = 0;
        if (Vector3.Angle(XZDirection, Vector3.forward) <= 45)
            orientation = 0;
        else if (Vector3.Angle(XZDirection, Vector3.right) <= 45)
            orientation = 5;
        else if (Vector3.Angle(XZDirection, Vector3.back) <= 45)
            orientation = 1;
        else
            orientation = 4;
    }

    private void PlaceCursorBlocks()
    {
        float step = checkIncrement;
        // 마지막으로 탐지된 위치를 저장할 변수;
        Vector3 lastPos = new Vector3();

        // reach 거리만큼 반복
        while (step < reach)
        {

            // 카메라의 위치에서 카메라가 바라보고 있는 방향으로 Step 거리만큼 이동
            Vector3 pos = transform.position + (transform.forward * step);

            // 만약 거기에 Voxel이 있다면
            if (world.CheckForVoxel(pos))
            {


                // 화면에 표시인데 좌표가 안맞아서 내가 투명하게 해놓음;
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;
                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                // 블록 탐지됐으므로 메서드 종료
                return;
            }
            // 현재 위치를 마지막 위치로 업데이트
            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            // 탐지 간격을 증가시킴;
            step += checkIncrement;
        }
        // reach 거리 내에 블럭이 없으면 비활성화
        // 사실 내가 투명도 100으로 해서 안보임
        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);

    }

    private void InvenScroll()
    {
        HandleScrollInput();
        HandleNumberInput();
        SelectedIndex();

        if (highlightBlock.gameObject.activeSelf)
        {
            HandleBlockDestroy();
            HandleBlockPlacement();
        }
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
                selectedBlockIndex = (byte)((selectedBlockIndex + 1) % 9);
            else
                selectedBlockIndex = (byte)((selectedBlockIndex - 1 + 9) % 9);
        }
    }

    private void HandleNumberInput()
    {
        for (int i = 0; i <= 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedBlockIndex = (byte)i;
                break;
            }
        }
    }

    private void HandleBlockDestroy()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 destroyPos = highlightBlock.position;
            byte destroyedBlockID = world.GetChunkFromVector3(destroyPos).EditVoxel(destroyPos, 0);

            if (destroyedBlockID == 3)
            {
                destroyedBlockID = 5;
            }
            Item_Manager.instance.SetItem(destroyedBlockID, destroyPos, world);
        }
    }

    private void HandleBlockPlacement()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            byte blockID = world.GetChunkFromVector3(highlightBlock.position).CheckBlockID(highlightBlock.position);

            switch (blockID)
            {
                case 16:
                    //ui_chest.SetActive(true);
                    break;
                case 18:
                    //ui_furnance.SetActive(true);
                    break;
                case 19:
                    //ui_craftTable.SetActive(true);
                    break;
                default:
                    PlaceBlockOrUseItem();
                    break;
            }
        }
    }

    private void PlaceBlockOrUseItem()
    {
        if (inventorySlots[selectedBlockIndex] != null)
        {
            if (inventorySlots[selectedBlockIndex].SetType == 3) // 블록들
            {
                world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, (byte)inventorySlots[selectedBlockIndex].ItemID);
                if (inventorySlots[selectedBlockIndex].Use())
                {
                    inventorySlots[selectedBlockIndex] = null;
                }
            }
            else if (inventorySlots[selectedBlockIndex].SetType == 2) // 소비 아이템
            {
                // 아이템 사용 로직을 추가하세요.
                if (inventorySlots[selectedBlockIndex].Use())
                {
                    inventorySlots[selectedBlockIndex] = null;
                }
            }

            Inventory.instance.ChangeEvent();
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

    private bool Check_Ground()
    {
        if (world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 0.05f, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 0.05f, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 0.05f, transform.position.z + EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 0.05f, transform.position.z + EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y - 0.05f, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y - 0.05f, transform.position.z - EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y - 0.05f, transform.position.z + EntityWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y - 0.05f, transform.position.z + EntityWidth))
            )
        {
            if (world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 0.05f, transform.position.z - EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 0.05f, transform.position.z - EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + EntityWidth, transform.position.y + 0.05f, transform.position.z + EntityWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x - EntityWidth, transform.position.y + 0.05f, transform.position.z + EntityWidth)))
                return false;
            else return true;
        }
        else return false;
    }

    private bool front
    {
        get
        {
            if (world.CheckForVoxel(transform.position + transform.forward * EntityWidth + transform.up * EntityHeight * 0.3f) || world.CheckForVoxel(transform.position + transform.forward * EntityWidth + transform.up * EntityHeight))
            {
                if (world.CheckWaterForVoxel(transform.position + transform.forward * EntityWidth + transform.up * EntityHeight * 0.3f) || world.CheckWaterForVoxel(transform.position + transform.forward * EntityWidth + transform.up * EntityHeight)) return false;
                else return true;
            }
            else return false;
        }
    }
    private bool back
    {
        get
        {
            if (world.CheckForVoxel(transform.position - transform.forward * EntityWidth + transform.up * EntityHeight * 0.3f) || world.CheckForVoxel(transform.position - transform.forward * EntityWidth + transform.up * EntityHeight))
            {
                if (world.CheckWaterForVoxel(transform.position - transform.forward * EntityWidth + transform.up * EntityHeight * 0.3f) || world.CheckWaterForVoxel(transform.position - transform.forward * EntityWidth + transform.up * EntityHeight)) return false;
                else return true;
            }
            else return false;
        }
    }
    private bool left
    {
        get
        {
            if (world.CheckForVoxel(transform.position - transform.right * EntityWidth + transform.up * EntityHeight * 0.3f) || world.CheckForVoxel(transform.position - transform.right * EntityWidth + transform.up * EntityHeight))
            {
                if (world.CheckWaterForVoxel(transform.position - transform.right * EntityWidth + transform.up * EntityHeight * 0.3f) || world.CheckWaterForVoxel(transform.position - transform.right * EntityWidth + transform.up * EntityHeight)) return false;
                else return true;
            }
            else return false;
        }
    }
    private bool right
    {
        get
        {
            if (world.CheckForVoxel(transform.position + transform.right * EntityWidth + transform.up * EntityHeight * 0.3f) || world.CheckForVoxel(transform.position + transform.right * EntityWidth + transform.up * EntityHeight))
            {
                if (world.CheckWaterForVoxel(transform.position + transform.right * EntityWidth + transform.up * EntityHeight * 0.3f) || world.CheckWaterForVoxel(transform.position + transform.right * EntityWidth + transform.up * EntityHeight)) return false;
                else return true;
            }
            else return false;
        }
    }
    private bool down
    {
        get
        {
            if (world.CheckForVoxel(transform.position + transform.up * EntityHeight * 0.1f))
            {
                if (world.CheckWaterForVoxel(transform.position + transform.up * EntityHeight * 0.1f)) return false;
                else return true;
            }
            else return false;
        }
    }
}