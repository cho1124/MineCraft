using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Move : MonoBehaviour
{
    private Vector3 MoveDirection = Vector3.zero;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ItemMove();
    }

    void ItemMove()
    {
        
        Vector3 rayStartPosition = transform.position + Vector3.up * transform.localScale.y;
        Vector3 rayDirection = Vector3.down;

        
        RaycastHit hit;
        bool isGrounded = false;

        
        if (Physics.Raycast(rayStartPosition, rayDirection, out hit, 100f))
        {
            
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }

        
        if (!isGrounded)
        {
            MoveDirection.y = -1f;
        }
        else
        {
            MoveDirection.y = 0f;
        }
    }

}
