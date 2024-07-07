using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
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
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    // ------------캐릭터 움직임 끝 ---------------//



    // 블록 파괴 및 배치
    public Transform highlightBlock;
    public Transform placeBlock;
    public float checkIncrement = 0.1f;
    public float reach = 8f;

    public Text selectedBlockText;
    public byte selefctedBlockIndex = 1; // 0 은 에어블록 -> isSolid 가 아님

    private void Start()
    {

        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();

        Cursor.lockState = CursorLockMode.Locked;

        selectedBlockText.text = world.blockTypes[selefctedBlockIndex].blockName + " blcok selected";

    }


    // 고정된 시간마다 호출, 불필요한 연산 줄이고 정확한 물리연산
    private void FixedUpdate()
    {

        //CalculateVelocity();
        //if (jumpRequest)
        //    Jump();
        //
        //transform.Rotate(Vector3.up * mouseHorizontal);
        //cam.Rotate(Vector3.right * -mouseVertical);
        //transform.Translate(velocity, Space.World);

        CalculateVelocity();
        if (jumpRequest)
            Jump();

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);

        Vector3 newPosition = transform.position + velocity;
        if (world.IsVoxelInWorld(newPosition))
        {
            transform.Translate(velocity, Space.World);
        }
        else
        {
            velocity = Vector3.zero;
        }

    }

    // 매 프레임마다 호출, 비물리적 연산, 부드러운 움직임
    private void Update()
    {

        GetPlayerInputs();
        PlaceCursorBlocks();

    }

    void Jump()
    {

        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;

    }


    // 블록 배치
    private void PlaceCursorBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while(step < reach)
        {
            Vector3 pos = cam.position + (cam.forward * step);

            if(world.CheckForVoxel(pos))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;
            }
            
            lastPos  = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;

        }

        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);

    }

    // 물리 계산
    private void CalculateVelocity()
    {

        // 중력 계산
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        // 걷기 + 달리기 계산
        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        // 공중의 있을때 수직속도
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = CheckDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = CheckUpSpeed(velocity.y);


    }

    private void GetPlayerInputs()
    {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;

        if (isGrounded && Input.GetButtonDown("Jump"))
            jumpRequest = true;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(scroll != 0)
        {
            if (scroll > 0)
                selefctedBlockIndex++;
            else
                selefctedBlockIndex--;

            if (selefctedBlockIndex > (byte)(world.blockTypes.Length - 1))
                selefctedBlockIndex = 1;
            if (selefctedBlockIndex < 1)
                selefctedBlockIndex = (byte)(world.blockTypes.Length - 1);

            selectedBlockText.text = world.blockTypes[selefctedBlockIndex].blockName + " blcok selected";
        }

        if(highlightBlock.gameObject.activeSelf)
        {
            // 블록 파괴 Destroy block
            if (Input.GetMouseButtonDown(0))
                world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);

            // 블록 배치 Place block
            if (Input.GetMouseButtonDown(1))
                world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, selefctedBlockIndex);
        }

    }

    private float CheckDownSpeed(float downSpeed)
    {

        if (
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))
           )
        {

            isGrounded = true;
            return 0;

        }
        else
        {

            isGrounded = false;
            return downSpeed;

        }

    }

    private float CheckUpSpeed(float upSpeed)
    {

        if (
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth))
           )
        {

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
            if (
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
                )
                return true;
            else
                return false;
        }

    }
    public bool back
    {

        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
                )
                return true;
            else
                return false;
        }

    }
    public bool left
    {

        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }

    }
    public bool right
    {

        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }

    }

}