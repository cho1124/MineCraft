using System.Collections;
using UnityEngine;


public class DragonMove : MonoBehaviour
{
    public float waveFrequency = 2.0f;  // 웨이브 주파수
    public float waveAmplitude = 0.5f;  // 웨이브 진폭
    public float forwardSpeed = 5.0f;   // 전진 속도
    public float verticalSpeed = 2.0f;  // 수직 속도 (위아래 움직임)

    private float waveOffset;           // 웨이브 오프셋
    private Vector3 currentDirection = Vector3.forward;


    void Start()
    {
        waveOffset = transform.position.y;
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        // 전진 이동
        transform.Translate(currentDirection * forwardSpeed * Time.deltaTime);

        // 웨이브 모션 (위아래)
        float wave = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        float verticalMove = Mathf.Sin(Time.time * verticalSpeed) * waveAmplitude;

        // Y축 위치 업데이트
        Vector3 newPosition = transform.position;
        newPosition.y = waveOffset + wave + verticalMove;
        transform.position = newPosition;

        // 방향 설정
        Vector3 lookDirection = currentDirection;
        lookDirection.z = 0; // y축 회전을 방지하기 위해 y를 0으로 설정
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        // 무작위로 새로운 방향 선택 (X, Z 평면 내에서)
        float angle = Random.Range(0, 360);
        currentDirection = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
    }





}
