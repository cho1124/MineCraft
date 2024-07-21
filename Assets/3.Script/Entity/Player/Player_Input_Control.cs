using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input_Control : MonoBehaviour
{
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input != null)
        {
            Debug.Log($"UNITY_EVENTS : {input.x}, {input.y}");
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if(input != null)
        {
            //Debug.Log($"UNITY_EVENTS : {input.x}, {input.y}");
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)  // Action Type�� "Button"�� ��� Ű�� ���ȴ��� üũ
        {
            // ���� ����
        }
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
    }
}
