using UnityEngine;

public class WormController : MonoBehaviour
{
    public Transform[] segments; // 지렁이 세그먼트들
    public float smoothTime = 0.3f; // 댐프 시간
    public float maxSpeed = 10f; // 최대 속도
    public float moveSpeed = 5f; // 이동 속도

    private Vector3[] velocities; // 세그먼트 속도 저장 배열

    void Start()
    {
        velocities = new Vector3[segments.Length];
    }

    void Update()
    {
        // 플레이어 입력 처리
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

        // 머리 세그먼트 이동
        segments[0].position = Vector3.SmoothDamp(segments[0].position, targetPosition, ref velocities[0], smoothTime, maxSpeed);

        // 나머지 세그먼트 이동
        for (int i = 1; i < segments.Length; i++)
        {
            segments[i].position = Vector3.SmoothDamp(segments[i].position, segments[i - 1].position, ref velocities[i], smoothTime, maxSpeed);
        }
    }
}
