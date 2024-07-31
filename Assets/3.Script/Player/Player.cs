using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// Player의 움직임, 물리연산, 블록 배치 및 파괴가 있음
// Rigidbody의 물리연산을 쓰지 않고 직접 작성...
// World가 Collider를 전부 활성화한채로 게임을 시작한다면 무진장 렉걸려서 
// 플레이어가 접촉한 부분만 Collider를 활성화 할꺼임
//
//
//
//
//
//
//

public class Player : MonoBehaviour
{

    public float health;
    public string worldName;
    public Inventory inventory;

    private Transform cam;
    private World world;

    // ------------캐릭터 움직임---------------//
    public bool isGrounded;
    public bool isSprinting;

    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;

    public float playerWidth = 0.3f;

    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;

    //[SerializeField] private float mouseSensitive;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;
    public bool isOpeningUI = false;
    //[HideInInspector]
    //-------------움직임 끝 -----------------//



    // 블록 파괴 및 배치 관련 변수들
    public Transform highlightBlock;
    public Transform placeBlock;
    public float checkIncrement = 0.1f;
    public float reach = 8f;

    public Text selectedBlockText;
    public byte selectedBlockIndex = 1; // 0 은 에어블록 -> isSolid 가 아님

    public int orientation;


    //--------------UI?------------------------//
    public GameObject ui_craftTable;
    public GameObject ui_chest;
    public GameObject ui_furnance;
    public Transform ContainerUI;
    
    private ItemComponent[] inventorySlots; //사용할 인덱스는 0부터 8까지
    //--------------UI end---------------------//

    //조영준 커스텀 시작
    private Item_Manager itemManager;
    private Player_Control controller;


    // 초기화
    private void Start()
    {

        PlayerData data = SaveSystem.LoadPlayer(worldName);

        cam = GameObject.Find("Main Camera").transform;
        //world = GameObject.Find("World").GetComponent<World>();
        world = FindAnyObjectByType<World>();
        itemManager = FindObjectOfType<Item_Manager>();
        controller = GetComponent<Player_Control>();




        inventorySlots = GetComponent<Inventory>().inv_Slot;
        if (itemManager == null)
        {
            //Debug.LogError("Fuck");
        }

        

        // 마우스 커서가 Game 밖으로 안나가게 고정
        Cursor.lockState = CursorLockMode.Locked;

        SelectedIndex();

        if (data != null) {
            health = data.health;
            Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
            transform.position = position;
      
            inventory.Clear();
            foreach (Item_Stackable_Data itemData in data.inventory) {
                ItemComponent itemComponent = new ItemComponent(); // ItemComponent 초기화
                itemComponent.ItemID = itemData.item_ID;
                itemComponent.stackCurrent = itemData.stackCurrent;
                itemComponent.item_name = itemData.item_name;
                itemComponent.item_model_in_world = itemData.item_model_in_world;
                itemComponent.item_model_in_inventory = itemData.item_model_in_inventory;
                itemComponent.stack_max = itemData.stack_max;

                InventoryItem item = new InventoryItem();
                item.Initialize(itemComponent, null); // 필요한 경우 parent를 설정
                inventory.Add(item);
            }
        }
    }

    private void OnApplicationQuit() {
        SaveSystem.SavePlayer(this);
    }




    private void SelectedIndex()
    {
        //SetHighlightBlock();

        if (inventorySlots[selectedBlockIndex] == null)
        {
            selectedBlockText.text = $"{selectedBlockIndex + 1} slot NONE block selected";
        }
        else
        {
            selectedBlockText.text = inventorySlots[selectedBlockIndex].itemName + " block selected";
        }
    }

    // 고정된 시간마다 호출, 불필요한 연산 줄이고 정확한 물리연산
    private void FixedUpdate()
    {
       CalculateVelocity();
       if (jumpRequest)
           Jump();
       
       transform.Rotate(Vector3.up * mouseHorizontal * world.settings.mouseSensitivity);
       cam.Rotate(Vector3.right * -mouseVertical * world.settings.mouseSensitivity);
       transform.Translate(velocity, Space.World);
    }



    // 매 프레임마다 호출, 비물리적 연산, 부드러운 움직임
    private void Update()
    {
        GetPlayerInputs();
        PlaceCursorBlocks();
        SetDirection();
        
    }




    void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
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
            Vector3 pos = cam.position + (cam.forward * step);

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






    private void CalculateVelocity()
    {

        // 중력 계산
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;


        // 걷기 & 달리기
        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

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





    // 캐릭터 컨트롤 인풋
    private void GetPlayerInputs()
    {
        if (isOpeningUI == false)
        {
           //horizontal = Input.GetAxis("Horizontal");
           //vertical = Input.GetAxis("Vertical");
           //mouseHorizontal = Input.GetAxis("Mouse X");
           //mouseVertical = Input.GetAxis("Mouse Y");
            
            
            
            
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
            
            
            
            if (Input.GetButtonDown("Sprint"))
                isSprinting = true;
            if (Input.GetButtonUp("Sprint"))
                isSprinting = false;
            
            if (isGrounded && Input.GetButtonDown("Jump"))
                jumpRequest = true;
            
            CreativeScroll();

            InvenScroll();
        }
        else
        {
            return;
        }
    }

    private void CreativeScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
                selectedBlockIndex++;
            else
                selectedBlockIndex--;

            if (selectedBlockIndex > (byte)(world.blockTypes.Length - 1))
                selectedBlockIndex = 1;
            if (selectedBlockIndex < 1)
                selectedBlockIndex = (byte)(world.blockTypes.Length - 1);

            selectedBlockText.text = world.blockTypes[selectedBlockIndex].blockName + " block selected";
        }

        if (highlightBlock.gameObject.activeSelf)
        {


            // 블록 파괴 Destroy block
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 destroyPos = highlightBlock.position;
                byte destroyedBlockID = world.GetChunkFromVector3(destroyPos).EditVoxel(destroyPos, 0);

                if (destroyedBlockID == 3)
                {
                    destroyedBlockID = 5;
                }


                //GameObject popObjectPrefab = Resources.Load<GameObject>("PopObject"); //찾았다 내사랑, 아이템 매니저의 spawnitem이랑 연동하면 끝

                itemManager.SetItem(destroyedBlockID, destroyPos, world);
                //GameObject popObjectInstance = itemManager.SpawnItem(destroyedBlockID, destroyPos);
                //
                //PopObject popObject = popObjectInstance.GetComponent<PopObject>();
                //
                //popObject.Initialize(world, destroyPos, destroyedBlockID);
            }




            // 블록 배치 Place block
            if (Input.GetMouseButtonDown(1))
            {
                switch (world.GetChunkFromVector3(highlightBlock.position).CheckBlockID(highlightBlock.position))
                {
                    // chest
                    case 16:
                        ui_chest.SetActive(true);
                        break;

                    // Furnance
                    case 18:
                        ui_furnance.SetActive(true);
                        break;

                    // craftTable
                    case 19:
                        ui_craftTable.SetActive(true);
                        break;
                    default:
                        world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, selectedBlockIndex);
                        break;
                }



            }



        }
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

            itemManager.SetItem(destroyedBlockID, destroyPos, world);
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
                    ui_chest.SetActive(true);
                    break;
                case 18:
                    ui_furnance.SetActive(true);
                    break;
                case 19:
                    ui_craftTable.SetActive(true);
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





    // 떨어지는거 계산
    private float CheckDownSpeed(float downSpeed)
    {
        if (world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)))
        {
            if (world.CheckWaterForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)))
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
        if (world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)))
        {
            if (world.CheckWaterForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)) ||
                world.CheckWaterForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)))
                return upSpeed;
            else
                return 0;
        }
        else
        {
            return upSpeed;
        }
    }

    /// ============================================================================ //
    /// =========================앞 뒤 양 옆 복셀 검사=============================== //
    /// ============================================================================ //
    ///                    true 면 블록이 있다는 뜻
    ///                    false 면 블록이 없다는 뜻




    public bool front
    {
        get
        {
            if (World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
               World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth)))
            {
                if (World.Instance.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth))||
                    World.Instance.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth)))
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

            if (World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                World.Instance.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth)))
            {
                if (World.Instance.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                    World.Instance.CheckWaterForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth)))
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

            if (World.Instance.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                World.Instance.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z)))
            {
                if (World.Instance.CheckWaterForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                    World.Instance.CheckWaterForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z)))
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

            if (World.Instance.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                World.Instance.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z)))
            {
                if (World.Instance.CheckWaterForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                    World.Instance.CheckWaterForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z)))
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
