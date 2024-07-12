using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryBarChoice : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots = null;
    [SerializeField] private GameObject obj = null;
    [SerializeField] private InventorySlot SelectedSlots;


    private void Start()
    {
        slots = GetComponentsInChildren<InventorySlot>();
    }


    private void Update()
    {
        for (int i = 1; i <= slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                
                //Debug.Log($"{i}");

                
                SelectedSlots = slots[i - 1];
                Debug.Log(SelectedSlots.name);
                SelectedSlots.GetComponent<Image>().color = Color.red;


            }
        }
    }
}