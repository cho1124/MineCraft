using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private Rigidbody player_rigidbody;
    [SerializeField] private Transform head_transform;
    private float cursor_x = 0f;
    private float cursor_y = 0f;
    private float temp_y = 0f;

    private void Awake()
    {
        TryGetComponent(out player_rigidbody);
        head_transform = transform.GetChild(1).transform;
    }

    private void LateUpdate()
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        cursor_x += h * 3f;
        cursor_y += v * 1.5f;
        cursor_y = Mathf.Clamp(cursor_y, -90f, 90f);

        Debug.Log(Difference(cursor_x, temp_y));

        if (Difference(cursor_x, temp_y) > 45f)
        {
            temp_y += h * 3f;
            transform.rotation = Quaternion.Euler(transform.rotation.x, temp_y, transform.rotation.z);
            head_transform.rotation = Quaternion.Euler(cursor_y, cursor_x, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, temp_y, transform.rotation.z);
            head_transform.rotation = Quaternion.Euler(cursor_y, cursor_x, 0);
        }
    }

    private float Difference(float a, float b)
    {
        if (a < b) return b - a;
        else if (a > b) return a - b;
        else return 0;
    }
}