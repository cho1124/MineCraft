using UnityEngine;

public class WormController : MonoBehaviour
{
    public Transform[] segments; // ������ ���׸�Ʈ��
    public float smoothTime = 0.3f; // ���� �ð�
    public float maxSpeed = 10f; // �ִ� �ӵ�
    public float moveSpeed = 5f; // �̵� �ӵ�

    private Vector3[] velocities; // ���׸�Ʈ �ӵ� ���� �迭

    void Start()
    {
        velocities = new Vector3[segments.Length];
    }

    void Update()
    {
        // �÷��̾� �Է� ó��
        Vector3 targetPosition = segments[0].position;

        if (Input.GetKey(KeyCode.W))
        {
            targetPosition += Vector3.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            targetPosition += Vector3.back * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            targetPosition += Vector3.left * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            targetPosition += Vector3.right * moveSpeed * Time.deltaTime;
        }

        // �Ӹ� ���׸�Ʈ �̵�
        segments[0].position = Vector3.SmoothDamp(segments[0].position, targetPosition, ref velocities[0], smoothTime, maxSpeed);

        // ������ ���׸�Ʈ �̵�
        for (int i = 1; i < segments.Length; i++)
        {
            segments[i].position = Vector3.SmoothDamp(segments[i].position, segments[i - 1].position, ref velocities[i], smoothTime, maxSpeed);
        }
    }
}
