using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOnOff : MonoBehaviour
{
    [SerializeField] private GameObject on_off_obj = null;
    [HideInInspector] public bool on_off_tr = false; // 인벤토리, Dead UI ON, OFF 선언 (Bool)

    private void Start()
    {
        on_off_obj.SetActive(on_off_tr);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E키를 누른다면
        {
            on_off_tr = !on_off_tr; // 인벤토리 UI ON, OFF 체크
        
            on_off_obj.SetActive(on_off_tr); // 인벤토리 UI ON, OFF 기능
        }
    }
}