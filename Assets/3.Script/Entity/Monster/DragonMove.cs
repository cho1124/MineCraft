using System.Collections;
using UnityEngine;


public class DragonMove : MonoBehaviour
{
    public float waveFrequency = 2.0f;  // ���̺� ���ļ�
    public float waveAmplitude = 0.5f;  // ���̺� ����
    public float forwardSpeed = 5.0f;   // ���� �ӵ�
    public float verticalSpeed = 2.0f;  // ���� �ӵ� (���Ʒ� ������)

    private float waveOffset;           // ���̺� ������
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
        // ���� �̵�
        transform.Translate(currentDirection * forwardSpeed * Time.deltaTime);

        // ���̺� ��� (���Ʒ�)
        float wave = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        float verticalMove = Mathf.Sin(Time.time * verticalSpeed) * waveAmplitude;

        // Y�� ��ġ ������Ʈ
        Vector3 newPosition = transform.position;
        newPosition.y = waveOffset + wave + verticalMove;
        transform.position = newPosition;

        // ���� ����
        Vector3 lookDirection = currentDirection;
        lookDirection.z = 0; // y�� ȸ���� �����ϱ� ���� y�� 0���� ����
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
        // �������� ���ο� ���� ���� (X, Z ��� ������)
        float angle = Random.Range(0, 360);
        currentDirection = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
    }





}
