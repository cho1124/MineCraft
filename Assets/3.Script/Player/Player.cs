using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    //
    // Player�� ������, ��������, ��� ��ġ �� �ı��� ����
    // Rigidbody�� ���������� ���� �ʰ� ���� �ۼ�...
    // World�� Collider�� ���� Ȱ��ȭ��ä�� ������ �����Ѵٸ� ������ ���ɷ��� 
    // �÷��̾ ������ �κи� Collider�� Ȱ��ȭ �Ҳ���
    //
    //
    //
    //
    //
    //
    //

    // ī�޶�
    private Transform cam;
    private World world;


    // ------------ĳ���� ������---------------//
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

    [SerializeField] private float mouseSensitive;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    // ------------ĳ���� ������ �� ---------------//



    // ��� �ı� �� ��ġ
    public Transform highlightBlock;
    public Transform placeBlock;
    public float checkIncrement = 0.1f;
    public float reach = 8f;

    public Text selectedBlockText;
    public byte selectedBlockIndex = 1; // 0 �� ������ -> isSolid �� �ƴ�


    // �ʱ�ȭ
    private void Start()
    {

        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();


        // ���콺 Ŀ���� Game ������ �ȳ����� ����
        Cursor.lockState = CursorLockMode.Locked;

        selectedBlockText.text = world.blockTypes[selectedBlockIndex].blockName + " blcok selected";

    }


    // ������ �ð����� ȣ��, ���ʿ��� ���� ���̰� ��Ȯ�� ��������

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

        transform.Rotate(Vector3.up * mouseHorizontal * mouseSensitive);
        cam.Rotate(Vector3.right * -mouseVertical * mouseSensitive);

        Vector3 newPosition = transform.position + velocity;
        //if (world.IsVoxelInWorld(newPosition))
        //{
        transform.Translate(velocity, Space.World);
        //}
        //else
        //{
        //    velocity = Vector3.zero;
        //}

    }

    // �� �����Ӹ��� ȣ��, �񹰸��� ����, �ε巯�� ������
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


    // ��� ��ġ


    private void PlaceCursorBlocks()
    {
        float step = checkIncrement;

        // ���������� Ž���� ��ġ�� ������ ����;
        Vector3 lastPos = new Vector3();

        // reach �Ÿ���ŭ �ݺ�
        while (step < reach)
        {
            // ī�޶��� ��ġ���� ī�޶� �ٶ󺸰� �ִ� �������� Step �Ÿ���ŭ �̵�
            Vector3 pos = cam.position + (cam.forward * step);

            // ���� �ű⿡ Voxel�� �ִٸ�
            if (world.CheckForVoxel(pos))
            {
                // ���̶���Ʈ ����� �ű⿡ ��ġ
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));


                placeBlock.position = lastPos;

                // ȭ�鿡 ǥ���ε� ��ǥ�� �ȸ¾Ƽ� ���� �����ϰ� �س���;
                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                // ��� Ž�������Ƿ� �޼��� ����
                return;
            }
            // ���� ��ġ�� ������ ��ġ�� ������Ʈ
            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            // Ž�� ������ ������Ŵ;
            step += checkIncrement;

        }
        // reach �Ÿ� ���� ���� ������ ��Ȱ��ȭ
        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);

    }

    // ���� ���
    private void CalculateVelocity()
    {

        // �߷� ���
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        // �ȱ� + �޸��� ���
        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        // ������ ������ �����ӵ�
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        // ��, ��, ��, �� �浹�˻�
        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        // ���� �ӵ� �˻� �� ����
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

            selectedBlockText.text = world.blockTypes[selectedBlockIndex].blockName + " blcok selected";
        }

        if (highlightBlock.gameObject.activeSelf)
        {
            // ��� �ı� Destroy block
            if (Input.GetMouseButtonDown(0))
                world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);

            // ��� ��ġ Place block
            if (Input.GetMouseButtonDown(1))
                world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, selectedBlockIndex);
        }

    }

    // �÷��̾ �ϰ� ���϶� �浹����, �ӵ�����
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

    // ������϶� �浹����, �ӵ�����
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

    /// ============================================================================ //
    /// =========================�� �� �� �� ���� �˻�=============================== //
    /// ============================================================================ //
    ///                    true �� ����� �ִٴ� ��
    ///                    false �� ����� ���ٴ� ��

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