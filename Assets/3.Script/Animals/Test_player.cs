using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_player : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);


    }
}
