using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private InventoryItemControl InventoryItemControl_class;

    [Header("조합 슬롯"), SerializeField] private Transform[] crafting_slot;
    [Header("조합 슬롯 결과"), SerializeField] private GameObject new_item_obj;
    [Header("팝업"), SerializeField] private GameObject on_off_obj;

    [Header("디버그")]
    public bool on_off_tr = false;
    [SerializeField] private bool full_crafting_slot = false;

    private void Start()
    {
        InventoryItemControl_class = FindObjectOfType<InventoryItemControl>();

        on_off_obj.SetActive(false);
        new_item_obj.SetActive(false);
    }

    private void Update()
    {
        if (on_off_tr)
        {
            Crafting_slot();
            Delete_item();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            on_off_tr = !on_off_tr;

            on_off_obj.SetActive(on_off_tr);
        }
    }

    public void Crafting_slot()
    {
        if (crafting_slot[0].childCount == 1 && crafting_slot[1].childCount == 1 && crafting_slot[2].childCount == 1 && crafting_slot[3].childCount == 1)
        {
            full_crafting_slot = true;
            new_item_obj.SetActive(true);
        }
        else
        {
            full_crafting_slot = false;
            new_item_obj.SetActive(false);
            
        }
    }

    public void Delete_item()
    {
        Debug.Log(InventoryItemControl_class.click_tr);

        if (InventoryItemControl_class.click_tr && full_crafting_slot)
        {
            for (int i = 0; i < crafting_slot.Length; i++)
            {
                Debug.Log("실행!");

                Destroy(crafting_slot[i].GetChild(0).gameObject);
            }
        }
    }
}